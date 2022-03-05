Imports Microsoft.VisualBasic.Data.GraphTheory

Public Class GraphPool

    ReadOnly engine As New Graph

    Sub New(knowledge As IEnumerable(Of Knowledge), links As IEnumerable(Of VertexEdge))
        For Each kb As Knowledge In knowledge
            Call engine.AddVertex(kb)
        Next
        For Each link As VertexEdge In links
            Call engine.AddEdge(link.U, link.V, link.weight)
        Next
    End Sub

End Class
