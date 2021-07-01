Imports Microsoft.VisualBasic.ApplicationServices.Development.VisualStudio
Imports Microsoft.VisualBasic.ApplicationServices.Development.VisualStudio.vbproj
Imports Microsoft.VisualBasic.Data.NLP
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

    Public Function SourceTokens(file As String) As String()
        Return file _
            .SolveStream _
            .Split(" "c, ASCII.CR, ASCII.LF, ASCII.TAB, """"c, "'"c,
                   "+"c, "-"c, "*"c, "/"c, "("c, ")"c, "["c, "]"c, "?"c,
                   "."c, "<"c, ">"c, ","c, "!"c, ":"c, "="c, "{"c, "}"c,
                   "#"c, "\"c, "&"c, "^"c, "%"c, "$"c, "@"c
             ) _
            .Where(Function(s) Not s.StringEmpty) _
            .DoCall(AddressOf StopWords.DefaultStopWords.DefaultValue.Removes) _
            .ToArray
    End Function

End Module
