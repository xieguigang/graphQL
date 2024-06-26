﻿#Region "Microsoft.VisualBasic::2813208437eb28f96a20508ed529e752, src\graphQL\Graph\GraphActions.vb"

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

    '   Total Lines: 128
    '    Code Lines: 88
    ' Comment Lines: 17
    '   Blank Lines: 23
    '     File Size: 4.57 KB


    '     Module GraphActions
    ' 
    '         Function: JoinGraph
    ' 
    '         Sub: unionNetwork, unionTerms
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Graph

    Public Module GraphActions

        ''' <summary>
        ''' merge graph <paramref name="k2"/> into graph <paramref name="k1"/>.
        ''' </summary>
        ''' <param name="k1"></param>
        ''' <param name="k2"></param>
        ''' <returns></returns>
        <Extension>
        Public Function JoinGraph(k1 As GraphModel, k2 As GraphModel) As GraphModel
            Dim evidencePool As EvidencePool
            Dim nsize As Integer = k2.size.vertex
            Dim delta As Integer = nsize / 20
            Dim i As Integer = Scan0
            Dim p As Integer = Scan0

            If TypeOf k1 Is EvidenceGraph AndAlso TypeOf k2 Is EvidenceGraph Then
                evidencePool = DirectCast(k2, EvidenceGraph).evidences
            Else
                evidencePool = New EvidencePool({}, {})
            End If

            For Each kn As Knowledge In k2.vertex
                Call unionTerms(k1, kn, evidencePool)

                p += 1
                i += 1

                If i = delta Then
                    i = 0
                    Call Console.WriteLine($"join_knowledge_terms: {CInt(p / nsize * 100)}%")
                End If
            Next

            Call unionNetwork(k1, k2)
            Call Console.WriteLine("~done")

            Return k1
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="k1"></param>
        ''' <param name="kn"></param>
        ''' <param name="evidencePool">
        ''' evidence data source of the k2 graph
        ''' </param>
        Private Sub unionTerms(k1 As GraphModel, kn As Knowledge, evidencePool As EvidencePool)
            Dim target As Knowledge
            Dim evidences = evidencePool.LoadEvidenceData(kn.evidence)

            If k1.ExistVertex(kn.label) Then
                Dim kold = k1.GetElementById(kn.label)

                kold.isMaster = kold.isMaster OrElse kn.isMaster
                kold.source.AddRange(kn.source)
                kold.type = If(kold.mentions > kn.mentions, kold.type, kn.type)
                kold.mentions += kn.mentions

                target = kold
            Else
                Dim knew = k1.AddVertex(kn.label)

                knew.isMaster = kn.isMaster
                knew.mentions = kn.mentions
                knew.source = kn.source
                knew.type = kn.type

                target = knew
            End If

            If TypeOf k1 Is EvidenceGraph Then
                ' merge evidence data,
                ' and then create new mapping between
                ' the evidence and target term
                Call DirectCast(k1, EvidenceGraph).JoinEvidence(target, evidences)
                Call DirectCast(k1, EvidenceGraph).buildEvidenceMapping(target)
                Call DirectCast(k1, EvidenceGraph).buildEvidenceGraph(
                    term:=target,
                    evidence:=evidencePool.LoadEvidenceData(kn.evidence).ToArray,
                    selfReference:=False
                )
            End If
        End Sub

        Private Sub unionNetwork(k1 As GraphModel, k2 As GraphModel)
            Dim nsize As Integer = k2.size.edges
            Dim delta As Integer = nsize / 20
            Dim i As Integer = Scan0
            Dim p As Integer = Scan0

            For Each ln As Association In k2.graphEdges
                If k1.ExistEdge(ln) Then
                    Dim edge As Association = k1.QueryEdge(ln.U.label, ln.V.label)

                    edge.type = If(edge.weight > ln.weight, edge.type, ln.type)
                    edge.weight += ln.weight
                    edge.source.AddRange(ln.source)
                Else
                    k1.AddEdge(
                        u:=k1.GetElementById(ln.U.label),
                        v:=k1.GetElementById(ln.V.label),
                        weight:=ln.weight
                    )

                    Dim edge As Association = k1.QueryEdge(ln.U.label, ln.V.label)

                    edge.type = ln.type
                    edge.source = ln.source
                End If

                p += 1
                i += 1

                If i = delta Then
                    i = 0
                    Call Console.WriteLine($"join_knowledge_graph: {CInt(p / nsize * 100)}%")
                End If
            Next
        End Sub
    End Module
End Namespace
