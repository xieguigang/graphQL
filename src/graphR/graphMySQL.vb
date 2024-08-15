#Region "Microsoft.VisualBasic::e0fe68a1bf7499ec24f02b4a27cc1d46, src\graphR\graphMySQL.vb"

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

    '   Total Lines: 402
    '    Code Lines: 298
    ' Comment Lines: 38
    '   Blank Lines: 66
    '     File Size: 15.43 KB


    ' Module graphMySQLTool
    ' 
    '     Function: add_term, assignTermId, fetch_table, fetchAKnowledgeJson, getKnowledgeCacheModel
    '               KnowlegdeBuilder, openGraphdb, pullNextGraph, saveKnowledge
    ' 
    ' /********************************************************************************/

#End Region

Imports graph.MySQL
Imports graph.MySQL.graphdb
Imports graphQL
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Components.[Interface]
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("graph_mysql")>
<RTypeExport("graphql", GetType(graph.MySQL.mysql))>
<RTypeExport("pubmed", GetType(graph.MySQL.PubMedKb))>
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

    ''' <summary>
    ''' cast the mysql object as the knowledge builder object
    ''' </summary>
    ''' <param name="graphdb"></param>
    ''' <param name="signature"></param>
    ''' <returns></returns>
    <ExportAPI("as.knowledge_builder")>
    Public Function KnowlegdeBuilder(graphdb As graphMySQL, Optional signature As SignatureVerifycation = Nothing) As KnowlegdeBuilder
        Return New KnowlegdeBuilder(graphdb, signature)
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
        Dim content As String()

        For Each nodeSet As IGrouping(Of String, Node) In g.vertex _
            .GroupBy(Function(vi)
                         Return vi.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
                     End Function)

            content = nodeSet _
                .Select(Function(vi) vi.text) _
                .Distinct _
                .OrderByDescending(Function(si) si.Length) _
                .ToArray
            term(nodeSet.Key) = content
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

    ''' <summary>
    ''' assign the knowledge term id to the knowledge nodes
    ''' </summary>
    ''' <param name="graphdb"></param>
    ''' <param name="g"></param>
    ''' <param name="term"></param>
    ''' <returns></returns>
    <ExportAPI("assign_graph")>
    Public Function assignTermId(graphdb As KnowlegdeBuilder, g As NetworkGraph, term As UInteger)
        Call graphdb.ReferenceToTerm(g, term)
        Return Nothing
    End Function

    ''' <summary>
    ''' save the knowledge data json into database as cache
    ''' </summary>
    ''' <param name="graphdb"></param>
    ''' <param name="seed"></param>
    ''' <param name="term"></param>
    ''' <param name="knowledge"></param>
    ''' <param name="unique_hash">
    ''' the slot key name for get the combination term for generates the hashcode
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("save.knowledge")>
    Public Function saveKnowledge(graphdb As KnowlegdeBuilder,
                                  seed As UInteger,
                                  term As String,
                                  unique_hash As String(),
                                  knowledge As list,
                                  Optional env As Environment = Nothing) As Object

        Dim cache = graphdb.knowledge_cache
        Dim hashcode As UInteger
        Dim uniques = unique_hash

        unique_hash = unique_hash.Select(Function(f) knowledge.getValue(f, env, "")).ToArray
        hashcode = FNV1a.GetHashCode($"{term}+{unique_hash.JoinBy("+")}")

        If term.StringEmpty Then
            If unique_hash.All(Function(si) si.StringEmpty) Then
                Return False
            End If
        End If

        ' check hash inside database
        Dim check As knowledge_cache = cache _
            .where(cache.field("hashcode") = hashcode) _
            .find(Of knowledge_cache)
        Dim note As String

        If Not check Is Nothing Then
