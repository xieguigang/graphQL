Imports Microsoft.VisualBasic.Serialization.JSON
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

''' <summary>
''' A knowledge link model
''' </summary>
Public Class link

    <DatabaseField("id")> Public Property id As UInteger
    <DatabaseField("seed")> Public Property seed As UInteger
    <DatabaseField("weight")> Public Property weight As Double
    <DatabaseField("display_title")> Public Property display_title As String
    <DatabaseField("node_type")> Public Property node_type As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

End Class