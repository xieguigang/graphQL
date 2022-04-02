Imports System.IO
Imports System.IO.Compression
Imports graphQL
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public Module GraphReader

    Public Function LoadGraph(file As Stream) As NetworkGraph
        Using zip As New ZipArchive(file, ZipArchiveMode.Read)
            Return LoadGraph(zip)
        End Using
    End Function

    Public Function LoadGraph(pack As ZipArchive) As NetworkGraph
        Dim terms As Dictionary(Of String, Knowledge) = StreamEmit.GetKnowledges(pack)
        Dim nodeTable = terms.Values.LoadNodeTable
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
