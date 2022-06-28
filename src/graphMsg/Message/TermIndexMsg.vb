Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Namespace Message

    Public Class TermIndexMsg

        <MessagePackMember(0)> Public Property index As TermIndex()

        Public Function ToList() As Dictionary(Of String, TermIndex)
            Return index.ToDictionary(Function(i) i.term)
        End Function

    End Class

    Public Class TermIndex

        <MessagePackMember(0)> Public Property term As String
        <MessagePackMember(1)> Public Property block As String
        <MessagePackMember(2)> Public Property offset As Integer

    End Class
End Namespace