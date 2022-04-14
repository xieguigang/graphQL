Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports graphQL
Imports graphQL.Graph
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.DBSCAN
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.DataMining.UMAP
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports Node = Microsoft.VisualBasic.Data.visualize.Network.Graph.Node
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("KnowledgeGraph")>
Module KnowledgeGraph

    ''' <summary>
    ''' export the graph database as the 
    ''' network graph model for run 
    ''' algorithm debug.
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <returns>
    ''' nodes meta: knowledge_type
    ''' </returns>
    <ExportAPI("networkGraph")>
    Public Function networkGraph(kb As GraphModel,
                                 <RRawVectorArgument>
                                 Optional filters As Object = Nothing) As NetworkGraph

        Dim filterList As String() = REnv.asVector(Of String)(filters)
        Dim graph As NetworkGraph = kb.CreateGraph(filters:=filterList)

        Return graph
    End Function

    <ExportAPI("Kosaraju.SCCs")>
    Public Function KosarajuSCCs(g As NetworkGraph) As NetworkGraph
        Dim result = Kosaraju.StronglyConnectedComponents(g)
        Dim sccs = result.GetComponents.ToArray
        Dim i As Integer = 0

        For Each block In sccs
            i += 1

            For Each link In block
                link.U.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = i
                link.V.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = i
            Next
        Next

        Dim colors As LoopArray(Of Color) = GDIColors.AllDotNetPrefixColors

        For Each group In g.vertex.GroupBy(Function(v) v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE))
            Dim color As String = colors.Next.ToHtmlColor

            For Each v In group
                v.data(NamesOf.REFLECTION_ID_MAPPING_NODECOLOR) = color
            Next
        Next

        Return g
    End Function

    <ExportAPI("graphUMAP")>
    Public Function graphUMAP(g As NetworkGraph, Optional eps As Double = 0.1) As Object
        Dim labels As String() = Nothing
        Dim umap As Umap = g.RunUMAP(labels)
        Dim embedding = umap.GetEmbedding
        Dim raw As ClusterEntity() = labels _
            .Select(Function(id, i)
                        Dim vec = embedding(i)
                        Dim point As New ClusterEntity With {
                            .uid = id,
                            .entityVector = vec
                        }

                        Return point
                    End Function) _
            .ToArray

        ' run dbscan
        Dim dbscan As New DbscanAlgorithm(Of ClusterEntity)(Function(x, y) x.entityVector.EuclideanDistance(y.entityVector))
        Dim result = dbscan.ComputeClusterDBSCAN(raw, eps, 5)
        Dim mat As New List(Of ClusterEntity)
        Dim idx As Integer = 0

        For Each group In result
            idx += 1

            For Each x In group
                x.cluster = idx
            Next

            mat.AddRange(group)
        Next

        Dim frame As New dataframe With {
            .rownames = mat.Select(Function(r) r.uid).ToArray,
            .columns = New Dictionary(Of String, Array) From {
                {"group", mat.Select(Function(r) r.cluster).ToArray}
            }
        }

        frame.columns("x") = mat.Select(Function(r) r.entityVector(0)).ToArray
        frame.columns("y") = mat.Select(Function(r) r.entityVector(1)).ToArray
        frame.columns("z") = mat.Select(Function(r) r.entityVector(2)).ToArray

        Return frame
    End Function

    <ExportAPI("knowledgeIslands")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function knowledgeIslands(graph As NetworkGraph) As pipeline
        Dim list As IEnumerable(Of NetworkGraph) =
            Iterator Function() As IEnumerable(Of NetworkGraph)
                For Each g As NetworkGraph In IteratesSubNetworks(Of Node, Edge, NetworkGraph)(graph, singleNodeAsGraph:=True)
                    Dim rebuild As New NetworkGraph

                    For Each v As Node In g.vertex
                        Call rebuild.AddNode(v, assignId:=True)
                    Next
                    For Each link As Edge In g.graphEdges
                        Call rebuild.CreateEdge(
                            u:=rebuild.GetElementByID(link.U.label),
                            v:=rebuild.GetElementByID(link.V.label),
                            weight:=link.weight,
                            data:=link.data
                        )
                    Next

                    Yield rebuild
                Next
            End Function()

        Return pipeline.CreateFromPopulator(list)
    End Function

    <ExportAPI("extractKnowledgeTerms")>
    Public Function extractKnowledgeTerms(island As NetworkGraph, Optional equals As Double = 0.5) As KnowledgeFrameRow()
        Return island.SplitKnowledges(equals).ToArray
    End Function

    <ExportAPI("niceTerms")>
    Public Function knowledgeTable(knowledges As KnowledgeFrameRow(), kb As GraphModel,
                                   <RRawVectorArgument(GetType(String))>
                                   Optional indexBy As Object = Nothing,
                                   Optional prefix As String = "Term") As EntityObject()

        Dim index As String() = DirectCast(REnv.asVector(Of String)(indexBy), String())
        Dim result As EntityObject() = knowledges _
            .Select(Function(row)
                        Return row.CreateNiceTerm(Of EntityObject)(kb)
                    End Function) _
            .ToArray

        For i As Integer = 0 To result.Length - 1
            result(i).ID = $"{prefix}{KnowledgeTerm.UniqueHashCode(result(i), indexBy)}"
        Next

        Return result
    End Function

    <ExportAPI("correctKnowledges")>
    Public Function correctKnowledges(kb As GraphModel,
                                      knowledges As KnowledgeFrameRow(),
                                      <RRawVectorArgument(GetType(String))>
                                      indexBy As Object) As KnowledgeFrameRow()

        Dim index As String() = DirectCast(REnv.asVector(Of String)(indexBy), String())
        Dim result = KnowledgeFrameRow.CorrectKnowledges(kb, knowledges, index).ToArray

        Return result
    End Function

    ''' <summary>
    ''' export knowledge terms based on the network community algorithm
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="common_type">
    ''' all of the type defined from this parameter will be removed from 
    ''' the community algorithm due to the reason of common type always 
    ''' be a hub node in the network, will create a false knowledge community 
    ''' result. example as formula string in chemical data knowledges 
    ''' will groups all Isomer compounds with the same formula string as 
    ''' one identical metabolite.
    ''' </param>
    ''' <returns>
    ''' this function returns a tuple list with two elements inside:
    ''' 
    ''' 1. ``graph`` - is the knowledge network graph data with community 
    '''                tags and trimmed data.
    ''' 2. ``knowledges`` - a table dataset that contains knowledge data 
    '''                     entities that detects from the network graph 
    '''                     community data result.
    ''' </returns>
    <ExportAPI("knowledgeCommunity")>
    Public Function knowledgeCommunity(kb As GraphModel,
                                       <RRawVectorArgument(GetType(String))> indexBy As Object,
                                       <RRawVectorArgument(GetType(String))>
                                       Optional common_type As Object = Nothing,
                                       Optional eps As Double = 0.001,
                                       Optional unweighted As Boolean = False) As list

        Throw New NotImplementedException

        Dim g As NetworkGraph = kb.CreateGraph
        Dim knowledges As KnowledgeFrameRow() = g.ExtractKnowledges(eps).ToArray
        Dim index As String() = DirectCast(REnv.asVector(Of String)(indexBy), String())



        'Dim commons As Index(Of String) = DirectCast(REnv.asVector(Of String)(common_type), String()).Indexing
        'Dim copy As New NetworkGraph

        'If commons.Count > 0 Then
        '    For Each v As Node In g.vertex.ToArray
        '        If Not v.data("knowledge_type") Like commons Then
        '            Call copy.CreateNode(v.label, v.data.Clone)
        '        End If
        '    Next

        '    For Each edge As Edge In g.graphEdges
        '        If (edge.U.data("knowledge_type") Like commons) OrElse (edge.V.data("knowledge_type") Like commons) Then
        '            Continue For
        '        End If

        '        Call copy.CreateEdge(
        '            u:=copy.GetElementByID(edge.U.label),
        '            v:=copy.GetElementByID(edge.V.label),
        '            weight:=edge.weight,
        '            data:=edge.data.Clone
        '        )
        '    Next
        'Else
        '    copy = g
        'End If

        'If unweighted Then
        '    Call Communities.AnalysisUnweighted(copy)
        'Else
        '    Call Communities.Analysis(copy, eps:=eps)
        'End If

        'If commons.Count > 0 Then
        '    For Each v As Node In copy.vertex
        '        g.GetElementByID(v.label).data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
        '    Next
        'End If


        'Dim communityList = g.vertex _
        '    .GroupBy(Function(v)
        '                 Return v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
        '             End Function) _
        '    .ToArray

        'For Each term In communityList
        '    Dim hits As Index(Of String) = term.Select(Function(v) v.label).Indexing
        '    Dim metadata = g.graphEdges _
        '        .Where(Function(url)
        '                   Return url.U.label Like hits OrElse url.V.label Like hits
        '               End Function) _
        '        .Select(Function(url) {url.U, url.V}) _
        '        .IteratesALL _
        '        .GroupBy(Function(v) v.label) _
        '        .Select(Function(v) v.First) _
        '        .GroupBy(Function(v)
        '                     Return v.data("knowledge_type")
        '                 End Function) _
        '        .ToArray
        '    Dim props As New Dictionary(Of String, String)

        '    For Each p In metadata
        '        Call props.Add(p.Key, p.Select(Function(v) v.label).JoinBy("; "))
        '    Next

        '    'Call knowledges.Add(New EntityObject With {
        '    '    .ID = term.Key,
        '    '    .Properties = props
        '    '})
        'Next

        'Dim rtvl As New list With {
        '    .slots = New Dictionary(Of String, Object) From {
        '        {"graph", g},
        '        {"knowledges", unique.castTable},
        '        {"raw", knowledges.castTable}
        '    }
        '}

        'Return rtvl
    End Function

    <Extension>
    Private Function castTable(data As IEnumerable(Of KnowledgeFrameRow)) As EntityObject()
        Return data _
            .Select(Function(i)
                        Return New EntityObject With {
                            .ID = i.UniqeId,
                            .Properties = i.Properties _
                                .ToDictionary(Function(a) a.Key,
                                              Function(a)
                                                  Return a.Value.JoinBy("; ")
                                              End Function)
                        }
                    End Function) _
            .ToArray
    End Function
End Module
