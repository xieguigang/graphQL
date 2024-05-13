#Region "Microsoft.VisualBasic::7dfa0967c55e08029c92fa1f042c4e40, src\graphMsg\Message\IndexByRef.vb"

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

    '   Total Lines: 16
    '    Code Lines: 11
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 443 B


    '     Class IndexByRef
    ' 
    '         Properties: source, types
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Message

    Public Class IndexByRef

        <MessagePackMember(0)> Public Property types As String()
        <MessagePackMember(1)> Public Property source As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

    End Class
End Namespace
