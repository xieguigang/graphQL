#Region "Microsoft.VisualBasic::dfcd145752fc64210cc17101010f2c80, G:/graphQL/src/graphQL//Graph/EvidenceGraph.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 206
    '    Code Lines: 147
    ' Comment Lines: 22
    '   Blank Lines: 37
    '     File Size: 8.05 KB


    '     Class EvidenceGraph
    ' 
    '         Properties: evidences
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ComputeIfAbsent, GetMappingTerms, isEmptyString
    ' 
    '         Sub: AddIgnores, AddKnowledge, buildEvidenceGraph, (+3 Overloads) buildEvidenceMapping, JoinEvidence
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace Graph

    ''' <summary>
    ''' 主要用于已有知识的聚合
    ''' </summary>
    Public Class EvidenceGraph : Inherits GraphModel

        ReadOnly mapping As New Dictionary(Of String, List(Of String))
        ''' <summary>
        ''' evidence types to ignores from build graph links
        ''' </summary>
        ReadOnly ignores As New Index(Of String)

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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub AddIgnores(type As String)
            Call ignores.Add(type)
        End Sub

        Public Iterator Function GetMappingTerms(key As String) As IEnumerable(Of Knowledge)
            If Not mapping.ContainsKey(key) Then
                Return
            End If

            For Each refer As String In mapping(key)
                Yield GetElementById(refer)
            Next
        End Function

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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Shared Function isEmptyString(str As String) As Boolean
            Return str Is Nothing OrElse str.Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF) = ""
        End Function

        Private Sub buildEvidenceMapping(term As Knowledge, evidence As Dictionary(Of String, String()))
            For Each metadata In evidence
                For Each ref As String In metadata.Value
                    If Not mapping.ContainsKey(ref) Then
                        Call mapping.Add(ref, New List(Of String))
                    End If

                    Call mapping(ref).Add(term.label)
                Next
            Next
        End Sub

        Friend Sub buildEvidenceMapping(term As Knowledge, evidence As IEnumerable(Of NamedValue(Of String())))
            For Each metadata As NamedValue(Of String()) In evidence
                For Each ref As String In metadata.Value
                    If Not mapping.ContainsKey(ref) Then
                        Call mapping.Add(ref, New List(Of String))
                    End If

                    Call mapping(ref).Add(term.label)
                Next
            Next
        End Sub

        Public Sub AddKnowledge(knowledge As String, type As String, evidence As Dictionary(Of String, String()), Optional selfReference As Boolean = True)
            Dim term As Knowledge = ComputeIfAbsent(knowledge, type)

            Call term.AddReferenceSource(source:=type)

            ' clean up of the evidence data
            For Each key As String In evidence.Keys.ToArray
                If evidence(key).IsNullOrEmpty OrElse evidence(key).All(AddressOf isEmptyString) Then
                    Call evidence.Remove(key)
                Else
                    evidence(key) = evidence(key) _
                        .Where(Function(str) Not isEmptyString(str)) _
                        .ToArray

                    If evidence(key).Length = 0 Then
                        Call evidence.Remove(key)
                    End If
                End If
            Next

            Call JoinEvidence(term, evidence)
            Call buildEvidenceMapping(term, evidence)
            Call buildEvidenceGraph(term, evidence, selfReference)
        End Sub

        ''' <summary>
        ''' add evidence data into graph
        ''' </summary>
        ''' <param name="term"></param>
        ''' <param name="evidence"></param>
        Public Sub JoinEvidence(ByRef term As Knowledge, evidence As IEnumerable(Of KeyValuePair(Of String, String())))
            For Each metadata In evidence.ToArray
                Dim evidenceItem = evidences.FindEvidence(term, category:=metadata.Key)

                If Not evidenceItem Is Nothing Then
                    Call evidences.Join(evidenceItem, metadata.Value)
                Else
                    evidenceItem = evidences.CreateEvidence(metadata.Key, metadata.Value)
                    term.evidence.Add(evidenceItem)
                End If
            Next
        End Sub

        ''' <summary>
        ''' create links between knowledge terms
        ''' if evidence has intersection
        ''' </summary>
        Friend Sub buildEvidenceGraph(term As Knowledge, evidence As IEnumerable(Of KeyValuePair(Of String, String())), selfReference As Boolean)
            For Each metadata As KeyValuePair(Of String, String()) In evidence
                ' skip of the evidence data 
                ' if the data type of the evidence data is specific to ignored
                If metadata.Key Like ignores Then
                    Continue For
                End If

                For Each ref As String In metadata.Value
                    Dim terms As IEnumerable(Of String) = mapping(ref)

                    For Each id As String In terms
                        If id <> term.label Then
                            Dim otherTerm As Knowledge = vertices(id)

                            If Not selfReference AndAlso term.type = otherTerm.type Then
                                Continue For
                            End If

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

                            Call link.AddReferenceSource(source:=$"{metadata.Key}:{ref}")
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
