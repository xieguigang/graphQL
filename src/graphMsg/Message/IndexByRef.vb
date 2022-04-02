Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Message

    Public Class IndexByRef

        <MessagePackMember(0)> Public Property types As String()
        <MessagePackMember(1)> Public Property source As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace