Imports System.IO
Imports System.IO.Compression
Imports graphQL
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Class StorageProvider

    <MessagePackMember(0)> Public Property terms As KnowledgeMsg()
    <MessagePackMember(1)> Public Property links As LinkMsg()

    Private Shared Sub SaveTerms(terms As KnowledgeMsg(), zip As ZipArchive)
        Dim blocks = terms _
            .GroupBy(Function(t) Mid(t.term, 1, 3)) _
            .GroupBy(Function(g) g.Key.MD5.Substring(0, 2)) _
            .ToArray
        Dim buffer As Stream

        For Each block In blocks
            terms = block.IteratesALL.ToArray
            buffer = zip.CreateEntry($"terms/{block.Key}.dat").Open

            Call MsgPackSerializer.SerializeObject(terms, buffer, closeFile:=True)
        Next
    End Sub

    Private Shared Sub SaveNetwork(links As LinkMsg(), zip As ZipArchive)
        Dim blocks = links.OrderByDescending(Function(d) d.weight).Split(4096)
        Dim buffer As Stream
        Dim i As Integer = 0

        For Each block In blocks
            i += 1
            buffer = zip.CreateEntry($"graph/{i.FormatZero("000")}").Open

            Call MsgPackSerializer.SerializeObject(block, buffer, closeFile:=True)
        Next
    End Sub

    ''' <summary>
    ''' save as zip file
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function Save(kb As GraphPool, file As Stream) As Boolean
        Dim termTypes As New List(Of String)
        Dim linkTypes As New List(Of String)
        Dim terms = KnowledgeMsg.GetTerms(kb, termTypes).ToArray
        Dim links = LinkMsg.GetRelationships(kb, linkTypes).ToArray

        Using zip As New ZipArchive(file, ZipArchiveMode.Create, leaveOpen:=False)
            ' save graph types
            Call MsgPackSerializer.SerializeObject(termTypes.ToArray, zip.CreateEntry("meta/keywords.msg").Open, closeFile:=True)
            Call MsgPackSerializer.SerializeObject(linkTypes.ToArray, zip.CreateEntry("meta/associations.msg").Open, closeFile:=True)

            Call SaveTerms(terms, zip)
            Call SaveNetwork(links, zip)
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

    Public Shared Function GetKeywords(res As String, pack As ZipArchive) As String()
        Return MsgPackSerializer.Deserialize(Of String())(pack.GetEntry(res).Open)
    End Function

    Public Shared Function GetKnowledges(pack As ZipArchive) As Dictionary(Of String, Knowledge)
        Dim terms As New Dictionary(Of String, Knowledge)
        Dim termTypes As String() = GetKeywords("meta/keywords.msg", pack)
        Dim files = pack.Entries

        For Each item In files.Where(Function(t) t.FullName.StartsWith("terms"))
            Dim list = MsgPackSerializer.Deserialize(Of KnowledgeMsg())(item.Open)

            For Each v As KnowledgeMsg In list
                terms(v.guid.ToString) = New Knowledge With {
                    .ID = v.guid,
                    .label = v.term,
                    .mentions = v.mentions,
                    .type = termTypes(v.type)
                }
            Next
        Next

        Return terms
    End Function

    Public Shared Iterator Function GetNetwork(pack As ZipArchive, terms As Dictionary(Of String, Knowledge)) As IEnumerable(Of Association)
        Dim linkTypes As String() = GetKeywords("meta/associations.msg", pack)
        Dim files = pack.Entries

        For Each item In files.Where(Function(t) t.FullName.StartsWith("graph"))
            Dim list = MsgPackSerializer.Deserialize(Of LinkMsg())(item.Open)

            For Each l As LinkMsg In list
                Yield New Association With {
                    .type = linkTypes(l.type),
                    .U = terms(l.u.ToString),
                    .V = terms(l.v.ToString),
                    .weight = l.weight
                }
            Next
        Next
    End Function

    Public Shared Function CreateQuery(pack As ZipArchive) As GraphPool
        Dim terms As Dictionary(Of String, Knowledge) = GetKnowledges(pack)
        Dim links As Association() = GetNetwork(pack, terms).ToArray

        Call pack.Dispose()

        Return New GraphPool(terms.Values, links)
    End Function

End Class
