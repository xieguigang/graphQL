Imports codegraph
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.Analysis.PageRank
Imports Microsoft.VisualBasic.Data.NLP
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Module Module1

    Sub Main()
        Dim vb = VBGraph.LoadProject("E:\GCModeller\src\runtime\sciBASIC#\Microsoft.VisualBasic.Core\src\47-dotnet_Microsoft.VisualBasic.vbproj")
        Dim g As New Graph

        For Each source As String In vb
            Call g.TextRankGraph(VBGraph.SourceTokens(source))
            Call Console.WriteLine(source)
        Next

        Dim text As New GraphMatrix(g)
        Dim pr As New PageRank(text)
        Dim result = text.TranslateVector(pr.ComputePageRank, True)

        Dim net As NetworkGraph = g.GetNetwork(cutoff:=0)

        For Each node In net.vertex
            node.data.Add("PageRank", result(node.label))
        Next

        Call net.Tabular(propertyNames:={"PageRank"}).Save(App.HOME)

        Pause()
    End Sub


End Module
