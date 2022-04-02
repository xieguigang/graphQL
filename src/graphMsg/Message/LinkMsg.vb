Imports graphQL
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Message

    Public Class LinkMsg

        <MessagePackMember(0)> Public Property u As Integer
        <MessagePackMember(1)> Public Property v As Integer
        <MessagePackMember(2)> Public Property weight As Double
        <MessagePackMember(3)> Public Property type As Integer
        <MessagePackMember(4)> Public Property referenceSources As Integer()

        Public Overrides Function ToString() As String
            Return $"[{u}->{v}] {type}"
        End Function

        Public Shared Iterator Function GetRelationships(kb As GraphPool, Optional ref As IndexByRef = Nothing) As IEnumerable(Of LinkMsg)
            Dim allTypes As Index(Of String) = kb.graphEdges _
                .Select(Function(i) i.type) _
                .Distinct _
                .Indexing
            Dim allSources = kb.graphEdges _
                .Select(Function(i) i.source) _
                .IteratesALL _
                .Distinct _
                .Indexing

            If Not ref Is Nothing Then
                ref.types = allTypes.Objects
                ref.source = allTypes.Objects
            End If

            For Each link As Association In kb.graphEdges
                Yield New LinkMsg With {
                    .type = allTypes.IndexOf(link.type),
                    .u = link.U.ID,
                    .v = link.V.ID,
                    .weight = link.weight,
                    .referenceSources = allSources.IndexOf(link.source.Distinct)
                }
            Next
        End Function
    End Class
End Namespace