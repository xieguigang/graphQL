Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Namespace Message

    Public Class EvidenceMsg

        ''' <summary>
        ''' <see cref="KnowledgeMsg.guid"/>
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> Public Property ref As Integer
        <MessagePackMember(1)> Public Property data As ReferenceData()

    End Class

    Public Class ReferenceData

        Public Property ref As Integer
        Public Property data As String()

    End Class
End Namespace