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
            For Each kn As Knowledge In k2.vertex
                Call unionTerms(k1, kn)
            Next

            Call unionNetwork(k1, k2)

            Return k1
        End Function

        Private Sub unionTerms(k1 As GraphModel, kn As Knowledge)
            Dim target As Knowledge

            If k1.ExistVertex(kn.label) Then
                Dim kold = k1.GetElementById(kn.label)

                kold.isMaster = kold.isMaster OrElse kn.isMaster
                kold.source.AddRange(kn.source)
                kold.type = If(kold.mentions > kn.mentions, kold.type, kn.type)
                kold.mentions += kn.mentions

                target = kold

                For Each evi In kn.evidence
                    If kold.evidence.ContainsKey(evi.Key) Then
                        kold.evidence(evi.Key) = kold.evidence(evi.Key) _
                            .JoinIterates(evi.Value) _
                            .Distinct _
                            .ToArray
                    Else
                        kold.evidence.Add(evi.Key, evi.Value)
                    End If
                Next
            Else
                Dim knew = k1.AddVertex(kn.label)

                knew.isMaster = kn.isMaster
                knew.mentions = kn.mentions
                knew.source = kn.source
                knew.type = kn.type
                knew.evidence = kn.evidence

                target = knew
            End If

            If TypeOf k1 Is EvidenceGraph Then
                Call DirectCast(k1, EvidenceGraph).buildEvidenceMapping(target, kn.evidence)
            End If
        End Sub

        Private Sub unionNetwork(k1 As GraphModel, k2 As GraphModel)
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
            Next
        End Sub
    End Module
End Namespace