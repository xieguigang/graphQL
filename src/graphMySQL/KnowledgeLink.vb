#Region "Microsoft.VisualBasic::699a3c65ae26873056c5be9d9bdd7391, src\graphMySQL\KnowledgeLink.vb"

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

    '   Total Lines: 19
    '    Code Lines: 12
    ' Comment Lines: 3
    '   Blank Lines: 4
    '     File Size: 639 B


    ' Class link
    ' 
    '     Properties: display_title, id, node_type, seed, weight
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes

''' <summary>
''' A knowledge link model
''' </summary>
Public Class link

    <DatabaseField("id")> Public Property id As UInteger
    <DatabaseField("seed")> Public Property seed As UInteger
    <DatabaseField("weight")> Public Property weight As Double
    <DatabaseField("display_title")> Public Property display_title As String
    <DatabaseField("node_type")> Public Property node_type As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

End Class
