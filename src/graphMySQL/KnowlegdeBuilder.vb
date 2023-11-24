Imports graph.MySQL.graphdb
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Class KnowlegdeBuilder : Inherits graphdbMySQL

    ReadOnly vocabulary As New Dictionary(Of String, UInteger)

    Sub New(graphdb As graphMySQL)
        Call MyBase.New(
            graphdb.graph,
            graphdb.hash_index,
            graphdb.knowledge,
            graphdb.knowledge_vocabulary
        )

        For Each vocabulary As knowledge_vocabulary In knowledge_vocabulary.select(Of knowledge_vocabulary)()
            Call Me.vocabulary.Add(vocabulary.vocabulary.ToLower, vocabulary.id)
        Next
    End Sub

    Public Function PullNextGraph(vocabulary As String()) As NetworkGraph

    End Function

End Class
