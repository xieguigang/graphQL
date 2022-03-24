Imports System.IO
Imports System.Runtime.CompilerServices
Imports graphQL
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
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
                knowledges = kb.vertex.WriteJSON(writer)
            End Using

            Using tmp As Stream = tmpLinks.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
                buffer As New StreamWriter(tmp)
                links = kb.graphEdges.WriteJSON(writer)
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
        End Using

        Return True
    End Function

    <Extension>
    Private Function WriteJSON(links As IEnumerable(Of Association), file As StreamWriter) As IndexByRef
        Dim types As New Index(Of String)
        Dim sources As New Index(Of String)

        Return New IndexByRef With {
            .types = types.Objects,
            .source = sources.Objects
        }
    End Function

    <Extension>
    Private Function WriteJSON(knowledges As IEnumerable(Of Knowledge), file As StreamWriter) As IndexByRef
        Dim types As New Index(Of String)
        Dim sources As New Index(Of String)

        For Each term As Knowledge In knowledges

        Next

        Return New IndexByRef With {
            .types = types.Objects,
            .source = sources.Objects
        }
    End Function
End Module
