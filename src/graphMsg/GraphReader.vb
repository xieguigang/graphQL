#Region "Microsoft.VisualBasic::8998429d40990c6b277fe619333c2fd5, G:/graphQL/src/graphMsg//GraphReader.vb"

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

    '   Total Lines: 52
    '    Code Lines: 40
    ' Comment Lines: 0
    '   Blank Lines: 12
    '     File Size: 1.59 KB


    ' Module GraphReader
    ' 
    '     Function: GetEdgeSource, (+2 Overloads) LoadGraph
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports graphMsg.Message
Imports graphQL
Imports graphQL.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Public Module GraphReader

    Public Function LoadGraph(file As Stream) As NetworkGraph
        Using hds As New StreamPack(file)
            Return LoadGraph(hds)
        End Using
    End Function

    Public Function GetEdgeSource(file As Stream) As String()
        Using hds As New StreamPack(file)
            Dim linkTypes As IndexByRef = StorageProvider.GetKeywords("meta/associations.msg", hds)
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

