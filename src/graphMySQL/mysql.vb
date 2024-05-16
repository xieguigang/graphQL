#Region "Microsoft.VisualBasic::0de0e218d1a21395bbdc8d068cdd3fb6, src\graphMySQL\mysql.vb"

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

    '   Total Lines: 13
    '    Code Lines: 9
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 356 B


    ' Class mysql
    ' 
    '     Properties: knowledge_cache
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri

Public Class mysql : Inherits graphdbMySQL

    Public ReadOnly Property knowledge_cache As Model

    Public Sub New(uri As ConnectionUri)
        MyBase.New(uri)
        knowledge_cache = New Model("knowledge_cache", uri)
    End Sub

End Class
