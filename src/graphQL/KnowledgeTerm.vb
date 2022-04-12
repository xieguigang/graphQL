Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.SecurityString

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

    <Extension>
    Public Function UniqueHashCode(Of T As DynamicPropertyBase(Of String))(term As T, indexBy As String()) As Long
        Dim sb As New StringBuilder

        For Each key As String In indexBy
            Call sb.AppendLine($"{key}: {term(key)}")
        Next

        Dim hash As String = sb.ToString.GetMd5Hash.ToLower
        Dim checksum As Long = hash.StringHashCode
        Dim i32 As Integer = BitConverter.ToInt16(BitConverter.GetBytes(checksum), Scan0)

        Return i32
    End Function
End Module
