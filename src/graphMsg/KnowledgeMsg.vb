Imports graphQL
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Public Class KnowledgeMsg

    <MessagePackMember(0)> Public Property guid As Integer
    <MessagePackMember(1)> Public Property term As String
    <MessagePackMember(2)> Public Property mentions As Integer

    Public Overrides Function ToString() As String
        Return term
    End Function

    Public Shared Iterator Function GetTerms(kb As GraphPool) As IEnumerable(Of KnowledgeMsg)
        For Each v As Knowledge In kb.vertex
            Yield New KnowledgeMsg With {
                .guid = v.ID,
                .term = v.label,
                .mentions = v.Mentions
            }
        Next
    End Function

End Class
