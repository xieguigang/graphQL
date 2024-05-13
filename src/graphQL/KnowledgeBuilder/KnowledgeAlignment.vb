#Region "Microsoft.VisualBasic::8b2a18e66e11876b67750785e5ddd8eb, src\graphQL\KnowledgeBuilder\KnowledgeAlignment.vb"

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

    '   Total Lines: 88
    '    Code Lines: 64
    ' Comment Lines: 9
    '   Blank Lines: 15
    '     File Size: 2.62 KB


    ' Class KnowledgeAlignment
    ' 
    '     Properties: AllUniqueTerms
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetMembers, GetObject, GetSimilarity
    ' 
    '     Sub: getScoreXy
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class KnowledgeAlignment : Inherits ComparisonProvider

    ReadOnly all As Dictionary(Of String, KnowledgeFrameRow)
    ReadOnly fieldSet As String()

    Public ReadOnly Property AllUniqueTerms As IEnumerable(Of String)
        Get
            Return all.Keys
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="all"></param>
    ''' <param name="fieldSet">
    ''' A collection set of data fields for produce alignment score.
    ''' </param>
    ''' <param name="equals"></param>
    ''' <param name="gt"></param>
    Public Sub New(all As IEnumerable(Of KnowledgeFrameRow), fieldSet As String(), equals As Double, gt As Double)
        MyBase.New(equals, gt)

        Me.fieldSet = fieldSet
        Me.all = all.ToDictionary(Function(i) i.UniqeId)
    End Sub

    Public Function GetMembers(ref As BTreeCluster) As KnowledgeFrameRow()
        Return (From id As String In ref.members Select all(id)).ToArray
    End Function

    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Dim a As KnowledgeFrameRow = all(x)
        Dim b As KnowledgeFrameRow = all(y)
        Dim v1 As New Vector(fieldSet.Length)
        Dim v2 As New Vector(fieldSet.Length)
        Dim d1 As Double = 0
        Dim d2 As Double = 0

        For i As Integer = 0 To fieldSet.Length - 1
            Call getScoreXy(a, b, fieldSet(i), d1, d2)

            v1(i) = d1
            v2(i) = d2
        Next

        Return v1.SSM(v2)
    End Function

    Private Shared Sub getScoreXy(a As KnowledgeFrameRow, b As KnowledgeFrameRow, ref As String, ByRef x As Double, ByRef y As Double)
        Dim v1 As String() = a(ref)
        Dim v2 As String() = b(ref)
        Dim nil1 = v1.IsNullOrEmpty
        Dim nil2 = v2.IsNullOrEmpty

        If nil1 AndAlso nil2 Then
            x = 0
            y = 0
            Return
        ElseIf nil1 Then
            x = 0
            y = 1
            Return
        ElseIf nil2 Then
            x = 1
            y = 0
            Return
        End If

        Dim jaccard As Double = v1.Intersect(v2).Count / v1.Union(v2).Count

        If jaccard = 0 Then
            x = 0
            y = 0
        Else
            x = 1
            y = jaccard
        End If
    End Sub

    Public Overrides Function GetObject(id As String) As Object
        Return all(id)
    End Function
End Class
