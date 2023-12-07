Imports Microsoft.VisualBasic.DataMining.BinaryTree

Public Class HubAlignment : Inherits ComparisonProvider

    ReadOnly matrix As Dictionary(Of String, Dictionary(Of String, Double))

    Public Sub New(matrix As Dictionary(Of String, Dictionary(Of String, Double)), equals As Double, gt As Double)
        MyBase.New(equals, gt)

        Me.matrix = matrix
    End Sub

    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Return matrix(x)(y)
    End Function

    Public Overrides Function GetObject(id As String) As Object
        Return matrix(id)
    End Function
End Class
