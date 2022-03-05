Imports Microsoft.VisualBasic.Data.GraphTheory

Public Class GraphPool : Inherits Graph(Of Knowledge, Association, GraphPool)

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
        Dim term As Knowledge = ComputeIfAbsent(knowledge)

        For Each info As KeyValuePair(Of String, String()) In meta
            For Each data As String In info.Value
                Dim metadata As Knowledge = ComputeIfAbsent(data)
                Dim refId As String = VertexEdge.EdgeKey(metadata, term)
                Dim link As Association

                If edges.ContainsKey(refId) Then
                    link = edges(refId)
                    link.weight += 1
                Else
                    link = New Association With {
                        .type = info.Key,
                        .U = metadata,
                        .V = term,
                        .weight = 1
                    }
                    Call Me.Insert(link)
                End If
            Next
        Next
    End Sub

    Private Function ComputeIfAbsent(term As String) As Knowledge
        If Me.ExistVertex(term) Then
            Return Me.vertices(term)
        Else
            Return Me.AddVertex(term)
        End If
    End Function

End Class
