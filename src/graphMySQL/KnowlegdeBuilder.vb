Imports System.Runtime.CompilerServices
Imports graph.MySQL.graphdb
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

Public Class KnowlegdeBuilder : Inherits graphdbMySQL

    ReadOnly vocabularyIndex As New Dictionary(Of String, UInteger)
    ReadOnly toLabel As New Dictionary(Of String, knowledge_vocabulary)

    Sub New(graphdb As graphMySQL)
        Call MyBase.New(
            graphdb.graph,
            graphdb.hash_index,
            graphdb.knowledge,
            graphdb.knowledge_vocabulary
        )

        For Each vocabulary As knowledge_vocabulary In knowledge_vocabulary.select(Of knowledge_vocabulary)()
            Call Me.vocabularyIndex.Add(vocabulary.vocabulary.ToLower, vocabulary.id)
            Call Me.toLabel.Add(vocabulary.id, vocabulary)
        Next
    End Sub

    Public Function PullNextGraph(vocabulary As String()) As NetworkGraph
        Dim seed = knowledge _
            .where(knowledge.field("knowledge_term") = 0) _
            .order_by({"graph_size"}, desc:=True) _
            .find(Of knowledge)

        If seed Is Nothing Then
            Return Nothing
        End If

        Dim g As New NetworkGraph
        Dim pull As New List(Of link)(push(g, seed))

        Return g
    End Function

    Private Iterator Function push(g As NetworkGraph, seed As knowledge) As IEnumerable(Of link)
        If g.GetElementByID(id:=seed.id) Is Nothing Then
            Dim ctor As New Node With {
                .ID = seed.id,
                .label = seed.key,
                .data = New NodeData With {
                    .label = seed.display_title,
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, toLabel(seed.node_type).vocabulary.ToLower}
                    },
                    .origID = seed.key,
                    .size = {seed.graph_size + 1},
                    .mass = seed.graph_size,
                    .weights = .size,
                    .color = toLabel(seed.node_type).color.GetBrush
                }
            }

            Call g.AddNode(ctor, assignId:=False)
        End If

        For Each link As link In loadViaFromNodes(seed.id)
            If g.GetElementByID(id:=link.id) Is Nothing Then
                For Each pull In push(g, knowledge.where(f("id") = link.id).find(Of knowledge))
                    Yield pull
                Next
            End If

            If seed.id <> link.id Then
                ' create link
                g.CreateEdge(
                    g.GetElementByID(id:=seed.id),
                    g.GetElementByID(id:=link.id),
                    weight:=link.weight
                )

                Yield link
            End If
        Next
        For Each link As link In loadViaToNodes(seed.id)
            If g.GetElementByID(id:=link.id) Is Nothing Then
                For Each pull In push(g, knowledge.where(f("id") = link.id).find(Of knowledge))
                    Yield pull
                Next
            End If

            If seed.id <> link.id Then
                ' create link
                g.CreateEdge(
                    g.GetElementByID(id:=seed.id),
                    g.GetElementByID(id:=link.id),
                    weight:=link.weight
                )

                Yield link
            End If
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function loadViaFromNodes(seed As UInteger) As IEnumerable(Of link)
        Return loadLinks(seed, "from_node")
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function loadViaToNodes(seed As UInteger) As IEnumerable(Of link)
        Return loadLinks(seed, "to_node")
    End Function

    Private Function loadLinks(seed As UInteger, field As String) As IEnumerable(Of link)
        Dim q = graph _
           .left_join("knowledge").on(
                f("knowledge.`id`") = f("from_node")) _
           .left_join("knowledge_vocabulary").on(
                f("knowledge_vocabulary.`id`") = f("node_type")) _
           .where(graph.f(field) = seed, f("knowledge_term") = 0) _
           .select(Of link)("knowledge.id", $"{field} as seed", "weight", "display_title", "vocabulary AS node_type")

        Return q
    End Function

End Class

Public Class link

    <DatabaseField("id")> Public Property id As UInteger
    <DatabaseField("seed")> Public Property seed As UInteger
    <DatabaseField("weight")> Public Property weight As Double
    <DatabaseField("display_title")> Public Property display_title As String
    <DatabaseField("node_type")> Public Property node_type As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

End Class