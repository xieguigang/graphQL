Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Module CreateNetwork

    <Extension>
    Public Function CreateGraph(kb As GraphPool) As NetworkGraph
        Dim g As New NetworkGraph
        Dim node As Node
        Dim link As Edge
        Dim nodeTable As New Dictionary(Of String, Node)

        For Each knowledge As Knowledge In kb.vertex
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
            Call g.AddNode(node, assignId:=False)
        Next

        For Each url As Association In kb.graphEdges
            link = New Edge With {
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

            Call g.AddEdge(link)
        Next

        Return g
    End Function
End Module
