#Region "Microsoft.VisualBasic::e757bbddf74fc3a6c7eae01eec7126ba, src\graphMsg\EvidenceLinks.vb"

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

    '   Total Lines: 80
    '    Code Lines: 63
    ' Comment Lines: 4
    '   Blank Lines: 13
    '     File Size: 2.99 KB


    ' Module EvidenceLinks
    ' 
    '     Sub: LoadEvidence, SaveEvidence
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq

Module EvidenceLinks

    Friend Sub SaveEvidence(evidences As EvidenceMsg(), pack As StreamPack)
        Dim offsets As New Dictionary(Of Long, Long)

        Using buffer As Stream = pack.OpenBlock("/meta/evidence_link/links.dat")
            Dim bin As New BinaryDataWriter(buffer)
            Dim buf As Byte()

            Call bin.Write(evidences.Length)

            For Each evidence As EvidenceMsg In evidences
                offsets.Add(evidence.ref, bin.Position)
                buf = MsgPackSerializer.SerializeObject(evidence)
                bin.Write(buf.Length)
                bin.Write(buf)
            Next

            Call bin.Flush()
        End Using

        Using buffer As Stream = pack.OpenBlock("/meta/evidence_link/offsets.i64")
            Dim bin As New BinaryDataWriter(buffer)

            For Each index In offsets
                Call bin.Write(index.Key)
                Call bin.Write(index.Value)
            Next

            Call bin.Flush()
        End Using
    End Sub

    ''' <summary>
    ''' attach evidence data for each knowledge terms
    ''' </summary>
    ''' 
    <Extension>
    Public Sub LoadEvidence(terms As Dictionary(Of String, Knowledge), pack As StreamPack)
        Using linkBuf As Stream = pack.OpenBlock("/meta/evidence_link/links.dat"),
            offsetBuf As Stream = pack.OpenBlock("/meta/evidence_link/offsets.i64")

            Dim offsetReader As New BinaryDataReader(offsetBuf)
            Dim linkReader As New BinaryDataReader(linkBuf)
            Dim nsize As Integer = linkReader.ReadInt32
            Dim evidence As EvidenceMsg
            Dim buf As Byte()
            Dim size As Integer
            Dim evidences As IEnumerable(Of Evidence)

            For idx As Integer = 1 To nsize
                offsetReader.ReadInt64()
                linkReader.Seek(offsetReader.ReadInt64, SeekOrigin.Begin)
                size = linkReader.ReadInt32
                buf = linkReader.ReadBytes(size)
                evidence = MsgPackSerializer.Deserialize(GetType(EvidenceMsg), buf)
                evidences = evidence.data _
                    .Select(Function(i)
                                Return New Evidence With {
                                    .category = i.ref,
                                    .reference = i.data
                                }
                            End Function) _
                    .ToArray

                Call terms(evidence.ref.ToString).evidence.AddRange(evidences)
            Next
        End Using
    End Sub
End Module
