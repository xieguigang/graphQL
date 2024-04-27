#Region "Microsoft.VisualBasic::e3f8cfa0796a53980850fab7acf7668e, G:/graphQL/src/graphMsg//Message/KnowledgeMsg.vb"

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

    '   Total Lines: 49
    '    Code Lines: 38
    ' Comment Lines: 4
    '   Blank Lines: 7
    '     File Size: 1.89 KB


    '     Class KnowledgeMsg
    ' 
    '         Properties: guid, isMaster, mentions, referenceSources, term
    '                     type
    ' 
    '         Function: GetTerms, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports graphQL.Graph
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Message

    Public Class KnowledgeMsg

        ''' <summary>
        ''' the unique reference id of current knowledge node in the graph.
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> Public Property guid As Integer
        <MessagePackMember(1)> Public Property term As String
        <MessagePackMember(2)> Public Property mentions As Integer
        <MessagePackMember(3)> Public Property type As Integer
        <MessagePackMember(4)> Public Property isMaster As Boolean
        <MessagePackMember(5)> Public Property referenceSources As Integer()

        Public Overrides Function ToString() As String
            Return term
        End Function

        Public Shared Iterator Function GetTerms(kb As GraphModel, Optional ref As IndexByRef = Nothing) As IEnumerable(Of KnowledgeMsg)
            Dim allTypes = kb.vertex.Select(Function(i) i.type).Distinct.Indexing
            Dim allSources = kb.vertex _
                .Select(Function(i) i.source) _
                .IteratesALL _
                .Distinct _
                .Indexing

            If Not ref Is Nothing Then
                ref.types = allTypes.Objects
                ref.source = allSources.Objects
            End If

            For Each v As Knowledge In kb.vertex
                Yield New KnowledgeMsg With {
                    .guid = v.ID,
                    .term = v.label,
                    .mentions = v.mentions,
                    .type = allTypes.IndexOf(v.type),
                    .isMaster = v.isMaster,
                    .referenceSources = allSources.IndexOf(v.source.Distinct)
                }
            Next
        End Function
    End Class
End Namespace
