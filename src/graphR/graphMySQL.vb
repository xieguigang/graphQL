
Imports graph.MySQL
Imports graph.MySQL.graphdb
Imports graphQL
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Mysqlx.XDevAPI.Relational
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Components.[Interface]
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

        Return cache.add(
            cache.field("seed_id") = seed,
            cache.field("term") = term,
            cache.field("hashcode") = Strings.LCase(term).MD5,
            cache.field("knowledge") = knowledge,
            cache.field("add_time") = Now
        )
    End Function

    <ExportAPI("fetch_json")>
    Public Function fetchAKnowledgeJson(graphdb As KnowlegdeBuilder, id As String, Optional env As Environment = Nothing) As Object
        Dim cache = graphdb.knowledge_cache
        Dim data As knowledge_cache = cache.where(cache.field("id") = id).find(Of knowledge_cache)

        If data Is Nothing Then
            Return data
        End If

        Dim R = env.globalEnvironment.Rscript
        Dim json As Object = R.Invoke("JSON::json_decode", ("str", data.knowledge))

        If TypeOf json Is Message Then
            Return json
        ElseIf json Is Nothing OrElse Not TypeOf json Is list Then
            Return json
        Else
            Dim obj As list = DirectCast(json, list)
            obj.add("knowledge_id", data.id)
            Return obj
        End If
    End Function

    <ExportAPI("fetch_table")>
    Public Function fetch_table(graphdb As KnowlegdeBuilder, <RRawVectorArgument> headers As Object,
                                Optional row_builder As RFunction = Nothing,
                                Optional n As Integer = 100000,
                                Optional env As Environment = Nothing) As Object

        Dim cache = graphdb.knowledge_cache
        Dim R = env.globalEnvironment.Rscript
        Dim names As String() = CLRVector.asCharacter(headers)
        Dim df As New dataframe With {.columns = New Dictionary(Of String, Array)}
        Dim rowNames As New List(Of String)
        Dim cols As New Dictionary(Of String, List(Of String))

        For Each name As String In names
            Call cols.Add(name, New List(Of String))
        Next

        Dim createRow As Func(Of list, [Variant](Of list, Message))

        If row_builder Is Nothing Then
            createRow = Function(list)
                            For Each name As String In names
                                list.slots(name) = CLRVector.asCharacter(list.getByName(name)).JoinBy("; ")
                            Next

                            Return list
                        End Function
        Else
            createRow = Function(list)
                            Dim result = row_builder.Invoke(env, InvokeParameter.CreateLiterals(list))

                            If TypeOf result Is Message Then
                                Return DirectCast(result, Message)
                            Else
                                Return DirectCast(result, list)
                            End If
                        End Function
        End If

        For offset As Integer = 1 To n
            Dim data As knowledge_cache = cache.where(cache.field("id") = offset).find(Of knowledge_cache)

            If data Is Nothing Then
                Continue For
            End If

            Dim json As Object = R.Invoke("JSON::json_decode", ("str", data.knowledge))

            If TypeOf json Is Message Then
                Return json
            ElseIf json Is Nothing OrElse Not TypeOf json Is list Then
                Continue For
            Else
                Dim row = createRow(json)

                If row Like GetType(Message) Then
                    Return row.TryCast(Of Message)
                End If

                Dim jsonList As list = row.TryCast(Of list)

                For Each name As String In names
                    Call cols(name).Add(jsonList.getValue(Of String)(name, env))
                Next

                Call rowNames.Add(data.seed_id)
            End If
        Next

        For Each name As String In names
            Call df.columns.Add(name, cols(name).ToArray)
        Next

        df.rownames = rowNames.ToArray

        Return df
    End Function

End Module
