Imports graphQL
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Public Class KnowledgeMsg

    <MessagePackMember(0)> Public Property term As String
    <MessagePackMember(1)> Public Property guid As Integer

    Public Shared Iterator Function GetTerms(kb As GraphPool) As IEnumerable(Of KnowledgeMsg)
        For Each v As Knowledge In kb.vertex
            Yield New KnowledgeMsg With {
                .guid = v.ID,
                .term = v.label
            }
        Next
    End Function

End Class
