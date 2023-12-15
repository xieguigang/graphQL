Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri

Public MustInherit Class graphdbMySQL : Inherits IDatabase

    Public ReadOnly Property graph As Model
    Public ReadOnly Property hash_index As Model
    Public ReadOnly Property knowledge As Model
    Public ReadOnly Property knowledge_vocabulary As Model

    Sub New(uri As ConnectionUri)
        Call MyBase.New(uri)

        graph = New Model("graph", uri)
        hash_index = New Model("hash_index", uri)
        knowledge = New Model("knowledge", uri)
        knowledge_vocabulary = New Model("knowledge_vocabulary", uri)
    End Sub

    Protected Sub New(graph As Model, hash_index As Model, knowledge As Model, knowledge_vocabulary As Model)
        Call MyBase.New(Nothing)

        Me.graph = graph
        Me.hash_index = hash_index
        Me.knowledge = knowledge
        Me.knowledge_vocabulary = knowledge_vocabulary
    End Sub

End Class