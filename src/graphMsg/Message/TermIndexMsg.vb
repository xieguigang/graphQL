Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Namespace Message

    Public Class TermIndexMsg

        <MessagePackMember(0)> Public Property index As TermIndex()

    End Class

    Public Class TermIndex

        <MessagePackMember(0)> Public Property term As String
        <MessagePackMember(1)> Public Property block As String
        <MessagePackMember(2)> Public Property offset As Integer
        <MessagePackMember(3)> Public Property size As Integer

    End Class
End Namespace