Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Public Class KnowledgeFrameRow : Inherits DynamicPropertyBase(Of String())
    Implements INamedValue

    Public Property UniqeId As String Implements IKeyedEntity(Of String).Key

End Class
