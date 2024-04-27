#Region "Microsoft.VisualBasic::130204f0f8ca19f152dde513b8cb391a, G:/graphQL/src/graphR//networkGraph/umapKnowledge.vb"

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

    '   Total Lines: 55
    '    Code Lines: 41
    ' Comment Lines: 0
    '   Blank Lines: 14
    '     File Size: 1.84 KB


    ' Module umapKnowledge
    ' 
    '     Function: RunUMAP, toFullMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.UMAP

Module umapKnowledge

    <Extension>
    Private Function toFullMatrix(g As NetworkGraph, ByRef labels As String()) As Double()()
        Dim nodes As Index(Of String) = g.vertex.Select(Function(v) v.label).Indexing
        Dim mat As Double()() = New Double(nodes.Count - 1)() {}

        For i As Integer = 0 To mat.Length - 1
            mat(i) = New Double(mat.Length - 1) {}
        Next

        For Each edge As Edge In g.graphEdges
            Dim i As Integer = nodes.IndexOf(edge.U.label)
            Dim j As Integer = nodes.IndexOf(edge.V.label)

            mat(i)(j) += edge.weight
            mat(j)(i) += edge.weight
        Next

        labels = nodes.Objects

        Return mat
    End Function

    <Extension>
    Public Function RunUMAP(g As NetworkGraph, ByRef labels As String()) As Umap
        Dim umap As New Umap(dimensions:=3, progressReporter:=AddressOf RunSlavePipeline.SendProgress)
        Dim nEpochs As Integer
        Dim matrix As Double()() = g.toFullMatrix(labels)

        Call Console.WriteLine("Initialize fit..")

        nEpochs = umap.InitializeFit(matrix)

        Console.WriteLine("- Done")
        Console.WriteLine()
        Console.WriteLine("Calculating..")

        For i As Integer = 0 To nEpochs - 1
            Call umap.Step()

            If (100 * i / nEpochs) Mod 5 = 0 Then
                Console.WriteLine($"- Completed {i + 1} of {nEpochs} [{CInt(100 * i / nEpochs)}%]")
            End If
        Next

        Return umap
    End Function
End Module

