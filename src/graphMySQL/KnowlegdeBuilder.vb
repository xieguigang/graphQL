Imports graph.MySQL.graphdb
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Class KnowlegdeBuilder : Inherits graphdbMySQL

    ReadOnly vocabularyIndex As New Dictionary(Of String, UInteger)

    Sub New(graphdb As graphMySQL)
        Call MyBase.New(
            graphdb.graph,
            graphdb.hash_index,
            graphdb.knowledge,
            graphdb.knowledge_vocabulary
        )

        For Each vocabulary As knowledge_vocabulary In knowledge_vocabulary.select(Of knowledge_vocabulary)()
            Call Me.vocabularyIndex.Add(vocabulary.vocabulary.ToLower, vocabulary.id)
        Next
    End Sub

    Public Function PullNextGraph(vocabulary As String()) As NetworkGraph
        Dim seed = knowledge _
            .where(knowledge.field("knowledge_term") = 0) _
            .order_by({"graph_size"}, desc:=True) _
            .find(Of knowledge)

        If seed Is Nothing Then
            Return Nothing
        End If


    End Function

End Class
