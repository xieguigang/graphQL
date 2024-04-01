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

        graph = model(Of graphdb.graph)()
        hash_index = model(Of graphdb.hash_index)()
        knowledge = model(Of graphdb.knowledge)()
        knowledge_vocabulary = model(Of graphdb.knowledge_vocabulary)()
    End Sub

    Protected Sub New(graph As Model, hash_index As Model, knowledge As Model, knowledge_vocabulary As Model)
        Call MyBase.New(Nothing)

        Me.graph = graph
        Me.hash_index = hash_index
        Me.knowledge = knowledge
        Me.knowledge_vocabulary = knowledge_vocabulary
    End Sub

End Class