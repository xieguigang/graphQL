Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Module CreateNetwork

    <Extension>
    Public Function LoadNodeTable(vertexList As IEnumerable(Of Knowledge)) As Dictionary(Of String, Node)
        Dim node As Node
        Dim nodeTable As New Dictionary(Of String, Node)

        For Each knowledge As Knowledge In vertexList
            node = New Node With {
                .ID = knowledge.ID,
                .label = knowledge.label,
                .data = New NodeData With {
                    .label = knowledge.label,
                    .origID = knowledge.label,
                    .mass = knowledge.mentions,
                    .Properties = New Dictionary(Of String, String) From {
                        {"knowledge_type", knowledge.type}
                    }
                }
            }

            Call nodeTable.Add(node.label, node)
        Next

        Return nodeTable
    End Function

    <Extension>
    Public Iterator Function AssembleLinks(links As IEnumerable(Of Association), nodeTable As Dictionary(Of String, Node)) As IEnumerable(Of Edge)
        For Each url As Association In links
            Yield New Edge With {
                .U = nodeTable(url.U.label),
                .V = nodeTable(url.V.label),
                .weight = url.weight,
                .isDirected = True,
                .data = New EdgeData With {
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, url.type}
                    }
                }
            }
        Next
    End Function

    <Extension>
    Public Function CreateGraph(kb As GraphPool) As NetworkGraph
        Dim nodeTable = kb.vertex.LoadNodeTable
        Dim g As New NetworkGraph

        For Each node As Node In nodeTable.Values
            Call g.AddNode(node, assignId:=False)
        Next

        For Each link As Edge In kb.graphEdges.AssembleLinks(nodeTable)
            Call g.AddEdge(link)
        Next

        Return g
    End Function
End Module
