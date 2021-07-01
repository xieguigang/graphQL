Imports codegraph
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.PageRank
Imports Microsoft.VisualBasic.Data.NLP

Module Module1

    Sub Main()
        Dim vb = VBGraph.LoadProject("E:\GCModeller\src\runtime\sciBASIC#\Microsoft.VisualBasic.Core\src\47-dotnet_Microsoft.VisualBasic.vbproj")
        Dim tokens As String() = VBGraph.SourceTokens(vb(Scan0))
        Dim g As New Graph

        Call g.TextRankGraph(tokens)

        Dim text As New GraphMatrix(g)
        Dim pr As New PageRank(text)
        Dim result = text.TranslateVector(pr.ComputePageRank, True)

        Dim net = g.GetNetwork

        For Each node In net.Nodes
            node.Properties.Add("PageRank", result(node.ID))
        Next

        Call net.Save("G:\GCModeller\src\runtime\sciBASIC#\Data\TextRank\")

        Pause()
    End Sub

End Module
