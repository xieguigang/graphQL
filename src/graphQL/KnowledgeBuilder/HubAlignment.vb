Imports Microsoft.VisualBasic.DataMining.BinaryTree

Public Class HubAlignment : Inherits ComparisonProvider

    ReadOnly matrix As Dictionary(Of String, Dictionary(Of String, Double))

    Public Sub New(matrix As Dictionary(Of String, Dictionary(Of String, Double)), equals As Double, gt As Double)
        MyBase.New(equals, gt)

        Me.matrix = matrix
    End Sub

    Protected Overrides Function GetSimilarity(x As String, y As String) As Double
        Throw New NotImplementedException()
    End Function
End Class
