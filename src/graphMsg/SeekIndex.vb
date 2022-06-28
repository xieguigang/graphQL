Imports System.IO
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Public Class SeekIndex

    Public Property index As Dictionary(Of String, TermIndex)

    ReadOnly pack As StreamPack
    ReadOnly termTypes As IndexByRef

    Sub New(pack As StreamPack)
        Me.pack = pack
        Me.termTypes = StorageProvider.GetKeywords("meta/keywords.msg", pack)
    End Sub

    Public Function SeekTerm(term As String) As Knowledge
        Dim index As TermIndex

        If Not Me.index.ContainsKey(term) Then
            Return Nothing
        Else
            index = Me.index(term)
        End If

        Dim blockfile As String = $"/terms/{index.block}.dat"
        Dim size As Integer
        Dim buf As Byte()
        Dim v As KnowledgeMsg
        Dim knowledge As Knowledge

        Using block As Stream = pack.OpenBlock(blockfile),
            bin As New BinaryDataReader(block)

            bin.Seek(index.offset, SeekOrigin.Begin)
            size = bin.ReadInt32
            buf = bin.ReadBytes(size)
            v = MsgPackSerializer.Deserialize(GetType(KnowledgeMsg), buf)
            knowledge = New Knowledge With {
                .ID = v.guid,
                .label = v.term,
                .mentions = v.mentions,
                .type = termTypes.types(v.type),
                .isMaster = v.isMaster,
                .source = v.referenceSources _
                    .Select(Function(i) termTypes.source(i)) _
                    .AsList
            }

            Return knowledge
        End Using
    End Function

End Class