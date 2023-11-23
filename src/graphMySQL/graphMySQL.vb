Imports System.Drawing
Imports graph.MySQL.mysql
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Imaging
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri

Public Class graphMySQL

    Public ReadOnly Property graph As Model
    Public ReadOnly Property hash_index As Model
    Public ReadOnly Property knowledge As Model
    Public ReadOnly Property knowledge_vocabulary As Model

    ReadOnly vocabulary_cache As New Dictionary(Of String, UInteger)

    Sub New(uri As ConnectionUri)
        graph = New Model("graph", uri)
        hash_index = New Model("hash_index", uri)
        knowledge = New Model("knowledge", uri)
        knowledge_vocabulary = New Model("knowledge_vocabulary", uri)

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

    Public Function Add(term As String, type As String, metadata As Dictionary(Of String, String())) As UInteger
        Dim hashcode As UInteger = FNV1a.GetHashCode($"{term}+{type}")
        Dim nhits As Integer

        If knowledge.where(field("id") = hashcode).find(Of knowledge) Is Nothing Then
            Call knowledge.add(
                field("id") = hashcode,
                field("key") = term,
                field("display_title") = term,
                field("node_type") = Vocabulary(type),
                field("graph_size") = 0,
                field("add_time") = Now,
                field("description") = ""
            )
        End If

        knowledge_vocabulary _
            .where(field("id") = Vocabulary(type)) _
            .limit(1) _
            .save(field("count") = "~ count + 1")

        For Each category As String In metadata.Keys
            Dim desc As String = $"{term}.{category}"
            Dim n As Integer = 0

            For Each val As String In metadata(category)
                If val.StringEmpty(testEmptyFactor:=True) OrElse val = "-" Then
                    Continue For
                End If

                Dim hash2 As UInteger = FNV1a.GetHashCode($"{val}+{category}")

                If knowledge.where(field("id") = hash2).find(Of knowledge) Is Nothing Then
                    knowledge.add(
                        field("id") = hash2,
                        field("key") = val,
                        field("display_title") = val,
                        field("node_type") = Vocabulary(category),
                        field("graph_size") = 1,
                        field("add_time") = Now,
                        field("description") = desc
                    )
                End If

                If graph.where(field("from_node") = hash2, field("to_node") = hashcode).find(Of mysql.graph) Is Nothing Then
                    graph.add(
                        field("from_node") = hash2,
                        field("to_node") = hashcode,
                        field("link_type") = Vocabulary(category),
                        field("weight") = 1,
                        field("add_time") = Now,
                        field("note") = ""
                    )
                End If

                n += 1
                nhits += 1
            Next

            knowledge_vocabulary _
                .where(field("id") = Vocabulary(category)) _
                .limit(1) _
                .save(field("count") = $"~ count + {n}")
        Next

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
