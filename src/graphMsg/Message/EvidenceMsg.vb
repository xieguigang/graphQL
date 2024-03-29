﻿Imports graphQL.Graph
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Message

    ''' <summary>
    ''' the association link between the knowledge 
    ''' terms and the evidence terms
    ''' </summary>
    Public Class EvidenceMsg

        ''' <summary>
        ''' <see cref="KnowledgeMsg.guid"/>
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> Public Property ref As Integer
        <MessagePackMember(1)> Public Property data As ReferenceData()

        Public Shared Iterator Function CreateEvidencePack(kb As GraphModel) As IEnumerable(Of EvidenceMsg)
            For Each v As Knowledge In kb.vertex
                Yield New EvidenceMsg With {
                    .ref = v.ID,
                    .data = v.evidence _
                        .Select(Function(evi)
                                    Return New ReferenceData With {
                                        .ref = evi.category,
                                        .data = evi.reference
                                    }
                                End Function) _
                        .ToArray
                }
            Next
        End Function
    End Class

    Public Class ReferenceData

        <MessagePackMember(0)> Public Property ref As Integer
        <MessagePackMember(1)> Public Property data As Integer()

        Public Overrides Function ToString() As String
            Return $"({ref}) {data.GetJson}"
        End Function

    End Class
End Namespace