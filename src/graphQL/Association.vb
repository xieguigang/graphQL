Imports Microsoft.VisualBasic.Data.GraphTheory

Public Class Association : Inherits Edge(Of Knowledge)

    ''' <summary>
    ''' the meta data key name
    ''' </summary>
    ''' <returns></returns>
    Public Property type As String

    ''' <summary>
    ''' the data source of this knowledge term 
    ''' association data.
    ''' </summary>
    ''' <returns></returns>
    Public Property source As New List(Of String)

End Class