conflicts:
            Dim load As Object = RJSON.ParseJSONinternal(check.knowledge,
                                                         raw:=False,
                                                         strict_vector_syntax:=True,
                                                         env:=env)
            If TypeOf load Is Message Then
                Return load
            End If

            Dim loadjson As list = DirectCast(load, list)
            Dim a As Dictionary(Of String, String()) = knowledge.AsGeneric(Of String())(env)
            Dim b As Dictionary(Of String, String()) = loadjson.AsGeneric(Of String())(env)

            For Each field As String In uniques
                If Not a.ContainsKey(field) Then
                    Continue For
                End If
                If Not b.ContainsKey(field) Then
                    Continue For
                End If

                If Not a(field).Intersect(b(field)).Any Then
                    ' different!
                    ' hash conflicts!
                    Call VBDebugger.EchoLine("hash conflicts!")

                    hashcode += 1
                    note = $"unique_hashseed: {term}+{unique_hash.JoinBy("+")} conflicts with seed_{check.seed_id}"

                    check = cache _
                        .where(cache.field("hashcode") = hashcode) _
                        .find(Of knowledge_cache)

                    If check IsNot Nothing Then
                        GoTo conflicts
                    Else
                        ' add new item after resolve hashcode conflict
                        Dim knowledge_json1 As Object = jsonlite.toJSON(knowledge, env)

                        If TypeOf knowledge_json1 Is Message Then
                            Return knowledge_json1
                        End If

                        cache.add(
                            cache.field("seed_id") = seed,
                            cache.field("term") = term,
                            cache.field("hashcode") = hashcode,
                            cache.field("knowledge") = CStr(knowledge_json1),
                            cache.field("add_time") = Now,
                            cache.field("note") = note
                        )

                        Return note
                    End If
                End If
            Next

            ' merge data if the term is the same
            note = check.note
            note = note & " " & $"UNION({seed})"

            For Each key As String In b.Keys.ToArray
                If a.ContainsKey(key) Then
                    ' union data
                    a(key) = a(key) _
                        .JoinIterates(b(key)) _
                        .Distinct _
                        .ToArray
                Else
                    a.Add(key, b(key))
                End If
            Next

            Dim knowledge_json As String = a.GetJson

            Return cache _
                .where(cache.field("id") = check.id) _
                .limit(1) _
                .save(cache.field("knowledge") = knowledge_json,
                      cache.field("note") = note)
        Else
            Dim knowledge_json As Object = jsonlite.toJSON(knowledge, env)

            If TypeOf knowledge_json Is Message Then
                Return knowledge_json
            Else
                note = $"unique_hashseed: {term}+{unique_hash.JoinBy("+")}"
            End If

            Return cache.add(
                cache.field("seed_id") = seed,
                cache.field("term") = term,
                cache.field("hashcode") = hashcode,
                cache.field("knowledge") = CStr(knowledge_json),
                cache.field("add_time") = Now,
                cache.field("note") = note
            )
        End If
    End Function

    <ExportAPI("fetch_json")>
    Public Function fetchAKnowledgeJson(graphdb As Object, id As String, Optional env As Environment = Nothing) As Object
        Dim cacheModel = getKnowledgeCacheModel(graphdb, env)

        If cacheModel Like GetType(Message) Then
            Return cacheModel.TryCast(Of Message)
        End If

        Dim cache As MySqlBuilder.Model = cacheModel.TryCast(Of MySqlBuilder.Model)
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

    Private Function getKnowledgeCacheModel(graphdb As Object, env As Environment) As [Variant](Of Message, MySqlBuilder.Model)
        If TypeOf graphdb Is KnowlegdeBuilder Then
            Return DirectCast(graphdb, KnowlegdeBuilder).knowledge_cache
        ElseIf TypeOf graphdb Is Global.graph.MySQL.mysql Then
            Return DirectCast(graphdb, Global.graph.MySQL.mysql).knowledge_cache
        Else
            Return Message.InCompatibleType(GetType(KnowlegdeBuilder), graphdb.GetType, env)
        End If
    End Function

    <ExportAPI("fetch_table")>
    Public Function fetch_table(graphdb As Object, <RRawVectorArgument> headers As Object,
                                Optional row_builder As RFunction = Nothing,
                                Optional n As Integer = 100000,
                                Optional env As Environment = Nothing) As Object

        Dim cacheModel = getKnowledgeCacheModel(graphdb, env)

        If cacheModel Like GetType(Message) Then
            Return cacheModel.TryCast(Of Message)
        End If

        Dim cache As MySqlBuilder.Model = cacheModel.TryCast(Of MySqlBuilder.Model)
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
