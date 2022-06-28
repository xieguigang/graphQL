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
        Dim offsets As New List(Of Long)

        Using buffer As Stream = pack.OpenBlock("/meta/evidence_link/links.dat")
            Dim bin As New BinaryDataWriter(buffer)
            Dim buf As Byte()

            Call bin.Write(evidences.Length)

            For Each evidence As EvidenceMsg In evidences
                offsets.Add(bin.Position)
                buf = MsgPackSerializer.SerializeObject(evidence)
                bin.Write(buf.Length)
                bin.Write(buf)
            Next

            Call bin.Flush()
        End Using

        Using buffer As Stream = pack.OpenBlock("/meta/evidence_link/offsets.i64")
            Dim bin As New BinaryDataWriter(buffer)

            Call bin.Write(offsets.ToArray)
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
            Dim offsets As Long() = offsetReader.ReadInt64s(nsize)
            Dim evidence As EvidenceMsg
            Dim buf As Byte()
            Dim size As Integer
            Dim evidences As IEnumerable(Of Evidence)

            For Each pos As Long In offsets
                linkReader.Seek(pos, SeekOrigin.Begin)
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
