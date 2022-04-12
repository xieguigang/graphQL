Imports System.Data
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Module CreateNetwork

    <Extension>
    Public Function LoadNodeTable(vertexList As IEnumerable(Of Knowledge),
                                  Optional filters As IEnumerable(Of String) = Nothing) As Dictionary(Of String, Node)
        Dim node As Node
        Dim nodeTable As New Dictionary(Of String, Node)
        Dim filterIndex As Index(Of String) = If(filters Is Nothing, New String() {}.Indexing, filters.Indexing)

        For Each knowledge As Knowledge In vertexList
            If filters IsNot Nothing AndAlso knowledge.type Like filterIndex Then
                Continue For
            End If

            node = New Node With {
                .ID = knowledge.ID,
                .label = knowledge.label,
                .data = New NodeData With {
                    .label = knowledge.label,
                    .origID = knowledge.label,
                    .mass = knowledge.mentions,
                    .Properties = New Dictionary(Of String, String) From {
                        {"knowledge_type", knowledge.type},
                        {"source", knowledge.source.JoinBy("; ")}
                    }
                }
            }

            Call nodeTable.Add(node.label, node)
        Next

        Return nodeTable
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="links"></param>
    ''' <param name="nodeTable"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 因为存在节点类型filter，所以<paramref name="nodeTable"/>与<paramref name="links"/>的数据会出现不一致的问题
    ''' 在这个函数中会主动跳过不一致的数据
    ''' </remarks>
    <Extension>
    Public Iterator Function AssembleLinks(links As IEnumerable(Of Association),
                                           nodeTable As Dictionary(Of String, Node),
                                           Optional skipInconsist As Boolean = True) As IEnumerable(Of Edge)

        For Each url As Association In links
            If Not (nodeTable.ContainsKey(url.U.label) AndAlso nodeTable.ContainsKey(url.V.label)) Then
                If skipInconsist Then
                    ' 主动跳过不一致的数据
                    Continue For
                Else
                    Throw New MissingPrimaryKeyException($"missing '{url.U.label}' or '{url.V.label}' in your graph!")
                End If
            End If

            Yield New Edge With {
                .U = nodeTable(url.U.label),
                .V = nodeTable(url.V.label),
                .weight = url.weight,
                .isDirected = True,
                .data = New EdgeData With {
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, url.type},
                        {"source", url.source.JoinBy("; ")}
                    }
                }
            }
        Next
    End Function

    <Extension>
    Public Function CreateGraph(kb As GraphPool, Optional filters As IEnumerable(Of String) = Nothing) As NetworkGraph
        Dim nodeTable As Dictionary(Of String, Node) = kb.vertex.LoadNodeTable(filters)
        Dim g As New NetworkGraph

        For Each node As Node In nodeTable.Values
            Call g.AddNode(node, assignId:=False)
        Next

        For Each link As Edge In kb.graphEdges.AssembleLinks(nodeTable, skipInconsist:=True)
            Call g.AddEdge(link)
        Next

        Return g
    End Function
End Module
