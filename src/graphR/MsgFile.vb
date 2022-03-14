Imports System.IO
Imports graphMsg
Imports graphQL
Imports Microsoft.VisualBasic.CommandLine.Reflection
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
    Public Function open(<RRawVectorArgument>
                         Optional file As Object = Nothing,
                         Optional env As Environment = Nothing) As Object

        If file Is Nothing Then
            Return New GraphPool({}, {})
        Else
            Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

            If buffer Like GetType(Message) Then
                Return buffer.TryCast(Of Message)
            Else
                Return StorageProvider.Open(buffer)
            End If
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
    Public Function save(kb As GraphPool, file As Object, Optional env As Environment = Nothing) As Object
        If file Is Nothing Then
            Return Internal.debug.stop("the required file resource to save data can not be nothing!", env)
        Else
            Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)

            If buffer Like GetType(Message) Then
                Return buffer.TryCast(Of Message)
            Else
                Return StorageProvider.Save(kb, buffer)
            End If
        End If
    End Function

End Module
