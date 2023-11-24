Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports graph.MySQL.graphdb
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri

Public MustInherit Class graphdbMySQL

    Public ReadOnly Property graph As Model
    Public ReadOnly Property hash_index As Model
    Public ReadOnly Property knowledge As Model
    Public ReadOnly Property knowledge_vocabulary As Model

    Sub New(uri As ConnectionUri)
        graph = New Model("graph", uri)
        hash_index = New Model("hash_index", uri)
        knowledge = New Model("knowledge", uri)
        knowledge_vocabulary = New Model("knowledge_vocabulary", uri)
    End Sub

    Protected Sub New(graph As Model, hash_index As Model, knowledge As Model, knowledge_vocabulary As Model)
        Me.graph = graph
        Me.hash_index = hash_index
        Me.knowledge = knowledge
        Me.knowledge_vocabulary = knowledge_vocabulary
    End Sub

End Class

Public Class graphMySQL : Inherits graphdbMySQL

    ReadOnly vocabulary_cache As New Dictionary(Of String, UInteger)
    ReadOnly empty_str As New Index(Of String)

    Sub New(uri As ConnectionUri)
        Call MyBase.New(uri)

        For Each term As knowledge_vocabulary In knowledge_vocabulary _
            .where("length(`hashcode`) < 32") _
            .select(Of knowledge_vocabulary)

            knowledge_vocabulary _
                .where(field("vocabulary") = term.vocabulary) _
                .save(
                    field("hashcode") = VocabularyHashCode(term.vocabulary)
                )
        Next
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub SetEmptyStringFactor(ls As IEnumerable(Of String))
        Call ls.SafeQuery.DoEach(Sub(s) empty_str.Add(s))
    End Sub

    Private Function getTermHashCode(term As String, type As String, desc As String) As UInteger
        Dim key As String = Strings.LCase(term).MD5
        Dim find As knowledge = knowledge _
            .where(
                field("key") = key,
                field("node_type") = Vocabulary(type)
            ) _
            .find(Of knowledge)

        If find Is Nothing Then
            Call knowledge.add(
                field("key") = key,
                field("display_title") = term,
                field("node_type") = Vocabulary(type),
                field("graph_size") = 0,
                field("add_time") = Now,
                field("description") = desc
            )
        End If

        find = knowledge _
            .where(
                field("key") = key,
                field("node_type") = Vocabulary(type)
            ) _
            .find(Of knowledge)

        If find Is Nothing Then
            Throw New InvalidProgramException($"Can not create knowledge term: {term}@{type}!")
        Else
            Return find.id
        End If
    End Function

    Public Function Add(term As String, type As String, metadata As Dictionary(Of String, String())) As UInteger
        Dim hashcode As UInteger = getTermHashCode(term, type, desc:=type)
        Dim nhits As Integer

        knowledge_vocabulary _
            .where(field("id") = Vocabulary(type)) _
            .limit(1) _
            .save(field("count") = "~ count + 1")

        If metadata Is Nothing Then
            Return hashcode
        End If

        For Each category As String In metadata.Keys
            Dim desc As String = $"{term}.{category}"
            Dim n As Integer = 0
            Dim ls As String() = metadata(category)

            If ls.IsNullOrEmpty Then
                Continue For
            End If

            Dim w As Double = 1 / ls.Length

            For Each val As String In ls
                If val.StringEmpty(testEmptyFactor:=True) OrElse val = "-" OrElse val Like empty_str Then
                    Continue For
                End If

                Dim hash2 As UInteger = getTermHashCode(val, category, desc)

                If graph.where(field("from_node") = hash2, field("to_node") = hashcode).find(Of graphdb.graph) Is Nothing Then
                    graph.add(
                        field("from_node") = hash2,
                        field("to_node") = hashcode,
                        field("link_type") = Vocabulary(category),
                        field("weight") = 1,
                        field("add_time") = Now,
                        field("note") = ""
                    )
                Else
                    graph _
                        .where(
                            field("from_node") = hash2,
                            field("to_node") = hashcode
                        ) _
                        .limit(1) _
                        .save(field("weight") = $"~ weight + {w}")
                End If

                n += 1
                nhits += 1
            Next

            ' update vocabulary reference count
            knowledge_vocabulary _
                .where(field("id") = Vocabulary(category)) _
                .limit(1) _
                .save(field("count") = $"~ count + {n}")
        Next

        ' update knowledge term graph size
        knowledge.where(field("id") = hashcode) _
            .limit(1) _
            .save(field("graph_size") = $"~ graph_size + {nhits}")

        Return hashcode
    End Function

    Public Shared Function VocabularyHashCode(term As String) As String
        Return Strings.LCase(term).MD5
    End Function

    Public Function Vocabulary(term As String) As Integer
        Return vocabulary_cache.ComputeIfAbsent(
            key:=Strings.LCase(term),
            lazyValue:=Function()
                           Dim find = knowledge_vocabulary _
                               .where(field("vocabulary") = term) _
                               .find(Of knowledge_vocabulary)

                           If find Is Nothing Then
                               ' add new
                               knowledge_vocabulary.add(
                                   field("vocabulary") = term,
                                   field("hashcode") = VocabularyHashCode(term),
                                   field("ancestor") = 1,
                                   field("level") = 1,
                                   field("add_time") = Now,
                                   field("color") = Color.Black.ToHtmlColor,
                                   field("count") = 0,
                                   field("description") = ""
                               )
                           End If

                           find = knowledge_vocabulary _
                               .where(field("vocabulary") = term) _
                               .find(Of knowledge_vocabulary)

                           Return find.id
                       End Function)
    End Function

End Class
