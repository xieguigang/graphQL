#Region "Microsoft.VisualBasic::11e2c6316770f89718b8e8e948b983a1, src\graphMsg\StreamEmit.vb"

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

    '   Total Lines: 252
    '    Code Lines: 194
    ' Comment Lines: 20
    '   Blank Lines: 38
    '     File Size: 10.45 KB


    ' Class StreamEmit
    ' 
    '     Function: GetKnowledges, GetNetwork, readmeText, Save, SaveNetwork
    '               SaveTerms, splitBlocks
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.DataStorage.HDSPack
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

        Using hdsPack As New StreamPack(file, meta_size:=1024 * 1024)
            ' save graph types
            Call MsgPackSerializer.SerializeObject(termRef, hdsPack.OpenBlock("meta/keywords.msg"), closeFile:=True)
            Call MsgPackSerializer.SerializeObject(linkRef, hdsPack.OpenBlock("meta/associations.msg"), closeFile:=True)
            Call EvidenceStream.Exports(evidenceRef, file:=hdsPack)

            Call SaveTerms(terms, hdsPack) _
                .GetJson _
                .FlushTo(
                    out:=New StreamWriter(hdsPack.OpenBlock("knowledge_blocks.json")),
                    closeFile:=True
                )
            Call SaveEvidence(evidences, hdsPack)

            Call info.Add("graph_blocks", SaveNetwork(links, hdsPack))
            Call info.Add("evidence_link", evidences.Length)
            Call info _
                .GetJson _
                .FlushTo(
                    out:=New StreamWriter(hdsPack.OpenBlock("summary.json")),
                    closeFile:=True
                )

            Call hdsPack.WriteText(
                text:=readmeText(summary:=info),
                fileName:="readme.txt"
            )
        End Using

        Return True
    End Function

    Private Shared Function readmeText(summary As Dictionary(Of String, String)) As String
        Dim sb As New StringBuilder

        Call sb.AppendLine("Graph database storage for no-sql database engine.")
        Call sb.AppendLine("Database summary:")
        Call sb.AppendLine()
        Call sb.AppendLine($"number of knowledge terms: {summary!knowledge_terms}")
        Call sb.AppendLine($"number of graph links: {summary!graph_size}")
        Call sb.AppendLine($"number of knowledge category: {summary!knowledge_types}")
        Call sb.AppendLine($"number of knowledge link category: {summary!link_types}")
        Call sb.AppendLine($"number of knowledge group evidence category: {summary!evidence_types}")
        Call sb.AppendLine($"number of knowledge group evidence terms: {summary!evidence_size}")
        Call sb.AppendLine($"number of knowledge group evidence links: {summary!evidence_link}")
        Call sb.AppendLine($"knowledge graph store in data blocks: {summary!graph_blocks}")

        Return sb.ToString
    End Function

    ''' <summary>
    ''' split terms into multiple data groups
    ''' </summary>
    ''' <param name="terms"></param>
    ''' <returns></returns>
    Private Shared Iterator Function splitBlocks(terms As KnowledgeMsg()) As IEnumerable(Of NamedCollection(Of KnowledgeMsg))
        Dim suffixGroups = terms _
            .GroupBy(Function(t)
                         If t.term.StringEmpty Then
                             Return "-"
                         ElseIf t.term.Length < 3 Then
                             Return t.term
                         Else
                             Return Mid(t.term, 1, 2) & Mid(t.term, t.term.Length - 2, 2)
                         End If
                     End Function)
        Dim md5Groups = suffixGroups.GroupBy(Function(g) g.Key.MD5.Substring(0, 2))

        For Each group In md5Groups
            Yield New NamedCollection(Of KnowledgeMsg) With {
                .name = group.Key,
                .value = group.IteratesALL.ToArray
            }
        Next
    End Function

    ''' <summary>
    ''' save all knowledge terms data 
    ''' </summary>
    ''' <param name="terms"></param>
    ''' <param name="pack"></param>
    ''' <returns></returns>
    Private Shared Function SaveTerms(terms As KnowledgeMsg(), pack As StreamPack) As Dictionary(Of String, Integer)
        Dim buffer As Stream
        Dim summary As New Dictionary(Of String, Integer)
        Dim index As New List(Of TermIndex)
        Dim bin As BinaryDataWriter

        For Each block As NamedCollection(Of KnowledgeMsg) In splitBlocks(terms)
            ' save terms to one data block section
            terms = block.ToArray
            buffer = pack.OpenBlock($"terms/{block.name}.dat")
            bin = New BinaryDataWriter(buffer)

            For Each term As KnowledgeMsg In terms
                Dim ms As Byte() = MsgPackSerializer.SerializeObject(term)
                Dim offset As Integer = bin.Position

                Call bin.Write(ms.Length)
                Call bin.Write(ms)
                Call bin.Flush()

                Call index.Add(New TermIndex With {.block = block.name, .offset = offset, .term = term.term})
            Next

            Call buffer.Flush()
            Call buffer.Dispose()
            Call summary.Add(block.name, terms.Length)
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

        For Each item As StreamBlock In files.Where(Function(t) t.fullName.StartsWith("/terms"))
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

        Call terms.LoadEvidence(pack)

        Return terms
    End Function

    Public Shared Iterator Function GetNetwork(pack As StreamPack, terms As Dictionary(Of String, Knowledge)) As IEnumerable(Of Association)
        Dim linkTypes As IndexByRef = StorageProvider.GetKeywords("meta/associations.msg", pack)
        Dim files As StreamBlock() = pack.files

        For Each item In files.Where(Function(t) t.fullName.StartsWith("/graph"))
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
