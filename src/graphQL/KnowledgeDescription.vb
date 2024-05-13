#Region "Microsoft.VisualBasic::759909438295d413f86fce8e74fcebec, src\graphQL\KnowledgeDescription.vb"

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
    '    Code Lines: 25
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 1.02 KB


    ' Class KnowledgeDescription
    ' 
    '     Properties: confidence, mentions, query, relationship, score
    '                 target, totalMentions, type
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Public Class KnowledgeDescription

    Public Property query As String
    Public Property target As String
    Public Property type As String
    Public Property confidence As Double
    Public Property relationship As Relationship
    Public Property mentions As (query As Integer, target As Integer)

    Public ReadOnly Property totalMentions As Integer
        Get
            Return mentions.query + mentions.target
        End Get
    End Property

    Public ReadOnly Property score As Double
        Get
            Return totalMentions * confidence * If(relationship = Relationship.is, 10, 1)
        End Get
    End Property

    Public Overrides Function ToString() As String
        If relationship = Relationship.is Then
            Return $"{query} is the {type} of {target} with confidence {confidence.ToString("F2")}"
        Else
            Return $"{query} has the {type} data '{target}' with confidence {confidence.ToString("F2")}"
        End If
    End Function

End Class
