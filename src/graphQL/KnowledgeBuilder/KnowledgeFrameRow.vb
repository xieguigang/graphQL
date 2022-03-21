Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq

Public Class KnowledgeFrameRow : Inherits DynamicPropertyBase(Of String())
    Implements INamedValue

    Public Property UniqeId As String Implements IKeyedEntity(Of String).Key

    ''' <summary>
    ''' unique and merge data by binary tree clustering
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="identical"></param>
    ''' <param name="similar"></param>
    ''' <returns></returns>
    Public Shared Iterator Function GroupBy(data As IEnumerable(Of KnowledgeFrameRow), fieldSet As String(),
                                            Optional identical As Double = 0.6,
                                            Optional similar As Double = 0.1) As IEnumerable(Of NamedCollection(Of KnowledgeFrameRow))

        Dim alignment As New KnowledgeAlignment(data, fieldSet, identical, similar)
        Dim rooTree As BTreeCluster = alignment.AllUniqueTerms.BTreeCluster(alignment)
        Dim poll As New List(Of BTreeCluster)

        Call SaveData(rooTree, save:=poll)

        For Each node As BTreeCluster In poll
            Yield New NamedCollection(Of KnowledgeFrameRow) With {
                .name = node.uuid,
                .value = alignment.GetMembers(node)
            }
        Next
    End Function

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
                    For Each refId As String In term(ref)
                        Call top.Add((refId, kb.GetElementById(refId).mentions))
                    Next
                Next

                corrected(ref) = {top.OrderByDescending(Function(i) i.score).First.ref}
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

    Private Shared Sub SaveData(tree As BTreeCluster, ByRef save As List(Of BTreeCluster))
        Call save.Add(tree)

        If Not tree.left Is Nothing Then Call SaveData(tree.left, save)
        If Not tree.right Is Nothing Then Call SaveData(tree.right, save)
    End Sub

End Class
