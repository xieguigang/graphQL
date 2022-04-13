Imports Microsoft.VisualBasic.Linq

Namespace Graph

    ''' <summary>
    ''' 主要用于已有知识的聚合
    ''' </summary>
    Public Class EvidenceGraph : Inherits GraphModel

        ReadOnly mapping As New Dictionary(Of String, List(Of String))
        ReadOnly evidences As EvidencePool

        Sub New(knowledge As IEnumerable(Of Knowledge), links As IEnumerable(Of Association))
            Call Console.WriteLine("add nodes...")

            For Each kb As Knowledge In knowledge
                Call AddVertex(kb)
                Call buildEvidenceMapping(kb, kb.evidence)
            Next

            Call Console.WriteLine("add links...")
            For Each link As Association In links
                Call Insert(link)
            Next
        End Sub

        Friend Sub buildEvidenceMapping(term As Knowledge, evidence As Dictionary(Of String, String()))
            For Each metadata In evidence
                For Each ref As String In metadata.Value
                    If Not mapping.ContainsKey(ref) Then
                        Call mapping.Add(ref, New List(Of String))
                    End If

                    Call mapping(ref).Add(term.label)
                Next
            Next
        End Sub

        Public Sub AddKnowledge(knowledge As String, type As String, evidence As Dictionary(Of String, String()))
            Dim term As Knowledge = ComputeIfAbsent(knowledge, type)

            Call term.AddReferenceSource(source:=type)

            For Each metadata In evidence
                If metadata.Value.IsNullOrEmpty Then
                    Continue For
                End If

                If term.evidence.ContainsKey(metadata.Key) Then
                    term.evidence(metadata.Key) = term.evidence(metadata.Key) _
                        .JoinIterates(metadata.Value) _
                        .Distinct _
                        .ToArray
                Else
                    term.evidence.Add(metadata.Key, metadata.Value)
                End If
            Next

            Call buildEvidenceMapping(term, evidence)

            ' create links between knowledge terms
            ' if evidence has intersection
            For Each metadata In evidence
                If metadata.Value.IsNullOrEmpty Then
                    Continue For
                End If

                For Each ref As String In metadata.Value
                    Dim terms = mapping(ref)

                    For Each id As String In terms
                        If id <> knowledge Then
                            Dim otherTerm = vertices(id)
                            Dim link As Association = QueryEdge(otherTerm.label, term.label)

                            If link Is Nothing Then
                                link = QueryEdge(term.label, otherTerm.label)
                            End If

                            If Not link Is Nothing Then
                                link.weight += 1
                            Else
                                link = New Association With {
                                    .type = metadata.Key,
                                    .U = otherTerm,
                                    .V = term,
                                    .weight = 1
                                }
                                Call Me.Insert(link)
                            End If

                            Call link.AddReferenceSource(source:=type)
                        End If
                    Next
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