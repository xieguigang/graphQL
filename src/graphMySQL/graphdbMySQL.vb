#Region "Microsoft.VisualBasic::88648b57cee7a3d1cfc7ef70ffb17a07, G:/graphQL/src/graphMySQL//graphdbMySQL.vb"

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

    '   Total Lines: 30
    '    Code Lines: 23
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.03 KB


    ' Class graphdbMySQL
    ' 
    '     Properties: graph, hash_index, knowledge, knowledge_vocabulary
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri

Public MustInherit Class graphdbMySQL : Inherits IDatabase

    Public ReadOnly Property graph As Model
    Public ReadOnly Property hash_index As Model
    Public ReadOnly Property knowledge As Model
    Public ReadOnly Property knowledge_vocabulary As Model

    Sub New(uri As ConnectionUri)
        Call MyBase.New(uri)

        graph = model(Of graphdb.graph)()
        hash_index = model(Of graphdb.hash_index)()
        knowledge = model(Of graphdb.knowledge)()
        knowledge_vocabulary = model(Of graphdb.knowledge_vocabulary)()
    End Sub

    Protected Sub New(graph As Model, hash_index As Model, knowledge As Model, knowledge_vocabulary As Model)
        Call MyBase.New(Nothing)

        Me.graph = graph
        Me.hash_index = hash_index
        Me.knowledge = knowledge
        Me.knowledge_vocabulary = knowledge_vocabulary
    End Sub

End Class
