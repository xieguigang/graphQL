Imports System.Runtime.CompilerServices
Imports graph.MySQL.graphdb
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder

Public Class KnowlegdeBuilder : Inherits graphdbMySQL

    ReadOnly vocabularyIndex As New Dictionary(Of String, UInteger)
    ReadOnly toLabel As New Dictionary(Of String, knowledge_vocabulary)

    Public ReadOnly Property knowledge_cache As Model

    Sub New(graphdb As graphMySQL)
        Call MyBase.New(
            graphdb.graph,
            graphdb.hash_index,
            graphdb.knowledge,
            graphdb.knowledge_vocabulary
        )

        knowledge_cache = New Model("knowledge_cache", graph.mysqli)

        For Each vocabulary As knowledge_vocabulary In knowledge_vocabulary.select(Of knowledge_vocabulary)()
            Call Me.vocabularyIndex.Add(vocabulary.vocabulary.ToLower, vocabulary.id)
            Call Me.toLabel.Add(vocabulary.id, vocabulary)
        Next
    End Sub

    ''' <summary>
    ''' assign the knowledge term id to the knowledge nodes
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="term"></param>
    ''' <remarks>
    ''' this function just assign the knowledge term id to the 
    ''' link node type, see about link node: 
    ''' <see cref="addNode(NetworkGraph, graphdb.knowledge, Index(Of String))"/>
    ''' </remarks>
    Public Sub ReferenceToTerm(g As NetworkGraph, term As UInteger)
        Dim links = g.vertex _
            .Where(Function(vi)
                       If vi.pinned Then
                           Return True
                       Else
                           Return vi.data("dataNode").TextEquals("false")
                       End If
                   End Function) _
            .ToArray

        For Each block As Node() In links.Split(100)
            Call knowledge _
                .where(field("id").in(block.Select(Function(a) a.ID))) _
                .limit(block.Length) _
                .save(field("knowledge_term") = term)
        Next
    End Sub

    ''' <summary>
    ''' pull a knowledge graph from a specific knowledge node
    ''' </summary>
    ''' <param name="vocabulary"></param>
    ''' <param name="uid"></param>
    ''' <param name="seed"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' usually used for run debug
    ''' </remarks>
    Public Function PullGraphById(vocabulary As String(), uid As UInteger, Optional ByRef seed As knowledge = Nothing) As NetworkGraph
        ' from a specific node
        seed = knowledge.where(knowledge.field("id") = uid).find(Of knowledge)

        If seed Is Nothing Then
            Return Nothing
        Else
            Return PullNextGraphInternal(vocabulary:=mapNodeTypes(vocabulary), seed)
        End If
    End Function

    Private Function mapNodeTypes(vocabulary As String()) As String()
        Return vocabulary _
            .Select(Function(si) si.ToLower) _
            .Where(Function(si) vocabularyIndex.ContainsKey(si)) _
            .Select(Function(l) vocabularyIndex(l).ToString) _
            .Distinct _
            .ToArray
    End Function

    ''' <summary>
    ''' pull a knowledge graph from a un-assigned term
    ''' </summary>
    ''' <param name="vocabulary"></param>
    ''' <param name="seed"></param>
    ''' <returns></returns>
    Public Function PullNextGraph(vocabulary As String(), Optional ByRef seed As knowledge = Nothing) As NetworkGraph
        Dim node_types As String() = mapNodeTypes(vocabulary)

        ' from a un-assigned node
        seed = knowledge _
            .where(knowledge.field("knowledge_term") = 0, knowledge.field("node_type").in(node_types)) _
            .order_by({"graph_size"}, desc:=True) _
            .find(Of knowledge)

        If seed Is Nothing Then
            Return Nothing
        Else
            Return PullNextGraphInternal(vocabulary:=node_types, seed)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="vocabulary">
    ''' A collection of the integer id
    ''' </param>
    ''' <param name="seed"></param>
    ''' <returns></returns>
    Private Function PullNextGraphInternal(vocabulary As String(), ByRef seed As knowledge) As NetworkGraph
        ' properties -> seed
        ' from_node -> to_node
        ' the seed is to_node always
        Dim g As New NetworkGraph
        Dim linkTypes As Index(Of String) = vocabulary.Indexing
        Dim knowledgeCache As New Dictionary(Of String, knowledge)
        Dim excludes As New Index(Of String)

        Call pullNextGraph(g, linkTypes, seed, knowledgeCache)
        Call excludes.Add(seed.id)

        Dim nsize As Integer = g.size.vertex

        For i As Integer = 0 To 1000000
            ' loop throught each link node
            For Each node As Node In g.pinnedNodes
                If Not node.ID.ToString Like excludes Then
                    Call pullNextGraph(g, linkTypes, knowledgeCache(node.ID.ToString), knowledgeCache)
                    Call excludes.Add(node.ID)
                End If
            Next

            If nsize = g.size.vertex Then
                Exit For
            Else
                nsize = g.size.vertex
            End If
        Next

        Return g
    End Function

    Private Sub pullNextGraph(g As NetworkGraph, linkTypes As Index(Of String), seed As knowledge, knowledgeCache As Dictionary(Of String, knowledge))
        ' load current node
        Call addNode(g, seed, linkTypes)

        For Each link As graphdb.graph In graph _
            .where(field("to_node") = seed.id) _
            .select(Of graphdb.graph)

            Dim propertyVal As knowledge = knowledgeCache.ComputeIfAbsent(
                key:=link.from_node,
                lazyValue:=AddressOf getNodeById
            )

            If Not propertyVal Is Nothing Then
                Call addNode(g, propertyVal, linkTypes)
                Call g.CreateEdge(
                    g.GetElementByID(id:=propertyVal.id),
                    g.GetElementByID(id:=seed.id),
                    weight:=link.weight
                )
            End If
        Next
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function getNodeById(id As String) As knowledge
        Return knowledge.where(field("id") = id).find(Of knowledge)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="seed"></param>
    ''' <param name="linkIndex">
    ''' there are two kind of graph node in the database:
    ''' 
    ''' 1. link node: node used for link the terms between different database, 
    '''    this kind of node will be assigned a knowledge term id
    ''' 2. data node: just used for save the data information, such kind of the 
    '''    data node may be common and overlaps between many knowledge terms, 
    '''    example as chemical formula information. this kind of node will not
    '''    be assigned a knowledge term
    ''' </param>
    ''' <remarks>
    ''' this function used two data slot for mark the data node type or the link node type:
    ''' 
    ''' 1. <see cref="Node.pinned"/>: true means current node is a link node, false means current node is a data node
    ''' 2. <see cref="NodeData.Properties"/>: dataNode is true means current node is a data 
    '''    node, otherwise means current node is a link node.
    ''' </remarks>
    Private Sub addNode(g As NetworkGraph, seed As knowledge, linkIndex As Index(Of String))
        Dim node_type As String = seed.node_type.ToString
        Dim is_link As Boolean = node_type Like linkIndex
        Dim key As String = seed.key & "@" & toLabel(node_type).vocabulary

        If g.GetElementByID(key) Is Nothing Then
            Dim ctor As New Node With {
                .ID = seed.id,
                .label = key,
                .data = New NodeData With {
                    .label = seed.display_title,
                    .Properties = New Dictionary(Of String, String) From {
                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, toLabel(node_type).vocabulary.ToLower},
                        {"dataNode", (Not is_link).ToString.ToLower}
                    },
                    .origID = seed.key,
                    .size = {seed.graph_size + 1},
                    .mass = seed.graph_size,
                    .weights = .size,
                    .color = toLabel(node_type).color.GetBrush
                },
                .pinned = is_link,
                .visited = .pinned
            }

            Call g.AddNode(ctor, assignId:=False)
        End If
    End Sub

    Private Iterator Function pullNodes(links As link()) As IEnumerable(Of knowledge)
        If links.IsNullOrEmpty Then
            Return
        End If

        For Each part As UInteger() In links.Select(Function(l) l.id).Distinct.Split(300)
            For Each k As knowledge In knowledge _
               .where(knowledge.f("id").in(part)) _
               .limit(part.Length) _
               .select(Of knowledge)

                Yield k
            Next
        Next
    End Function

    ''' <summary>
    ''' only pull the link node from database for create knowledge seeds
    ''' </summary>
    ''' <param name="seed"></param>
    ''' <param name="node_types"></param>
    ''' <returns></returns>
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

            For Each seed2 As knowledge In pullSeed
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
