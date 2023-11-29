
Imports graph.MySQL
Imports graph.MySQL.graphdb
Imports graphQL
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("graph_mysql")>
Public Module graphMySQLTool

    <ExportAPI("open.graphdb")>
    <RApiReturn(GetType(graphMySQL))>
    Public Function openGraphdb(user_name As String, password As String,
                                Optional host As String = "localhost",
                                Optional port As Integer = 3306,
                                Optional dbname As String = "graphql") As Object

        Return New graphMySQL(uri:=New ConnectionUri() With {
            .Database = dbname,
            .IPAddress = host,
            .Password = password,
            .Port = port,
            .User = user_name
        })
    End Function

    <ExportAPI("as.knowledge_builder")>
    Public Function KnowlegdeBuilder(graphdb As graphMySQL) As KnowlegdeBuilder
        Return New KnowlegdeBuilder(graphdb)
    End Function

    <ExportAPI("add_term")>
    Public Function add_term(graphdb As graphMySQL,
                             term As String,
                             Optional category As String = "unclass",
                             <RListObjectArgument>
                             Optional metadata As list = Nothing,
                             Optional env As Environment = Nothing) As Object

        If metadata.length = 1 AndAlso TypeOf metadata.data.First Is list Then
            metadata = metadata.data.First
        End If

        Dim meta_str As Dictionary(Of String, String()) = metadata.AsGeneric(Of String())(env)
        Dim result = graphdb.Add(term, category, meta_str)

        Return result
    End Function

    ''' <summary>
    ''' pull a knowledge graph
    ''' </summary>
    ''' <param name="graphdb"></param>
    ''' <param name="vocabulary"></param>
    ''' <param name="id">used for debug test</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("pull_nextGraph")>
    <RApiReturn("graph", "term", "seed", "id")>
    Public Function pullNextGraph(graphdb As KnowlegdeBuilder, <RRawVectorArgument> vocabulary As Object,
                                  Optional id As UInteger? = Nothing,
                                  Optional env As Environment = Nothing) As Object

        Dim cats = CLRVector.asCharacter(vocabulary)
        Dim seed As knowledge = Nothing
        Dim g As NetworkGraph

        If id Is Nothing Then
            g = graphdb.PullNextGraph(cats, seed)
        Else
            g = graphdb.PullGraphById(cats, id, seed)
        End If

        If seed Is Nothing OrElse g Is Nothing Then
            Return Nothing
        End If

        Dim term As New KnowledgeFrameRow With {
            .Properties = New Dictionary(Of String, String()),
            .UniqeId = seed.id
        }

        For Each nodeSet In g.vertex _
            .GroupBy(Function(vi)
                         Return vi.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
                     End Function)

            Call term.Add(nodeSet.Key, nodeSet.Select(Function(vi) vi.text).Distinct.ToArray)
        Next

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"graph", g},
                {"term", term},
                {"seed", seed},
                {"id", seed.id}
            }
        }
    End Function

    <ExportAPI("assign_graph")>
    Public Function assignTermId(graphdb As KnowlegdeBuilder, g As NetworkGraph, term As UInteger)
        Call graphdb.ReferenceToTerm(g, term)
        Return Nothing
    End Function

    <ExportAPI("save.knowledge")>
    Public Function saveKnowledge(graphdb As KnowlegdeBuilder, seed As UInteger, term As String, knowledge As String) As Object
        Dim cache = graphdb.knowledge_cache

        Return cache.save(
            cache.field("seed_id") = seed,
            cache.field("term") = term,
            cache.field("hashcode") = Strings.LCase(term).MD5,
            cache.field("knowledge") = knowledge,
            cache.field("add_time") = Now
        )
    End Function

End Module
