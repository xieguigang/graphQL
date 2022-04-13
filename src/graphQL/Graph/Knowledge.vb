Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory

Namespace Graph

    ''' <summary>
    ''' A knowledge node in this graph database
    ''' </summary>
    Public Class Knowledge : Inherits Vertex

        ''' <summary>
        ''' the count of the current knowledge term 
        ''' has been mentioned in the knowledge 
        ''' network.
        ''' </summary>
        ''' <returns></returns>
        Public Property mentions As Integer

        ''' <summary>
        ''' the data type of the current term data.
        ''' </summary>
        ''' <returns></returns>
        Public Property type As String

        ''' <summary>
        ''' current knowledge term is a master source term?
        ''' </summary>
        ''' <returns>
        ''' true - is a master source term, 
        ''' false - is a meta data term
        ''' </returns>
        Public Property isMaster As Boolean

        ''' <summary>
        ''' the data source of this knowledge term node
        ''' </summary>
        ''' <returns></returns>
        Public Property source As New List(Of String)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 20220413
        ''' 
        ''' 在这里原先是直接使用一个字符串字典来表示的，但是在处理大型数据集的时候
        ''' 在指向相同对象实体的词条之间冗余的字符串会造成大量的内存浪费
        ''' 所以在这里改为指针数据以减少内存冗余信息，降低内存占用率
        ''' </remarks>
        Public Property evidence As New List(Of Evidence)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub AddReferenceSource(source As String)
            Call Me.source.Add(source)
        End Sub

    End Class
End Namespace
