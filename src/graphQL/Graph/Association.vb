Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory

Namespace Graph

    Public Class Association : Inherits Edge(Of Knowledge)

        ''' <summary>
        ''' the meta data key name
        ''' </summary>
        ''' <returns></returns>
        Public Property type As String

        ''' <summary>
        ''' the data source of this knowledge term 
        ''' association data.
        ''' </summary>
        ''' <returns></returns>
        Public Property source As New List(Of String)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub AddReferenceSource(source As String)
            Call Me.source.Add(source)
        End Sub

    End Class
End Namespace