Imports System.IO
Imports System.IO.Compression
Imports graphMsg.Message
Imports graphQL
Imports graphQL.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Public Module GraphReader

    Public Function LoadGraph(file As Stream) As NetworkGraph
        Using zip As New StreamPack(file)
            Return LoadGraph(zip)
        End Using
    End Function

    Public Function GetEdgeSource(file As Stream) As String()
        Using zip As New StreamPack(file)
            Dim linkTypes As IndexByRef = StorageProvider.GetKeywords("meta/associations.msg", zip)
            Dim sources As String() = linkTypes.source

            Return sources
        End Using
    End Function

    Public Function LoadGraph(pack As StreamPack) As NetworkGraph
        Dim terms As Dictionary(Of String, Knowledge) = StreamEmit.GetKnowledges(pack)
        Dim evidences As EvidencePool

        If terms.Values.Any(Function(i) i.evidence.Count > 0) Then
            evidences = EvidenceStream.Load(pack:=pack)
        Else
            evidences = EvidencePool.Empty
        End If

        Dim nodeTable = terms.Values.LoadNodeTable(evidences)
        Dim g As New NetworkGraph

        For Each node As Node In nodeTable.Values
            Call g.AddNode(node, assignId:=False)
        Next

        For Each link As Edge In StreamEmit _
            .GetNetwork(pack, terms) _
            .AssembleLinks(nodeTable)

            Call g.AddEdge(link)
        Next

        Return g
    End Function

End Module
