Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq

Namespace Graph

    ''' <summary>
    ''' 主要用于已有知识的聚合
    ''' </summary>
    Public Class EvidenceGraph : Inherits Graph(Of Knowledge, Association, GraphPool)

        ReadOnly mapping As New Dictionary(Of String, List(Of String))

        Public Sub AddKnowledge(knowledge As String, type As String, evidence As Dictionary(Of String, String()))
            Dim term As Knowledge = ComputeIfAbsent(knowledge, type)

            Call term.AddReferenceSource(source:=type)

            For Each metadata In evidence
                If term.evidence.ContainsKey(metadata.Key) Then
                    term.evidence(metadata.Key) = term.evidence(metadata.Key) _
                        .JoinIterates(metadata.Value) _
                        .Distinct _
                        .ToArray
                Else
                    term.evidence.Add(metadata.Key, metadata.Value)
                End If

                For Each ref As String In metadata.Value
                    If Not mapping.ContainsKey(ref) Then
                        Call mapping.Add(ref, New List(Of String))
                    End If

                    Call mapping(ref).Add(term.label)
                Next
            Next
        End Sub

        Private Function ComputeIfAbsent(term As String, type As String) As Knowledge
            Dim vertex As Knowledge

            If Me.vertices.ContainsKey(term) Then
                vertex = Me.vertices(term)
            Else
                vertex = Me.AddVertex(term)
                vertex.type = type
            End If

            vertex.mentions += 1
            vertex.isMaster = True

            Return vertex
        End Function
    End Class
End Namespace