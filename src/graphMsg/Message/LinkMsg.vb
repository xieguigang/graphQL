#Region "Microsoft.VisualBasic::551a9d00e5be2d62e268b54c1ace268e, G:/graphQL/src/graphMsg//Message/LinkMsg.vb"

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

    '   Total Lines: 47
    '    Code Lines: 40
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.78 KB


    '     Class LinkMsg
    ' 
    '         Properties: referenceSources, type, u, v, weight
    ' 
    '         Function: GetRelationships, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports graphQL.Graph
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Message

    Public Class LinkMsg

        <MessagePackMember(0)> Public Property u As Integer
        <MessagePackMember(1)> Public Property v As Integer
        <MessagePackMember(2)> Public Property weight As Double
        <MessagePackMember(3)> Public Property type As Integer
        <MessagePackMember(4)> Public Property referenceSources As Integer()

        Public Overrides Function ToString() As String
            Return $"[{u}->{v}] {type}"
        End Function

        Public Shared Iterator Function GetRelationships(kb As GraphModel, Optional ref As IndexByRef = Nothing) As IEnumerable(Of LinkMsg)
            Dim allTypes As Index(Of String) = kb.graphEdges _
                .Select(Function(i) i.type) _
                .Distinct _
                .Indexing
            Dim allSources = kb.graphEdges _
                .Select(Function(i) i.source) _
                .IteratesALL _
                .Distinct _
                .Indexing

            If Not ref Is Nothing Then
                ref.source = allSources.Objects
                ref.types = allTypes.Objects
            End If

            For Each link As Association In kb.graphEdges
                Yield New LinkMsg With {
                    .type = allTypes.IndexOf(link.type),
                    .u = link.U.ID,
                    .v = link.V.ID,
                    .weight = link.weight,
                    .referenceSources = allSources.IndexOf(link.source.Distinct)
                }
            Next
        End Function
    End Class
End Namespace
