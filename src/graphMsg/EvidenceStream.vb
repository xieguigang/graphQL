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
        Dim category As String()

        Using buf As Stream = pack.OpenBlock("/meta/evidence_stream/packdata.chr")
            Dim bin As New BinaryDataReader(buf)
            Dim nsize As Integer = bin.ReadInt32

            For i As Integer = 1 To nsize
                Call referenceData.Add(bin.ReadString(BinaryStringFormat.ZeroTerminated))
            Next
        End Using

        Using buffer As Stream = pack.OpenBlock("/meta/evidence_stream/category.txt")
            Dim bin As New BinaryDataReader(buffer)

            category = bin _
                .ReadString(BinaryStringFormat.DwordLengthPrefix) _
                .LineTokens
        End Using

        Return New EvidencePool(category, referenceData)
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
