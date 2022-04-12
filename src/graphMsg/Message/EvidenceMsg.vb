Imports graphQL.Graph
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Message

    Public Class EvidenceMsg

        ''' <summary>
        ''' <see cref="KnowledgeMsg.guid"/>
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> Public Property ref As Integer
        <MessagePackMember(1)> Public Property data As ReferenceData()

        Public Shared Iterator Function CreateEvidencePack(kb As GraphModel, Optional ref As IndexByRef = Nothing) As IEnumerable(Of EvidenceMsg)
            Dim allTypes As Index(Of String) = kb.vertex _
                .Select(Function(i) i.evidence.Keys) _
                .IteratesALL _
                .Distinct _
                .Indexing

            If Not ref Is Nothing Then
                ref.types = allTypes.Objects
            End If

            For Each v As Knowledge In kb.vertex
                Yield New EvidenceMsg With {
                    .ref = v.ID,
                    .data = v.evidence _
                        .Select(Function(evi)
                                    Return New ReferenceData With {
                                        .ref = allTypes.IndexOf(evi.Key),
                                        .data = evi.Value
                                    }
                                End Function) _
                        .ToArray
                }
            Next
        End Function
    End Class

    Public Class ReferenceData

        Public Property ref As Integer
        Public Property data As String()

    End Class
End Namespace