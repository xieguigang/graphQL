Imports Microsoft.VisualBasic.Data.GraphTheory

''' <summary>
''' A knowledge node in this graph database
''' </summary>
Public Class Knowledge : Inherits Vertex

    Public Property mentions As Integer
    Public Property type As String

    ''' <summary>
    ''' current knowledge term is a master source term?
    ''' </summary>
    ''' <returns>
    ''' true - is a master source term, 
    ''' false - is a meta data term
    ''' </returns>
    Public Property isMaster As Boolean

    ''' <summary>
    ''' the data source of this knowledge term node
    ''' </summary>
    ''' <returns></returns>
    Public Property source As New List(Of String)

End Class


