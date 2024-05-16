#Region "Microsoft.VisualBasic::668b6353ce21ca7c287f84009ff4dcdd, src\graphQL\KnowledgeBuilder\CreateKnowledge.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 198
    '    Code Lines: 131
    ' Comment Lines: 42
    '   Blank Lines: 25
    '     File Size: 7.54 KB


    ' Module CreateKnowledge
    ' 
    '     Function: ComputeKnowlegdes, ExtractKnowledges, getSharedNodes, GroupMeltdown, SplitKnowledges
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports Node = Microsoft.VisualBasic.Data.visualize.Network.Graph.Node

Public Module CreateKnowledge

    <Extension>
    Public Iterator Function ExtractKnowledges(graph As NetworkGraph, Optional eps As Double = 0.00000001) As IEnumerable(Of KnowledgeFrameRow)
        ' 解析出所有的信息孤岛
        Dim islands As IEnumerable(Of NetworkGraph) = graph.IteratesSubNetworks(singleNodeAsGraph:=False)
        Dim islandId As i32 = 1

        ' 并行计算知识分区
        For Each gc As NetworkGraph In islands _
            .AsParallel _
            .Select(Function(g)
                        Return g.ComputeKnowlegdes(eps)
                    End Function)

            For Each term As KnowledgeFrameRow In gc.SplitKnowledges()
                term.UniqeId = ++islandId
                Yield term
            Next
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gc">
    ''' 假设目标节点已经是完成了:
    ''' 
    ''' 1. 社区分组计算操作
    ''' 2. degree计算
    ''' </param>
    ''' <returns></returns>
    Public Function GroupMeltdown(gc As NetworkGraph, Optional equals As Double = 0.75) As NetworkGraph
        ' 找出所有的hub节点
        ' 将hub节点定义为每一个分组中degree值最高的那个节点
        Dim groups = gc.vertex _
            .GroupBy(Function(i) i.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)) _
            .ToDictionary(Function(v) v.Key,
                          Function(v)
                              Return v.ToArray
                          End Function)
        Dim hubs = groups _
            .Select(Function(group)
                        Return group _
                            .Value _
                            .OrderByDescending(Function(v) v.degree.In + v.degree.Out) _
                            .First
                    End Function) _
            .ToArray
        ' 计算出任意两个hub节点之间的共享节点数量
        Dim shareMatrix As New Dictionary(Of String, Dictionary(Of String, Double))
        Dim n As Integer = gc.vertex.Count

        For Each v In hubs
            Dim vector As New Dictionary(Of String, Double)

            For Each u In hubs
                vector(u.label) = gc.getSharedNodes(v, u)
            Next

            shareMatrix(v.label) = vector
        Next

        ' 计算一下分位数
        Dim q = shareMatrix.Values.Select(Function(r) r.Values).IteratesALL.GKQuantile
        Dim alignment As New HubAlignment(shareMatrix, equals:=q.Query(equals) + 1, gt:=1)
        Dim rooTree As BTreeCluster = shareMatrix.Keys.BTreeCluster(alignment)
        Dim poll As New List(Of BTreeCluster)

        Call Console.WriteLine(q.ToString)
        Call KnowledgeFrameRow.SaveData(rooTree, save:=poll)

        Dim meltGroupId As Integer = 1
        Dim hubTable = hubs.ToDictionary(Function(v) v.label)

        For Each node As BTreeCluster In poll
            Dim groupKeys = node.members

            For Each key As String In groupKeys
                For Each v As Node In groups(hubTable(key).data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE))
                    v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = meltGroupId
                Next
            Next

            meltGroupId += 1
        Next

        Return gc
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gc">
    ''' 假设目标节点已经是完成了:
    ''' 
    ''' 1. 社区分组计算操作
    ''' 2. degree计算
    ''' </param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Iterator Function SplitKnowledges(gc As NetworkGraph, Optional equals As Double = 0.75) As IEnumerable(Of KnowledgeFrameRow)
        Dim groups = GroupMeltdown(gc, equals).vertex _
            .GroupBy(Function(i)
                         Return i.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
                     End Function) _
            .ToDictionary(Function(v) v.Key,
                          Function(v)
                              Return v.ToArray
                          End Function)

        For Each group In groups
            Dim hits As Index(Of String) = group.Value.Select(Function(v) v.label).Indexing
            Dim metadata = gc.graphEdges _
                .Where(Function(url)
                           Return url.U.label Like hits OrElse url.V.label Like hits
                       End Function) _
                .Select(Function(url) {url.U, url.V}) _
                .IteratesALL _
                .GroupBy(Function(v) v.label) _
                .Select(Function(v) v.First) _
                .GroupBy(Function(v)
                             Return v.data("knowledge_type")
                         End Function) _
                .ToArray
            Dim props As New Dictionary(Of String, String())

            For Each p In metadata
                Call props.Add(p.Key, (From v As Node In p Select v.label).ToArray)
            Next

            Yield New KnowledgeFrameRow With {
                .UniqeId = group.Key,
                .Properties = props
            }
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="hub1"></param>
    ''' <param name="hub2"></param>
    ''' <returns>
    ''' 使用共享节点来描述两个hub节点之间的距离
    ''' length(v) / shares
    ''' 
    ''' 共享节点数量越多，则对应的比值越小，说明两个hub节点的距离越近
    ''' </returns>
    <Extension>
    Public Function getSharedNodes(g As NetworkGraph, hub1 As Node, hub2 As Node) As Double
        ' hub1 --- shared --- hub2
        Dim edge1 = hub1.adjacencies.EnumerateAllEdges.Select(Function(link) link.Other(hub1)).ToArray
        Dim edge2 = hub2.adjacencies.EnumerateAllEdges.Select(Function(link) link.Other(hub2)).ToArray
        Dim shares = edge1.Select(Function(v) v.label).Intersect(edge2.Select(Function(v) v.label)).ToArray
        Dim n As Integer = shares.Length

        Return n
    End Function

    <Extension>
    Public Function ComputeKnowlegdes(graph As NetworkGraph, eps As Double) As NetworkGraph
        ' rebuild graph for fix id reference
        ' to run communities analysis
        Dim rebuild As New NetworkGraph

        For Each g In graph.vertex
            Call rebuild.CreateNode(g.label, g.data)
        Next
        For Each l In graph.graphEdges
            rebuild.AddEdge(rebuild.CreateEdge(
                rebuild.GetElementByID(l.U.label),
                rebuild.GetElementByID(l.V.label),
                l.weight,
                l.data
            ))
        Next

        Call Communities.Analysis(rebuild, eps:=eps)
        Call rebuild.ComputeNodeDegrees

        Return rebuild
    End Function
End Module
