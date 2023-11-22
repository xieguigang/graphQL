Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder

Public Class graphMySQL

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

    Public Function Add(term As String, metadata As Dictionary(Of String, String()))

    End Function

End Class
