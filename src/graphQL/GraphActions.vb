Imports System.Runtime.CompilerServices

Public Module GraphActions

    <Extension>
    Public Function JoinGraph(k1 As GraphPool, k2 As GraphPool) As GraphPool
        For Each kn As Knowledge In k2.vertex
            If k1.ExistVertex(kn.label) Then
                Dim kold = k1.GetElementById(kn.label)

                kold.isMaster = kold.isMaster OrElse kn.isMaster
                kold.source.AddRange(kn.source)
                kold.type = If(kold.mentions > kn.mentions, kold.type, kn.type)
                kold.mentions += kn.mentions
            Else
                Dim knew = k1.AddVertex(kn.label)

                knew.isMaster = kn.isMaster
                knew.mentions = kn.mentions
                knew.source = kn.source
                knew.type = kn.type
            End If
        Next

        For Each ln As Association In k2.graphEdges
            Dim ref As String = $"{ln.U.label}+{ln.V.label}"

            If k1.ExistEdge(ln) Then
                Dim edge As Association = k1.QueryEdge(ref)

                edge.type = If(edge.weight > ln.weight, edge.type, ln.type)
                edge.weight += ln.weight
                edge.source.AddRange(ln.source)
            Else
                k1.AddEdge(
                    u:=k1.GetElementById(ln.U.label),
                    v:=k1.GetElementById(ln.V.label),
                    weight:=ln.weight
                )

                Dim edge As Association = k1.QueryEdge(ref)

                edge.type = ln.type
                edge.source = ln.source
            End If
        Next

        Return k1
    End Function

End Module
