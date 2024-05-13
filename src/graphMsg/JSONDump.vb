#Region "Microsoft.VisualBasic::e72b269a67c30f13a50b47accb00eef6, src\graphMsg\JSONDump.vb"

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

    '   Total Lines: 86
    '    Code Lines: 62
    ' Comment Lines: 6
    '   Blank Lines: 18
    '     File Size: 3.11 KB


    ' Module JSONDump
    ' 
    '     Function: WriteGraphJSON, WriteJSON, WriteTermsJSON
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module JSONDump

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="file">target file stream to write json string data.</param>
    ''' <returns></returns>
    Public Function WriteJSON(kb As GraphModel, file As Stream) As Boolean
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
    Private Function WriteGraphJSON(links As GraphModel, file As StreamWriter) As IndexByRef
        Dim index As New IndexByRef

        For Each link As LinkMsg In LinkMsg.GetRelationships(links, ref:=index)
            Call file.WriteLine(link.GetJson & ",")
        Next

        Return index
    End Function

    <Extension>
    Private Function WriteTermsJSON(knowledges As GraphModel, file As StreamWriter) As IndexByRef
        Dim index As New IndexByRef

        For Each term As KnowledgeMsg In KnowledgeMsg.GetTerms(knowledges, ref:=index)
            Call file.WriteLine(term.GetJson & ",")
        Next

        Return index
    End Function
End Module
