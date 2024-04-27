#Region "Microsoft.VisualBasic::e3decbd39bd060ebd96a6cf6464c8ad9, G:/graphQL/src/graphMsg//EvidenceStream.vb"

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

    '   Total Lines: 70
    '    Code Lines: 53
    ' Comment Lines: 0
    '   Blank Lines: 17
    '     File Size: 2.41 KB


    ' Module EvidenceStream
    ' 
    '     Function: Load, loadEvidenceCategoryData
    ' 
    '     Sub: Exports
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Module EvidenceStream

    <Extension>
    Public Function Load(pack As StreamPack) As EvidencePool
        Dim referenceData As New List(Of String)
        Dim category As String() = pack.loadEvidenceCategoryData

        Using buf As Stream = pack.OpenBlock("/meta/evidence_stream/packdata.chr")
            Dim bin As New BinaryDataReader(buf)
            Dim nsize As Integer = bin.ReadInt32

            For i As Integer = 1 To nsize
                Call referenceData.Add(bin.ReadString(BinaryStringFormat.ZeroTerminated))
            Next
        End Using

        Return New EvidencePool(category, referenceData)
    End Function

    <Extension>
    Friend Function loadEvidenceCategoryData(pack As StreamPack) As String()
        Using buffer As Stream = pack.OpenBlock("/meta/evidence_stream/category.txt")
            Dim bin As New BinaryDataReader(buffer)

            Return bin _
                .ReadString(BinaryStringFormat.DwordLengthPrefix) _
                .LineTokens
        End Using
    End Function

    <Extension>
    Public Sub Exports(evidenceRef As IndexByRef, file As StreamPack)
        Dim offsets As New List(Of Long)

        Using buffer As Stream = file.OpenBlock("/meta/evidence_stream/packdata.chr")
            Dim bin As New BinaryDataWriter(buffer)

            Call bin.Write(evidenceRef.source.Length)

            For Each line As String In evidenceRef.source
                Call offsets.Add(bin.Position)
                Call bin.Write(line, BinaryStringFormat.ZeroTerminated)
            Next

            Call bin.Flush()
        End Using

        Using buffer As Stream = file.OpenBlock("/meta/evidence_stream/offsets.i64")
            Dim bin As New BinaryDataWriter(buffer)

            Call bin.Write(offsets.ToArray)
            Call bin.Flush()
        End Using

        Using buffer As Stream = file.OpenBlock("/meta/evidence_stream/category.txt")
            Dim bin As New BinaryDataWriter(buffer)

            Call bin.Write(evidenceRef.types.JoinBy(vbLf), BinaryStringFormat.DwordLengthPrefix)
            Call bin.Flush()
        End Using
    End Sub

End Module

