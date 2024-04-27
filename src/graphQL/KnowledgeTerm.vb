#Region "Microsoft.VisualBasic::105712c4461907a216301ea76d77712b, G:/graphQL/src/graphQL//KnowledgeTerm.vb"

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

    '   Total Lines: 118
    '    Code Lines: 87
    ' Comment Lines: 12
    '   Blank Lines: 19
    '     File Size: 4.41 KB


    ' Module KnowledgeTerm
    ' 
    '     Function: (+3 Overloads) CreateNiceTerm, UniqueHashCode
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports graphQL.Graph
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Data.Repository
Imports stdNum = System.Math

Public Module KnowledgeTerm

    <Extension>
    Public Function CreateNiceTerm(Of T As {New, INamedValue, DynamicPropertyBase(Of String)})(term As KnowledgeFrameRow, kb As GraphModel) As T
        If TypeOf kb Is EvidenceGraph Then
            Return term.CreateNiceTerm(Of T)(DirectCast(kb, EvidenceGraph))
        Else
            Return term.CreateNiceTerm(Of T)(DirectCast(kb, GraphPool))
        End If
    End Function

    ReadOnly internalKeys As Index(Of String) = {"knowledge_type", "source"}
    ReadOnly Name As Index(Of String) = {"name", "Name", "NAME"}

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="term"></param>
    ''' <param name="kb"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function has a special rule for the knowledge 
    ''' term its ``name`` field.
    ''' </remarks>
    <Extension>
    Public Function CreateNiceTerm(Of T As {New, INamedValue, DynamicPropertyBase(Of String)})(term As KnowledgeFrameRow, kb As EvidenceGraph) As T
        Dim nice As New T With {.Key = term.UniqeId}
        Dim terms As String()
        Dim w As Double()

        For Each key As String In From str As String
                                  In term.EnumerateKeys
                                  Where Not str Like internalKeys
            terms = term(key)

            If terms.Length = 1 Then
                nice(key) = terms(Scan0)
            Else
                w = (From str As String
                     In terms
                     Let vlist As Knowledge() = kb.GetMappingTerms(str).ToArray
                     Let score As Double = (Aggregate v As Knowledge
                                            In vlist
                                            Let vscore As Integer = v.mentions * (v.source.Count + 1)
                                            Into Sum(vscore))
                     Select score).ToArray

                If key Like KnowledgeTerm.Name Then
                    ' determine which name is the best common name?
                    Dim nchars As Double() = (From str As String
                                              In terms
                                              Let nc As Integer = str.Length
                                              Select If(nc < 3, 3 / 8, 6 / nc)).ToArray

                    nice(key) = terms(which.Max(w.Select(Function(wi, i) wi * nchars(i))))
                Else
                    nice(key) = terms(which.Max(w))
                End If
            End If
        Next

        Return nice
    End Function

    <Extension>
    Public Function CreateNiceTerm(Of T As {New, INamedValue, DynamicPropertyBase(Of String)})(term As KnowledgeFrameRow, kb As GraphPool) As T
        Dim nice As New T With {.Key = term.UniqeId}
        Dim terms As String()
        Dim w As Double()

        For Each key As String In term.EnumerateKeys
            terms = term(key)

            If terms.Length = 1 Then
                nice(key) = terms(Scan0)
            Else
                w = (From str As String
                     In terms
                     Let v As Knowledge = kb.GetElementById(str)
                     Let score As Double = v.mentions * (v.source.Count + 1)
                     Select score).ToArray

                nice(key) = terms(which.Max(w))
            End If
        Next

        Return nice
    End Function

    <Extension>
    Public Function UniqueHashCode(Of T As DynamicPropertyBase(Of String))(term As T, indexBy As String()) As Long
        Dim keys As New List(Of String)

        For Each key As String In indexBy
            Call keys.Add(term(key))
        Next

        Dim checksum As Integer = FNV1a.GetHashCode(keys)

        If checksum <= 0 Then
            checksum = stdNum.Abs(checksum)
            checksum += 3
        End If

        Return checksum
    End Function
End Module

