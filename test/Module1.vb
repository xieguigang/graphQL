Imports codegraph

Module Module1

    Sub Main()
        Dim vb = VBGraph.LoadProject("E:\GCModeller\src\runtime\sciBASIC#\Microsoft.VisualBasic.Core\src\47-dotnet_Microsoft.VisualBasic.vbproj")
        Dim tokens As String() = VBGraph.SourceTokens(vb(Scan0))

        Pause()
    End Sub

End Module
