#Region "Microsoft.VisualBasic::d6ba42c4c3cc4905c8419a837f5bb010, G:/graphQL/src/graphMsg//Message/EvidenceMsg.vb"

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
    '    Code Lines: 33
    ' Comment Lines: 8
    '   Blank Lines: 8
    '     File Size: 1.72 KB


    '     Class EvidenceMsg
    ' 
    '         Properties: data, ref
    ' 
    '         Function: CreateEvidencePack
    ' 
    '     Class ReferenceData
    ' 
    '         Properties: data, ref
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports graphQL.Graph
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Message

    ''' <summary>
    ''' the association link between the knowledge 
    ''' terms and the evidence terms
    ''' </summary>
    Public Class EvidenceMsg

        ''' <summary>
        ''' <see cref="KnowledgeMsg.guid"/>
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> Public Property ref As Integer
        <MessagePackMember(1)> Public Property data As ReferenceData()

        Public Shared Iterator Function CreateEvidencePack(kb As GraphModel) As IEnumerable(Of EvidenceMsg)
            For Each v As Knowledge In kb.vertex
                Yield New EvidenceMsg With {
                    .ref = v.ID,
                    .data = v.evidence _
                        .Select(Function(evi)
                                    Return New ReferenceData With {
                                        .ref = evi.category,
                                        .data = evi.reference
                                    }
                                End Function) _
                        .ToArray
                }
            Next
        End Function
    End Class

    Public Class ReferenceData

        <MessagePackMember(0)> Public Property ref As Integer
        <MessagePackMember(1)> Public Property data As Integer()

        Public Overrides Function ToString() As String
            Return $"({ref}) {data.GetJson}"
        End Function

    End Class
End Namespace
