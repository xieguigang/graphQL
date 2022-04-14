Imports System.IO
Imports graphMsg
Imports graphQL.Graph
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

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
            ElseIf evidenceAggregate Then
                Return StorageProvider.Open(Of EvidenceGraph)(buffer)
            Else
                Return StorageProvider.Open(Of GraphPool)(buffer)
            End If
        End If
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
            Return Internal.debug.stop("the required file resource to save data can not be nothing!", env)
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
