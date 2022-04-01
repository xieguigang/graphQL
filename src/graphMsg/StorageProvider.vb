Imports System.IO
Imports System.IO.Compression
Imports graphMsg.Message
Imports graphQL
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Public Class StorageProvider

    <MessagePackMember(0)> Public Property terms As KnowledgeMsg()
    <MessagePackMember(1)> Public Property links As LinkMsg()

    ''' <summary>
    ''' save as zip file
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function Save(kb As GraphPool, file As Stream) As Boolean
        Return StreamEmit.Save(kb, file)
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
            Return StreamEmit.GetKnowledges(zip).Values.ToArray
        End Using
    End Function

    ''' <summary>
    ''' load knowledge database
    ''' </summary>
    ''' <param name="pack"></param>
    ''' <returns></returns>
    Public Shared Function CreateQuery(pack As ZipArchive) As GraphPool
        Dim terms As Dictionary(Of String, Knowledge) = StreamEmit.GetKnowledges(pack)
        Dim links As Association() = StreamEmit.GetNetwork(pack, terms).ToArray

        Call pack.Dispose()

        Return New GraphPool(terms.Values, links)
    End Function

End Class
