Imports System.IO
Imports System.IO.Compression
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq

Public Class StorageProvider

    <MessagePackMember(0)> Public Property terms As KnowledgeMsg()
    <MessagePackMember(1)> Public Property links As LinkMsg()

    ''' <summary>
    ''' save as zip file
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
            Return CreateQuery(Of T)(New ZipArchive(file, ZipArchiveMode.Read), noGraph)
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
    Public Shared Function CreateQuery(Of T As GraphModel)(pack As ZipArchive, Optional noGraph As Boolean = False) As GraphModel
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
            evidences = EvidenceStream.Load(zip:=pack)
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
