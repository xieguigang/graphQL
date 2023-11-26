Imports System.Runtime.CompilerServices
Imports graph.MySQL.graphdb
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
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

    Public Sub ReferenceToTerm(g As NetworkGraph, term As UInteger)
        For Each block In g.vertex.Split(1000)
            Call knowledge _
                .where(field("id").in(block.Select(Function(a) a.ID))) _
                .save(field("knowledge_term") = term)
        Next
    End Sub

    Public Function PullGraphById(vocabulary As String(), uid As UInteger, Optional ByRef seed As knowledge = Nothing) As NetworkGraph
        seed = knowledge.where(knowledge.field("id") = uid).find(Of knowledge)

        If seed Is Nothing Then
            Return Nothing
        Else
            Return PullNextGraphInternal(vocabulary, seed)
        End If
    End Function

    Public Function PullNextGraph(vocabulary As String(), Optional ByRef seed As knowledge = Nothing) As NetworkGraph
        seed = knowledge _
            .where(knowledge.field("knowledge_term") = 0) _
            .order_by({"graph_size"}, desc:=True) _
            .find(Of knowledge)

        If seed Is Nothing Then
            Return Nothing
        Else
            Return PullNextGraphInternal(vocabulary, seed)
        End If
    End Function

    Private Function PullNextGraphInternal(vocabulary As String(), Optional ByRef seed As knowledge = Nothing) As NetworkGraph
        Dim node_types As String() = vocabulary _
            .Select(Function(si) si.ToLower) _
            .Where(Function(si) vocabularyIndex.ContainsKey(si)) _
            .Select(Function(l) vocabularyIndex(l).ToString) _
            .Distinct _
            .ToArray
        Dim pull As New List(Of knowledge)(push(seed, node_types))
        Dim linksTo As New List(Of link)

        For Each ki As knowledge In pull
            Call linksTo.AddRange(loadViaFromNodes(seed.id, Nothing, Nothing))
            Call linksTo.AddRange(loadViaToNodes(seed.id, Nothing, Nothing))
        Next

        Call pull.AddRange(pullNodes(linksTo.ToArray))
        ' Call pull.Sort(Function(a, b) a.key.CompareTo(b.key))

        Dim g As New NetworkGraph

        For Each node As knowledge In pull _
            .GroupBy(Function(a) $"{a.key}+{a.node_type}") _
            .Select(Function(gi) gi.First)

            Call addNode(g, node)
        Next
        For Each link As link In linksTo _
            .GroupBy(Function(a) {a.id, a.seed}.OrderBy(Function(id) id).JoinBy("+")) _
            .Select(Function(d) d.First)

            Call g.CreateEdge(
                g.GetElementByID(id:=link.id),
                g.GetElementByID(id:=link.seed),
                weight:=link.weight
            )
        Next

        Return g
    End Function

    Private Sub addNode(g As NetworkGraph, seed As knowledge)
        Dim ctor As New Node With {
            .ID = seed.id,
            .label = seed.key & "@" & toLabel(seed.node_type).vocabulary,
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
    End Sub

    Private Function pullNodes(links As link()) As IEnumerable(Of knowledge)
        If links.IsNullOrEmpty Then
            Return New knowledge() {}
        End If

        Return knowledge _
           .where(knowledge.f("id").in(links.Select(Function(l) l.id).Distinct)) _
           .select(Of knowledge)
    End Function

    Private Function push(seed As knowledge, node_types As String()) As IEnumerable(Of knowledge)
        Dim links As New List(Of link)

        Call links.AddRange(loadViaFromNodes(seed.id, Nothing, node_types))
        Call links.AddRange(loadViaToNodes(seed.id, Nothing, node_types))

        Dim moreSeeds As New List(Of knowledge)(pullNodes(links.ToArray))
        Dim pullSeed = moreSeeds

        Do While True
            Dim excludes As String() = links _
                .Select(Function(l) l.id) _
                .Distinct _
                .Select(Function(i) i.ToString) _
                .ToArray
            Dim a = links.Count
            Dim b = moreSeeds.Count
            Dim pullNodes As New List(Of knowledge)

            For Each seed2 In pullSeed
                Dim pull = loadViaFromNodes(seed2.id, excludes, node_types) _
                    .JoinIterates(loadViaToNodes(seed2.id, excludes, node_types)) _
                    .ToArray
                links.AddRange(pull)
                pullNodes.AddRange(Me.pullNodes(pull))
            Next

            moreSeeds.AddRange(pullNodes)
            pullSeed = pullNodes

            If links.Count = a AndAlso moreSeeds.Count = b Then
                Exit Do
            End If
        Loop

        Call moreSeeds.Add(seed)

        Return moreSeeds
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function loadViaFromNodes(seed As UInteger, excludes As IEnumerable(Of String), node_types As String()) As IEnumerable(Of link)
        Return loadLinks(seed, "from_node", "to_node", excludes.SafeQuery.ToArray, node_types)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function loadViaToNodes(seed As UInteger, excludes As IEnumerable(Of String), node_types As String()) As IEnumerable(Of link)
        Return loadLinks(seed, "to_node", "from_node", excludes.SafeQuery.ToArray, node_types)
    End Function

    Private Function loadLinks(seed As UInteger, field As String, take As String, excludes As String(), node_types As String()) As IEnumerable(Of link)
        Dim sql = graph _
           .left_join("knowledge").on(
                f("knowledge.`id`") = f(take)) _
           .left_join("knowledge_vocabulary").on(
                f("knowledge_vocabulary.`id`") = f("node_type")) _
           .where(graph.f(field) = seed, f("knowledge_term") = 0)

        If Not excludes.IsNullOrEmpty Then
            sql = sql.and(Not f("knowledge.`id`").in(excludes))
        End If
        If Not node_types.IsNullOrEmpty Then
            sql = sql.and(f("knowledge.`node_type`").in(node_types))
        End If

        Dim q = sql.select(Of link)("knowledge.id", $"{field} as seed", "weight", "display_title", "vocabulary AS node_type")

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