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
    ReadOnly evidenceLinkOffsets As New Dictionary(Of Long, Long)
    ReadOnly linkReader As BinaryDataReader
    ReadOnly evidenceCategory As String()
    ReadOnly evidenceCharSet As BinaryDataReader
    ReadOnly evidenceCharOffsets As Long()

    Sub New(pack As StreamPack)
        Me.pack = pack
        Me.termTypes = StorageProvider.GetKeywords("meta/keywords.msg", pack)
        Me.linkReader = New BinaryDataReader(pack.OpenBlock("/meta/evidence_link/links.dat"))
        Me.evidenceCategory = pack.loadEvidenceCategoryData
        Me.evidenceCharSet = New BinaryDataReader(pack.OpenBlock("/meta/evidence_stream/packdata.chr"))

        Using buffer As Stream = pack.OpenBlock("/meta/evidence_stream/offsets.i64")
            Dim bin As New BinaryDataReader(buffer)
            Dim nsize As Integer = evidenceCharSet.ReadInt32

            Me.evidenceCharOffsets = bin.ReadInt64s(nsize)
        End Using

        Call loadEvidenceIndex()
    End Sub

    Private Function addMetadata(knowledge As KnowledgeData) As KnowledgeData
        Dim offset As Long = evidenceLinkOffsets(CLng(knowledge.id))

        Call linkReader.Seek(offset, SeekOrigin.Begin)

        Dim size = linkReader.ReadInt32
        Dim buf = linkReader.ReadBytes(size)
        Dim evidence As EvidenceMsg = MsgPackSerializer.Deserialize(GetType(EvidenceMsg), buf)
        Dim evidences = evidence.data _
            .Select(Function(i)
                        Return New Evidence With {
                            .category = i.ref,
                            .reference = i.data
                        }
                    End Function) _
            .ToArray
        Dim metadata As New Dictionary(Of String, List(Of String))

        For Each ptr As Evidence In evidences
            Dim catName As String = evidenceCategory(ptr.category)
            Dim data As String() = ptr.reference _
                .Select(Function(i)
                            Dim os = evidenceCharOffsets(i)
                            evidenceCharSet.Seek(os, SeekOrigin.Begin)
                            Return evidenceCharSet.ReadString(BinaryStringFormat.ZeroTerminated)
                        End Function) _
                .ToArray

            If Not metadata.ContainsKey(catName) Then
                Call metadata.Add(catName, New List(Of String))
            End If

            Call metadata(catName).AddRange(data)
        Next

        knowledge.metadata = metadata _
            .ToDictionary(Function(c) c.Key,
                          Function(c)
                              Return c _
                                 .Value _
                                 .Distinct _
                                 .ToArray
                          End Function)

        Return knowledge
    End Function

    Private Sub loadEvidenceIndex()
        Using buffer As Stream = pack.OpenBlock("/meta/evidence_link/offsets.i64")
            Dim bin As New BinaryDataReader(buffer)
            Dim n As Integer = linkReader.ReadInt32

            For i As Integer = 1 To n
                Call evidenceLinkOffsets.Add(bin.ReadInt64, bin.ReadInt64)
            Next
        End Using
    End Sub

    Public Function SeekTerm(term As String) As KnowledgeData
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
        Dim knowledge As KnowledgeData

        Using block As Stream = pack.OpenBlock(blockfile),
            bin As New BinaryDataReader(block)

            bin.Seek(index.offset, SeekOrigin.Begin)
            size = bin.ReadInt32
            buf = bin.ReadBytes(size)
            v = MsgPackSerializer.Deserialize(GetType(KnowledgeMsg), buf)
            knowledge = New KnowledgeData With {
                .id = v.guid,
                .term = v.term,
                .mentions = v.mentions,
                .type = termTypes.types(v.type),
                .isMaster = v.isMaster,
                .source = v.referenceSources _
                    .Select(Function(i)
                                Return termTypes.source(i)
                            End Function) _
                    .AsList
            }

            ' add metadata and then returns the result
            Return addMetadata(knowledge)
        End Using
    End Function

End Class

Public Class KnowledgeData

    Public Property id As Integer
    Public Property term As String
    Public Property mentions As Integer
    Public Property type As String
    Public Property isMaster As Boolean
    Public Property source As String()
    Public Property metadata As Dictionary(Of String, String())

End Class