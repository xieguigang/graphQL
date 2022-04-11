Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Public Module KnowledgeTerm

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
                w = (From str As String In terms
                     Let v As Knowledge = kb.GetElementById(str)
                     Let score As Double = v.mentions * (v.source.Count + 1)
                     Select score).ToArray

                nice(key) = terms(which.Max(w))
            End If
        Next

        Return nice
    End Function
End Module
