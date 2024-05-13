#Region "Microsoft.VisualBasic::6dc89087daf5574957282e500250cfbb, src\graphQL\KnowledgeBuilder\KnowledgeFrameRow.vb"

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

    '   Total Lines: 73
    '    Code Lines: 58
    ' Comment Lines: 2
    '   Blank Lines: 13
    '     File Size: 3.07 KB


    ' Class KnowledgeFrameRow
    ' 
    '     Properties: UniqeId
    ' 
    '     Function: (+2 Overloads) CorrectKnowledges
    ' 
    '     Sub: SaveData
    ' 
    ' /********************************************************************************/

#End Region

Imports graphQL.Graph
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq

Public Class KnowledgeFrameRow : Inherits DynamicPropertyBase(Of String())
    Implements INamedValue

    Public Property UniqeId As String Implements IKeyedEntity(Of String).Key

    Public Shared Iterator Function CorrectKnowledges(kb As GraphPool, data As IEnumerable(Of NamedCollection(Of KnowledgeFrameRow)), fieldSet As String()) As IEnumerable(Of KnowledgeFrameRow)
        Dim uniqueSet As Index(Of String) = fieldSet.Indexing

        For Each duplicated As NamedCollection(Of KnowledgeFrameRow) In data
            If duplicated.Length = 1 Then
                Yield duplicated.First
            Else
                Yield CorrectKnowledges(kb, duplicated, fieldSet)
            End If
        Next
    End Function

    Public Shared Function CorrectKnowledges(kb As GraphPool, data As NamedCollection(Of KnowledgeFrameRow), fieldUniques As Index(Of String)) As KnowledgeFrameRow
        Dim allFields As String() = data.Select(Function(a) a.Properties.Keys).IteratesALL.Distinct.ToArray
        Dim corrected As New KnowledgeFrameRow With {
            .UniqeId = data.name
        }

        For Each ref As String In allFields
            If ref Like fieldUniques Then
                ' get top reference as the unique id
                Dim top As New List(Of (ref As String, score As Double))

                For Each term As KnowledgeFrameRow In data
                    If term(ref) Is Nothing Then
                        Continue For
                    End If

                    For Each refId As String In term(ref)
                        Call top.Add((refId, kb.GetElementById(refId).mentions))
                    Next
                Next

                If top.Count = 0 Then
                    corrected(ref) = {}
                Else
                    corrected(ref) = {top.OrderByDescending(Function(i) i.score).First.ref}
                End If
            Else
                ' just union 
                corrected(ref) = (From str As String
                                  In data.Select(Function(i) i(ref)).IteratesALL
                                  Where Not str.StringEmpty
                                  Select str
                                  Distinct
                                  Order By str).ToArray
            End If
        Next

        Return corrected
    End Function

    Friend Shared Sub SaveData(tree As BTreeCluster, ByRef save As List(Of BTreeCluster))
        Call save.Add(tree)

        If Not tree.left Is Nothing Then Call SaveData(tree.left, save)
        If Not tree.right Is Nothing Then Call SaveData(tree.right, save)
    End Sub

End Class
