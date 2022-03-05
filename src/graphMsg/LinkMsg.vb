Imports graphQL
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Public Class LinkMsg

    <MessagePackMember(0)> Public Property u As Integer
    <MessagePackMember(1)> Public Property v As Integer
    <MessagePackMember(2)> Public Property weight As Double
    <MessagePackMember(3)> Public Property type As String

    Public Shared Iterator Function GetRelationships(kb As GraphPool) As IEnumerable(Of LinkMsg)
        For Each link As Association In kb.graphEdges
            Yield New LinkMsg With {
                .type = link.type,
                .u = link.U.ID,
                .v = link.V.ID,
                .weight = link.weight
            }
        Next
    End Function

End Class
