Imports graphQL
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Public Class KnowledgeMsg

    <MessagePackMember(0)> Public Property guid As Integer
    <MessagePackMember(1)> Public Property term As String
    <MessagePackMember(2)> Public Property mentions As Integer
    <MessagePackMember(3)> Public Property type As Integer
    <MessagePackMember(4)> Public Property isMaster As Boolean

    Public Overrides Function ToString() As String
        Return term
    End Function

    Public Shared Iterator Function GetTerms(kb As GraphPool, Optional types As List(Of String) = Nothing) As IEnumerable(Of KnowledgeMsg)
        Dim allTypes = kb.vertex.Select(Function(i) i.type).Distinct.Indexing

        If Not types Is Nothing Then
            Call types.Clear()
            Call types.AddRange(allTypes.Objects)
        End If

        For Each v As Knowledge In kb.vertex
            Yield New KnowledgeMsg With {
                .guid = v.ID,
                .term = v.label,
                .mentions = v.mentions,
                .type = allTypes.IndexOf(v.type),
                .isMaster = v.isMaster
            }
        Next
    End Function

End Class
