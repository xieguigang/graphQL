Imports System.IO
Imports System.Runtime.CompilerServices
Imports graphQL
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module JSONDump

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="file">target file stream to write json string data.</param>
    ''' <returns></returns>
    Public Function WriteJSON(kb As GraphPool, file As Stream) As Boolean
        Using writer As New StreamWriter(file)
            Dim tmpKnowledges As String = TempFileSystem.GetAppSysTempFile()
            Dim tmpLinks As String = TempFileSystem.GetAppSysTempFile()
            Dim knowledges As IndexByRef
            Dim links As IndexByRef

            Using tmp As Stream = tmpKnowledges.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
                buffer As New StreamWriter(tmp)
                knowledges = kb.WriteTermsJSON(writer)
            End Using

            Using tmp As Stream = tmpLinks.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
                buffer As New StreamWriter(tmp)
                links = kb.WriteGraphJSON(writer)
            End Using

            Call writer.WriteLine("{")
            Call writer.WriteLine("   meta:{")
            Call writer.WriteLine("      knowledges:" & knowledges.GetJson & ",")
            Call writer.WriteLine("      graph:" & links.GetJson & ",")
            Call writer.WriteLine("   },")

            Call writer.WriteLine("   knowledges: [")
            Call writer.Flush()

            Using tmp As Stream = tmpKnowledges.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Call tmp.CopyTo(writer.BaseStream)
                Call writer.BaseStream.Flush()
            End Using

            Call writer.WriteLine("   ],")

            Call writer.WriteLine("   graph: [")
            Call writer.Flush()

            Using tmp As Stream = tmpLinks.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Call tmp.CopyTo(writer.BaseStream)
                Call writer.BaseStream.Flush()
            End Using

            Call writer.WriteLine("   ]")
            Call writer.WriteLine("}")
            Call writer.Flush()
        End Using

        Return True
    End Function

    <Extension>
    Private Function WriteGraphJSON(links As GraphPool, file As StreamWriter) As IndexByRef
        Dim index As New IndexByRef

        For Each link As LinkMsg In LinkMsg.GetRelationships(links, ref:=index)
            Call file.WriteLine(link.GetJson & ",")
        Next

        Return index
    End Function

    <Extension>
    Private Function WriteTermsJSON(knowledges As GraphPool, file As StreamWriter) As IndexByRef
        Dim index As New IndexByRef

        For Each term As KnowledgeMsg In KnowledgeMsg.GetTerms(knowledges, ref:=index)
            Call file.WriteLine(term.GetJson & ",")
        Next

        Return index
    End Function
End Module
