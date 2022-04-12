Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory

Namespace Graph

    Public MustInherit Class GraphModel : Inherits Graph(Of Knowledge, Association, GraphModel)

        ''' <summary>
        ''' get knowledge node element by id
        ''' </summary>
        ''' <param name="ref"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetElementById(ref As String) As Knowledge
            Return vertices(ref)
        End Function
    End Class
End Namespace