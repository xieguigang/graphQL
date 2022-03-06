Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class GraphPool : Inherits Graph(Of Knowledge, Association, GraphPool)

    Sub New(knowledge As IEnumerable(Of Knowledge), links As IEnumerable(Of Association))
        For Each kb As Knowledge In knowledge
            Call AddVertex(kb)
        Next
        For Each link As Association In links
            Call Me.Insert(link)
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
            If info.Value Is Nothing Then
                Continue For
            End If

            For Each data As String In info.Value
                If data Is Nothing Then
                    Continue For
                End If

                Dim metadata As Knowledge = ComputeIfAbsent(data)

                If metadata Is term Then
                    Continue For
                End If

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
        Dim vertex As Knowledge

        If Me.vertices.ContainsKey(term) Then
            vertex = Me.vertices(term)
        Else
            vertex = Me.AddVertex(term)
        End If

        vertex.Mentions += 1

        Return vertex
    End Function

    Public Iterator Function GetKnowledgeData(term As String) As IEnumerable(Of KnowledgeDescription)
        Dim knowledge As Knowledge = If(ExistVertex(term), vertices(term), Nothing)

        If knowledge Is Nothing Then
            Return
        End If

        For Each edge As Association In edges.Values
            If edge.V Is knowledge Then
                Yield New KnowledgeDescription With {
                    .target = edge.U.label,
                    .confidence = edge.weight,
                    .type = edge.type,
                    .query = edge.V.label,
                    .relationship = Relationship.has,
                    .mentions = (edge.V.Mentions, edge.U.Mentions)
                }
            ElseIf edge.U Is knowledge Then
                Yield New KnowledgeDescription With {
                    .target = edge.V.label,
                    .confidence = edge.weight,
                    .type = edge.type,
                    .query = edge.U.label,
                    .relationship = Relationship.is,
                    .mentions = (edge.U.Mentions, edge.V.Mentions)
                }
            End If
        Next
    End Function

    Public Function Similar(x As String, y As String, Optional weights As Dictionary(Of String, Double) = Nothing) As Double
        Dim v1 = GetKnowledgeData(x).DoCall(AddressOf getVectorSet)
        Dim v2 = GetKnowledgeData(y).DoCall(AddressOf getVectorSet)
        Dim allGroups As String() = v1.Keys _
            .JoinIterates(v2.Keys) _
            .Distinct _
            .ToArray
        Dim scores As Vector = allGroups _
            .Select(Function(i)
                        Return Jaccard(v1.TryGetValue(i), v2.TryGetValue(i))
                    End Function) _
            .AsVector

        If weights.IsNullOrEmpty Then
            Return scores.Average
        Else
            ' weighted average
            Dim w As Vector = allGroups _
                .Select(Function(i) weights.TryGetValue(i)) _
                .AsVector
            Dim score As Double = (w * scores).Sum

            Return score
        End If
    End Function

    Private Shared Function getVectorSet(list As IEnumerable(Of KnowledgeDescription)) As Dictionary(Of String, Dictionary(Of String, KnowledgeDescription))
        Return list _
            .GroupBy(Function(i) i.type) _
            .ToDictionary(Function(i) i.Key,
                          Function(i)
                              Return topNFactor(i, 5).ToDictionary(Function(j) j.target)
                          End Function)
    End Function

    Private Shared Iterator Function topNFactor(list As IGrouping(Of String, KnowledgeDescription), n As Integer) As IEnumerable(Of KnowledgeDescription)
        Dim keyGroups = list _
            .GroupBy(Function(i) i.target) _
            .OrderByDescending(Function(i)
                                   Return i.Sum(Function(o) o.score)
                               End Function) _
            .ToArray

        For Each group In keyGroups.Take(n)
            Yield group.OrderByDescending(Function(i) i.score).First
        Next
    End Function

    Private Function Jaccard(x As Dictionary(Of String, KnowledgeDescription), y As Dictionary(Of String, KnowledgeDescription)) As Double
        If x.IsNullOrEmpty OrElse y.IsNullOrEmpty Then
            Return 0
        Else
            Call x.Add(x.First.Value.query, x.Values.OrderByDescending(Function(i) i.score).First)
            Call y.Add(y.First.Value.query, y.Values.OrderByDescending(Function(i) i.score).First)
        End If

        Dim intersect As String() = x.Keys.Intersect(y.Keys).ToArray
        Dim intersectScore = intersect.Select(Function(a) x(a).score + y(a).score).Sum
        Dim unionScore = Aggregate i As KnowledgeDescription
                         In x.Values.JoinIterates(y.Values)
                         Into Sum(i.score)

        Return intersectScore / unionScore
    End Function

End Class
