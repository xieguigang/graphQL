Imports graphQL
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Public Class LinkMsg

    <MessagePackMember(0)> Public Property u As Integer
    <MessagePackMember(1)> Public Property v As Integer
    <MessagePackMember(2)> Public Property weight As Double
    <MessagePackMember(3)> Public Property type As Integer

    Public Overrides Function ToString() As String
        Return $"[{u}->{v}] {type}"
    End Function

    Public Shared Iterator Function GetRelationships(kb As GraphPool, Optional types As List(Of String) = Nothing) As IEnumerable(Of LinkMsg)
        Dim allTypes = kb.graphEdges.Select(Function(i) i.type).Distinct.Indexing

        If Not types Is Nothing Then
            Call types.Clear()
            Call types.AddRange(allTypes.Objects)
        End If

        For Each link As Association In kb.graphEdges
            Yield New LinkMsg With {
                .type = allTypes.IndexOf(link.type),
                .u = link.U.ID,
                .v = link.V.ID,
                .weight = link.weight
            }
        Next
    End Function

End Class
