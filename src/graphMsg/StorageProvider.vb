Imports System.IO
Imports System.IO.Compression
Imports graphMsg.Message
Imports graphQL
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class StorageProvider

    <MessagePackMember(0)> Public Property terms As KnowledgeMsg()
    <MessagePackMember(1)> Public Property links As LinkMsg()

    Private Shared Function SaveTerms(terms As KnowledgeMsg(), zip As ZipArchive) As Dictionary(Of String, Integer)
        Dim blocks = terms _
            .GroupBy(Function(t) Mid(t.term, 1, 3)) _
            .Where(Function(g) g.Key <> "") _
            .GroupBy(Function(g) g.Key.MD5.Substring(0, 2)) _
            .ToArray
        Dim buffer As Stream
        Dim summary As New Dictionary(Of String, Integer)

        For Each block In blocks
            terms = block.IteratesALL.ToArray
            buffer = zip.CreateEntry($"terms/{block.Key}.dat").Open

            Call MsgPackSerializer.SerializeObject(terms, buffer, closeFile:=True)
            Call summary.Add(block.Key, terms.Length)
        Next

        Return summary
    End Function

    Private Shared Function SaveNetwork(links As LinkMsg(), zip As ZipArchive) As Integer
        Dim blocks = links.OrderByDescending(Function(d) d.weight).Split(4096)
        Dim buffer As Stream
        Dim i As Integer = 0

        For Each block In blocks
            i += 1
            buffer = zip.CreateEntry($"graph/{i.FormatZero("000")}").Open

            Call MsgPackSerializer.SerializeObject(block, buffer, closeFile:=True)
        Next

        Return blocks.Length
    End Function

    ''' <summary>
    ''' save as zip file
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function Save(kb As GraphPool, file As Stream) As Boolean
        Dim termRef As New IndexByRef
        Dim linkRef As New IndexByRef
        Dim terms = KnowledgeMsg.GetTerms(kb, termRef).ToArray
        Dim links = LinkMsg.GetRelationships(kb, linkRef).ToArray
        Dim info As New Dictionary(Of String, String)

        Call info.Add("knowledge_terms", terms.Length)
        Call info.Add("graph_size", links.Length)
        Call info.Add("knowledge_types", termRef.types.Length)
        Call info.Add("link_types", links.Count)

        Using zip As New ZipArchive(file, ZipArchiveMode.Create, leaveOpen:=False)
            ' save graph types
            Call MsgPackSerializer.SerializeObject(termRef, zip.CreateEntry("meta/keywords.msg").Open, closeFile:=True)
            Call MsgPackSerializer.SerializeObject(linkRef, zip.CreateEntry("meta/associations.msg").Open, closeFile:=True)

            Call SaveTerms(terms, zip) _
                .GetJson _
                .FlushTo(
                    out:=New StreamWriter(zip.CreateEntry("knowledge_blocks.json").Open),
                    closeFile:=True
                )

            Call info.Add("graph_blocks", SaveNetwork(links, zip))
            Call info _
                .GetJson _
                .FlushTo(
                    out:=New StreamWriter(zip.CreateEntry("summary.json").Open),
                    closeFile:=True
                )
        End Using

        Return True
    End Function

    Public Shared Function Open(file As Stream) As GraphPool
        If file Is Nothing Then
            Return New GraphPool({}, {})
        Else
            Return CreateQuery(New ZipArchive(file, ZipArchiveMode.Read))
        End If
    End Function

    Public Shared Function GetKeywords(res As String, pack As ZipArchive) As IndexByRef
        Return MsgPackSerializer.Deserialize(Of IndexByRef)(pack.GetEntry(res).Open)
    End Function

    Public Shared Function GetKnowledges(file As Stream) As Knowledge()
        Using zip As New ZipArchive(file, ZipArchiveMode.Read, leaveOpen:=False)
            Return GetKnowledges(zip).Values.ToArray
        End Using
    End Function

    Public Shared Function GetKnowledges(pack As ZipArchive) As Dictionary(Of String, Knowledge)
        Dim terms As New Dictionary(Of String, Knowledge)
        Dim termTypes As IndexByRef = GetKeywords("meta/keywords.msg", pack)
        Dim files = pack.Entries

        For Each item In files.Where(Function(t) t.FullName.StartsWith("terms"))
            Dim list = MsgPackSerializer.Deserialize(Of KnowledgeMsg())(item.Open)

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

        Return terms
    End Function

    Public Shared Iterator Function GetNetwork(pack As ZipArchive, terms As Dictionary(Of String, Knowledge)) As IEnumerable(Of Association)
        Dim linkTypes As IndexByRef = GetKeywords("meta/associations.msg", pack)
        Dim files = pack.Entries

        For Each item In files.Where(Function(t) t.FullName.StartsWith("graph"))
            Dim list = MsgPackSerializer.Deserialize(Of LinkMsg())(item.Open)

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

    ''' <summary>
    ''' load knowledge database
    ''' </summary>
    ''' <param name="pack"></param>
    ''' <returns></returns>
    Public Shared Function CreateQuery(pack As ZipArchive) As GraphPool
        Dim terms As Dictionary(Of String, Knowledge) = GetKnowledges(pack)
        Dim links As Association() = GetNetwork(pack, terms).ToArray

        Call pack.Dispose()

        Return New GraphPool(terms.Values, links)
    End Function

End Class
