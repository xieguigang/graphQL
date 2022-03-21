Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

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
    Public Shared Iterator Function GroupBy(data As IEnumerable(Of KnowledgeFrameRow),
                                            Optional identical As Double = 0.95,
                                            Optional similar As Double = 0.8) As IEnumerable(Of NamedCollection(Of KnowledgeFrameRow))

    End Function

End Class
