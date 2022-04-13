Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace Graph

    ''' <summary>
    ''' 主要用于已有知识的聚合
    ''' </summary>
    Public Class EvidenceGraph : Inherits GraphModel

        ReadOnly mapping As New Dictionary(Of String, List(Of String))

        ''' <summary>
        ''' evidence data
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property evidences As EvidencePool

        Sub New(knowledge As IEnumerable(Of Knowledge), links As IEnumerable(Of Association), evidence As EvidencePool)
            Me.evidences = evidence

            Call Console.WriteLine("add nodes...")

            For Each kb As Knowledge In knowledge
                Call AddVertex(kb)
                Call buildEvidenceMapping(kb)
            Next

            Call Console.WriteLine("add links...")
            For Each link As Association In links
                Call Insert(link)
            Next
        End Sub

        Friend Sub buildEvidenceMapping(term As Knowledge)
            For Each evidence As Evidence In term.evidence
                For Each referId As Integer In evidence.reference
                    Dim ref As String = evidences(referId)

                    If Not mapping.ContainsKey(ref) Then
                        Call mapping.Add(ref, New List(Of String))
                    End If

                    Call mapping(ref).Add(term.label)
                Next
            Next
        End Sub

        Friend Sub buildEvidenceMapping(term As Knowledge, evidence As Dictionary(Of String, String()))
            For Each metadata In evidence
                If metadata.Value.IsNullOrEmpty Then
                    Continue For
                End If

                For Each ref As String In metadata.Value
                    If ref Is Nothing OrElse ref.Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF) = "" Then
                        Continue For
                    End If
                    If Not mapping.ContainsKey(ref) Then
                        Call mapping.Add(ref, New List(Of String))
                    End If

                    Call mapping(ref).Add(term.label)
                Next
            Next
        End Sub

        Public Sub AddKnowledge(knowledge As String, type As String, evidence As Dictionary(Of String, String()))
            Dim term As Knowledge = ComputeIfAbsent(knowledge, type)
            Dim evidenceItem As Evidence

            Call term.AddReferenceSource(source:=type)

            For Each metadata In evidence
                If metadata.Value.IsNullOrEmpty Then
                    Continue For
                Else
                    evidenceItem = evidences.FindEvidence(term, category:=metadata.Key)
                End If

                If Not evidenceItem Is Nothing Then
                    Call evidences.Join(evidenceItem, metadata.Value)
                Else
                    evidenceItem = evidences.CreateEvidence(metadata.Key, metadata.Value)
                    term.evidence.Add(evidenceItem)
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
                    If ref Is Nothing OrElse ref.Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF) = "" Then
                        Continue For
                    Else
                        terms = mapping(ref)
                    End If

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