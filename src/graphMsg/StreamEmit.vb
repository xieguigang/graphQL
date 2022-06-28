Imports System.IO
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class StreamEmit

    ''' <summary>
    ''' save as HDS file
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function Save(kb As GraphModel, file As Stream) As Boolean
        Dim termRef As New IndexByRef
        Dim linkRef As New IndexByRef
        Dim terms = KnowledgeMsg.GetTerms(kb, termRef).ToArray
        Dim links = LinkMsg.GetRelationships(kb, linkRef).ToArray
        Dim evidences = EvidenceMsg.CreateEvidencePack(kb).ToArray
        Dim info As New Dictionary(Of String, String)
        Dim evidencePool As EvidencePool = Nothing
        Dim evidenceRef As New IndexByRef With {
            .types = {},
            .source = {}
        }

        If TypeOf kb Is EvidenceGraph Then
            evidencePool = DirectCast(kb, EvidenceGraph).evidences
            evidenceRef = New IndexByRef With {
                .types = evidencePool.categoryList,
                .source = evidencePool.evidenceReference
            }
        End If

        Call info.Add("knowledge_terms", terms.Length)
        Call info.Add("graph_size", links.Length)
        Call info.Add("knowledge_types", termRef.types.Length)
        Call info.Add("link_types", links.Count)
        Call info.Add("evidence_types", If(evidencePool Is Nothing, 0, evidencePool.categoryList.Length))
        Call info.Add("evidence_size", Aggregate i In evidences Into Sum(i.data.Length))

        Using hdsPack As New StreamPack(file)
            ' save graph types
            Call MsgPackSerializer.SerializeObject(termRef, hdsPack.OpenBlock("meta/keywords.msg"), closeFile:=True)
            Call MsgPackSerializer.SerializeObject(linkRef, hdsPack.OpenBlock("meta/associations.msg"), closeFile:=True)
            Call MsgPackSerializer.SerializeObject(evidenceRef, hdsPack.OpenBlock("meta/evidences.msg"), closeFile:=True)

            Call SaveTerms(terms, hdsPack) _
                .GetJson _
                .FlushTo(
                    out:=New StreamWriter(hdsPack.OpenBlock("knowledge_blocks.json")),
                    closeFile:=True
                )
            Call SaveEvidence(evidences, hdsPack) _
                .GetJson _
                .FlushTo(
                    out:=New StreamWriter(hdsPack.OpenBlock("evidence_blocks.json")),
                    closeFile:=True
                )

            Call info.Add("graph_blocks", SaveNetwork(links, hdsPack))
            Call info _
                .GetJson _
                .FlushTo(
                    out:=New StreamWriter(hdsPack.OpenBlock("summary.json")),
                    closeFile:=True
                )
        End Using

        Return True
    End Function

    Private Shared Function SaveEvidence(evidences As EvidenceMsg(), pack As StreamPack) As Dictionary(Of String, Integer)
        Dim blocks = evidences _
            .GroupBy(Function(i)
                         Return i.ref.ToString.Last.ToString
                     End Function) _
            .ToArray
        Dim buffer As Stream
        Dim summary As New Dictionary(Of String, Integer)

        For Each block In blocks
            evidences = block.ToArray
            buffer = pack.OpenBlock($"evidences/{block.Key}.dat")

            Call MsgPackSerializer.SerializeObject(evidences, buffer, closeFile:=True)
            Call summary.Add(block.Key, evidences.Length)
        Next

        Return summary
    End Function

    ''' <summary>
    ''' save all knowledge terms data 
    ''' </summary>
    ''' <param name="terms"></param>
    ''' <param name="pack"></param>
    ''' <returns></returns>
    Private Shared Function SaveTerms(terms As KnowledgeMsg(), pack As StreamPack) As Dictionary(Of String, Integer)
        ' split terms into multiple data groups
        Dim blocks = terms _
            .GroupBy(Function(t) Mid(t.term, 1, 3)) _
            .Where(Function(g) g.Key <> "") _
            .GroupBy(Function(g) g.Key.MD5.Substring(0, 2)) _
            .ToArray
        Dim buffer As Stream
        Dim summary As New Dictionary(Of String, Integer)
        Dim index As New List(Of TermIndex)
        Dim bin As BinaryDataWriter

        For Each block In blocks
            ' save terms to one data block section
            terms = block.IteratesALL.ToArray
            buffer = pack.OpenBlock($"terms/{block.Key}.dat")
            bin = New BinaryDataWriter(buffer)

            For Each term As KnowledgeMsg In terms
                Dim ms As Byte() = MsgPackSerializer.SerializeObject(term)
                Dim offset As Integer = bin.Position

                Call bin.Write(ms.Length)
                Call bin.Write(ms)
                Call bin.Flush()

                Call index.Add(New TermIndex With {.block = block.Key, .offset = offset, .term = term.term})
            Next

            Call buffer.Flush()
            Call buffer.Dispose()
            Call summary.Add(block.Key, terms.Length)
        Next

        ' save index data
        buffer = pack.OpenBlock("index.dat")
        MsgPackSerializer.SerializeObject(New TermIndexMsg With {.index = index.ToArray}, buffer, closeFile:=True)

        Return summary
    End Function

    Private Shared Function SaveNetwork(links As LinkMsg(), pack As StreamPack) As Integer
        Dim blocks = links.OrderByDescending(Function(d) d.weight).Split(4096)
        Dim buffer As Stream
        Dim i As Integer = 0

        For Each block In blocks
            i += 1
            buffer = pack.OpenBlock($"graph/{i.FormatZero("000")}")

            Call MsgPackSerializer.SerializeObject(block, buffer, closeFile:=True)
        Next

        Return blocks.Length
    End Function

    Public Shared Function GetKnowledges(pack As StreamPack) As Dictionary(Of String, Knowledge)
        Dim terms As New Dictionary(Of String, Knowledge)
        Dim termTypes As IndexByRef = StorageProvider.GetKeywords("meta/keywords.msg", pack)
        Dim files As StreamBlock() = pack.files
        Dim summary As Dictionary(Of String, Integer)

        Using buf As Stream = pack.OpenBlock("knowledge_blocks.json"),
            read As New StreamReader(buf)

            summary = read _
                .ReadToEnd _
                .LoadJSON(Of Dictionary(Of String, Integer))
        End Using

        For Each item As StreamBlock In files.Where(Function(t) t.fullName.StartsWith("terms"))
            Dim list As New List(Of KnowledgeMsg)
            Dim blockKey As String = item.referencePath.FileName
            Dim nsize As Integer = summary(blockKey)

            Using bin As New BinaryDataReader(pack.OpenBlock(item))
                For i As Integer = 1 To nsize
                    Dim size As Integer = bin.ReadInt32
                    Dim buf As Byte() = bin.ReadBytes(size)
                    Dim term As KnowledgeMsg = MsgPackSerializer.Deserialize(GetType(KnowledgeMsg), buf)

                    Call list.Add(term)
                Next
            End Using

            For Each v As KnowledgeMsg In list
                terms(v.guid.ToString) = New Knowledge With {
                    .ID = v.guid,
                    .label = v.term,
                    .mentions = v.mentions,
                    .type = termTypes.types(v.type),
                    .isMaster = v.isMaster,
                    .source = v.referenceSources _
                        .Select(Function(i) termTypes.source(i)) _
                        .AsList
                }
            Next
        Next

        ' attach evidence data for each knowledge terms
        For Each item In files.Where(Function(t) t.fullName.StartsWith("evidences"))
            Dim list = MsgPackSerializer.Deserialize(Of EvidenceMsg())(pack.OpenBlock(item))
            Dim evidences As IEnumerable(Of Evidence)

            For Each evi As EvidenceMsg In list
                evidences = evi.data _
                    .Select(Function(i)
                                Return New Evidence With {
                                    .category = i.ref,
                                    .reference = i.data
                                }
                            End Function) _
                    .ToArray

                ' terms(evi.ref.ToString).evidence.Clear()
                terms(evi.ref.ToString).evidence.AddRange(evidences)
            Next
        Next

        Return terms
    End Function

    Public Shared Iterator Function GetNetwork(pack As StreamPack, terms As Dictionary(Of String, Knowledge)) As IEnumerable(Of Association)
        Dim linkTypes As IndexByRef = StorageProvider.GetKeywords("meta/associations.msg", pack)
        Dim files As StreamBlock() = pack.files

        For Each item In files.Where(Function(t) t.fullName.StartsWith("graph"))
            Dim list = MsgPackSerializer.Deserialize(Of LinkMsg())(pack.OpenBlock(item))

            For Each l As LinkMsg In list
                Yield New Association With {
                    .type = linkTypes.types(l.type),
                    .U = terms(l.u.ToString),
                    .V = terms(l.v.ToString),
                    .weight = l.weight,
                    .source = l.referenceSources _
                        .Select(Function(i) linkTypes.source(i)) _
                        .AsList
                }
            Next
        Next
    End Function
End Class
