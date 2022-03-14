Imports Microsoft.VisualBasic.Data.GraphTheory

''' <summary>
''' A knowledge node in this graph database
''' </summary>
Public Class Knowledge : Inherits Vertex

    Public Property mentions As Integer
    Public Property type As String

End Class

Public Class Association : Inherits Edge(Of Knowledge)

    ''' <summary>
    ''' the meta data key name
    ''' </summary>
    ''' <returns></returns>
    Public Property type As String

End Class
