Imports System.IO
Imports graphMsg
Imports graphQL
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("MsgFile")>
Module MsgFile

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
