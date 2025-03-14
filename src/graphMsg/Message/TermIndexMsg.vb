﻿#Region "Microsoft.VisualBasic::8e1c377a018e1846dd7aa0577de499a6, src\graphMsg\Message\TermIndexMsg.vb"

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

    '   Total Lines: 22
    '    Code Lines: 14
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 615 B


    '     Class TermIndexMsg
    ' 
    '         Properties: index
    ' 
    '         Function: ToList
    ' 
    '     Class TermIndex
    ' 
    '         Properties: block, offset, term
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization

Namespace Message

    Public Class TermIndexMsg

        <MessagePackMember(0)> Public Property index As TermIndex()

        Public Function ToList() As Dictionary(Of String, TermIndex)
            Return index.ToDictionary(Function(i) i.term)
        End Function

    End Class

    Public Class TermIndex

        <MessagePackMember(0)> Public Property term As String
        <MessagePackMember(1)> Public Property block As String
        <MessagePackMember(2)> Public Property offset As Integer

    End Class
End Namespace
