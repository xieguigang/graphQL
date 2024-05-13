#Region "Microsoft.VisualBasic::9f87ff3a8f756ff0ecaa09a70cc0edbb, src\graphMsg\StorageProvider.vb"

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

    '   Total Lines: 87
    '    Code Lines: 62
    ' Comment Lines: 11
    '   Blank Lines: 14
    '     File Size: 3.24 KB


    ' Class StorageProvider
    ' 
    '     Properties: links, terms
    ' 
    '     Function: CreateQuery, GetKeywords, GetKnowledges, Open, Save
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.IO.Compression
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq

Public Class StorageProvider

    <MessagePackMember(0)> Public Property terms As KnowledgeMsg()
    <MessagePackMember(1)> Public Property links As LinkMsg()

    ''' <summary>
    ''' save as HDS file
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function Save(kb As GraphModel, file As Stream) As Boolean
        Return StreamEmit.Save(kb, file)
    End Function

    Public Shared Function Open(Of T As GraphModel)(file As Stream, Optional noGraph As Boolean = False) As GraphModel
        If file Is Nothing Then
            If GetType(T) Is GetType(GraphPool) Then
                Return New GraphPool({}, {})
            ElseIf GetType(T) Is GetType(EvidenceGraph) Then
                Return New EvidenceGraph({}, {}, EvidencePool.Empty)
            Else
                Throw New NotImplementedException(GetType(T).FullName)
            End If
        Else
            Return CreateQuery(Of T)(New StreamPack(file), noGraph)
        End If
    End Function

    Public Shared Function GetKeywords(res As String, pack As StreamPack) As IndexByRef
        Return MsgPackSerializer.Deserialize(Of IndexByRef)(pack.OpenBlock(res))
    End Function

    Public Shared Function GetKnowledges(file As Stream) As Knowledge()
        Using pack As New StreamPack(file)
            Return StreamEmit.GetKnowledges(pack).Values.ToArray
        End Using
    End Function

    ''' <summary>
    ''' load knowledge database
    ''' </summary>
    ''' <param name="pack"></param>
    ''' <returns></returns>
    Public Shared Function CreateQuery(Of T As GraphModel)(pack As StreamPack, Optional noGraph As Boolean = False) As GraphModel
        Dim evidences As EvidencePool = Nothing
        Call Console.WriteLine("start to loading knowledge data...")
        Dim terms As Dictionary(Of String, Knowledge) = StreamEmit.GetKnowledges(pack)
        Call Console.WriteLine($"get {terms.Count} knowledge nodes!")
        Call Console.WriteLine("loading network graph...")

        Dim links As Association() = {}

        If Not noGraph Then
            links = StreamEmit _
                .GetNetwork(pack, terms) _
                .ToArray
        End If

        Call Console.WriteLine($"get {links.Length} graph links!")

        If GetType(T) Is GetType(EvidenceGraph) Then
            evidences = EvidenceStream.Load(pack:=pack)
        End If

        Call pack.Dispose()
        Call Console.WriteLine("build knowledge graph!")

        If GetType(T) Is GetType(GraphPool) Then
            Return New GraphPool(terms.Values, links)
        ElseIf GetType(T) Is GetType(EvidenceGraph) Then
            Return New EvidenceGraph(terms.Values, links, evidences)
        Else
            Throw New NotImplementedException
        End If
    End Function

End Class
