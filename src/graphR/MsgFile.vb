#Region "Microsoft.VisualBasic::fa7f15a490fe1a33984fa160e29ce303, src\graphR\MsgFile.vb"

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

    '   Total Lines: 163
    '    Code Lines: 114
    ' Comment Lines: 30
    '   Blank Lines: 19
    '     File Size: 6.46 KB


    ' Module MsgFile
    ' 
    '     Function: getEdgeConnectionSource, getKnowledgeTable, getTerms, open, readGraph
    '               save, seekTerm
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports graphMsg
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' the graph database file I/O handler
''' </summary>
<Package("MsgFile")>
Module MsgFile

    ''' <summary>
    ''' open a message pack graph database file or 
    ''' create a new empty graph database object.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("open")>
    <RApiReturn(GetType(GraphPool), GetType(EvidenceGraph))>
    Public Function open(<RRawVectorArgument>
                         Optional file As Object = Nothing,
                         Optional evidenceAggregate As Boolean = False,
                         Optional noGraph As Boolean = False,
                         Optional seekIndex As Boolean = False,
                         Optional env As Environment = Nothing) As Object

        If file Is Nothing Then
            If evidenceAggregate Then
                Return New EvidenceGraph({}, {}, EvidencePool.Empty)
            Else
                Return New GraphPool({}, {})
            End If
        Else
            Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

            If buffer Like GetType(Message) Then
                Return buffer.TryCast(Of Message)
            ElseIf seekIndex Then
                Dim pack As New StreamPack(buffer.TryCast(Of Stream))
                Dim indexBuffer = pack.OpenBlock("/index.dat")
                Dim index As TermIndexMsg = MsgPackSerializer.Deserialize(GetType(TermIndexMsg), indexBuffer)
                Dim seek As New SeekIndex(pack) With {.index = index.ToList}

                Return seek
            ElseIf evidenceAggregate Then
                Return StorageProvider.Open(Of EvidenceGraph)(buffer, noGraph)
            Else
                Return StorageProvider.Open(Of GraphPool)(buffer, noGraph)
            End If
        End If
    End Function

    <ExportAPI("seekTerm")>
    Public Function seekTerm(index As SeekIndex, term As String) As Object
        Return index.SeekTerm(term)
    End Function

    <ExportAPI("getTerms")>
    Public Function getTerms(index As SeekIndex) As String()
        Return index.index.Keys.ToArray
    End Function

    ''' <summary>
    ''' read target graph database as network graph object
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.graph")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function readGraph(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        Else
            Return GraphReader.LoadGraph(buffer)
        End If
    End Function

    <ExportAPI("edgeSource")>
    Public Function getEdgeConnectionSource(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        Else
            Return GraphReader.GetEdgeSource(buffer)
        End If
    End Function

    ''' <summary>
    ''' fetch the knowledge terms table from the graph database file.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.knowledge_table")>
    <RApiReturn(GetType(dataframe))>
    Public Function getKnowledgeTable(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        Else
            Dim terms = StorageProvider.GetKnowledges(buffer)
            Dim table As New dataframe With {
                .columns = New Dictionary(Of String, Array)
            }

            Call table.add(NameOf(Knowledge.ID), terms.Select(Function(t) t.ID).ToArray)
            Call table.add(NameOf(Knowledge.label), terms.Select(Function(t) t.label).ToArray)
            Call table.add(NameOf(Knowledge.type), terms.Select(Function(t) t.type).ToArray)
            Call table.add(NameOf(Knowledge.mentions), terms.Select(Function(t) t.mentions).ToArray)
            Call table.add(NameOf(Knowledge.isMaster), terms.Select(Function(t) t.isMaster).ToArray)
            Call table.add(NameOf(Knowledge.source), terms.Select(Function(t) t.source.JoinBy("; ")).ToArray)

            Return table
        End If
    End Function

    ''' <summary>
    ''' save a graph database result into a file 
    ''' in messagepack format.
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("save")>
    Public Function save(kb As GraphModel,
                         file As Object,
                         Optional json_dump As Boolean = False,
                         Optional env As Environment = Nothing) As Object

        If file Is Nothing Then
            Return RInternal.debug.stop("the required file resource to save data can not be nothing!", env)
        Else
            Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

            If buffer Like GetType(Message) Then
                Return buffer.TryCast(Of Message)
            Else
                If json_dump Then
                    Return JSONDump.WriteJSON(kb, buffer.TryCast(Of Stream))
                Else
                    Return StorageProvider.Save(kb, buffer)
                End If
            End If
        End If
    End Function

End Module
