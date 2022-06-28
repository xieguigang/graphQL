Imports System.IO
Imports graphMsg.Message
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Public Class SeekIndex

    Public Property index As Dictionary(Of String, TermIndex)
    Public Property pack As StreamPack

    Public Function SeekTerm(term As String) As Object
        Dim index As TermIndex

        If Not Me.index.ContainsKey(term) Then
            Return Nothing
        Else
            index = Me.index(term)
        End If

        Dim blockfile As String = $"/terms/{index.block}.dat"
        Dim size As Integer
        Dim buf As Byte()
        Dim msg As KnowledgeMsg

        Using block As Stream = pack.OpenBlock(blockfile),
            bin As New BinaryDataReader(block)

            bin.Seek(index.offset, SeekOrigin.Begin)
            size = bin.ReadInt32
            buf = bin.ReadBytes(size)
            msg = MsgPackSerializer.Deserialize(GetType(KnowledgeMsg), buf)

            Return msg
        End Using
    End Function

End Class