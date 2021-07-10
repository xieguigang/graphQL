Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Development.VisualStudio
Imports Microsoft.VisualBasic.ApplicationServices.Development.VisualStudio.vbproj
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.NLP
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Public Module VBGraph

    Public Function LoadProject(vbproj As String) As String()
        Dim proj As Project = vbproj.LoadXml(Of Project)
        Dim vb As String() = proj _
            .EnumerateSourceFiles(skipAssmInfo:=True) _
            .ToArray
        Dim dir As String = vbproj.ParentPath

        vb = vb _
            .Select(Function(filename)
                        Return $"{dir}/{filename}".GetFullPath
                    End Function) _
            .ToArray

        Return vb
    End Function

    Public Function SourceTokens(file As String, Optional maxChars As Integer = 24) As String()
        Return file _
            .SolveStream _
            .ToLower _
            .Where(Function(c)
                       Dim b As Integer = AscW(c)

                       Return b > 0 AndAlso b <= 255
                   End Function) _
            .CharString _
            .Split(" "c, ASCII.CR, ASCII.LF, ASCII.TAB, """"c, "'"c, "`"c, "~"c,
                   "+"c, "-"c, "*"c, "/"c, "("c, ")"c, "["c, "]"c, "?"c,
                   "."c, "<"c, ">"c, ","c, "!"c, ":"c, "="c, "{"c, "}"c,
                   "#"c, "\"c, "&"c, "^"c, "%"c, "$"c, "@"c, ";"c,
                   "_"c, "|"c, "·"c, "©"c, "°"c, "÷"c, "×"c
             ) _
            .Where(Function(s) Not s.StringEmpty) _
            .Where(Function(s) Not s.IsPattern("\d+")) _
            .Where(Function(s) s.Length < maxChars) _
            .DoCall(AddressOf StopWords.DefaultStopWords.DefaultValue.Removes) _
            .ToArray
    End Function

    <Extension>
    Public Function GetNetwork(g As Graph, Optional cutoff As Double = 0.3) As NetworkGraph
        Dim net As New NetworkGraph
        Dim w As DoubleRange = g.graphEdges _
            .Select(Function(d) d.weight) _
            .ToArray
        Dim threshold As Double = w.Max * cutoff

        For Each node In g.vertex
            net.CreateNode(node.label)
        Next
        For Each link In g.graphEdges.Where(Function(d) d.weight >= threshold)
            net.CreateEdge(link.U.label, link.V.label)
        Next

        Return net
    End Function
End Module
