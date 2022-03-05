Imports Microsoft.VisualBasic.Data.GraphTheory

Public Class GraphPool : Inherits Graph(Of Knowledge, Association)

    Sub New(knowledge As IEnumerable(Of Knowledge), links As IEnumerable(Of VertexEdge))
        For Each kb As Knowledge In knowledge
            Call AddVertex(kb)
        Next
        For Each link As VertexEdge In links
            Call AddEdge(link.U, link.V, link.weight)
        Next
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="knowledge">term</param>
    ''' <param name="meta">
    ''' another knowledge data that associated 
    ''' with the given <paramref name="knowledge"/> 
    ''' term.
    ''' </param>
    Public Sub AddKnowledge(knowledge As String, meta As Dictionary(Of String, String()))
        Dim term As Knowledge
    End Sub

End Class
