#Region "Microsoft.VisualBasic::60d7a230a4d55f63350c9c5ffe543b27, src\graphQL\Graph\Association.vb"

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

    '   Total Lines: 27
    '    Code Lines: 12
    ' Comment Lines: 9
    '   Blank Lines: 6
    '     File Size: 755 B


    '     Class Association
    ' 
    '         Properties: source, type
    ' 
    '         Sub: AddReferenceSource
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory

Namespace Graph

    Public Class Association : Inherits Edge(Of Knowledge)

        ''' <summary>
        ''' the meta data key name
        ''' </summary>
        ''' <returns></returns>
        Public Property type As String

        ''' <summary>
        ''' the data source of this knowledge term 
        ''' association data.
        ''' </summary>
        ''' <returns></returns>
        Public Property source As New List(Of String)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub AddReferenceSource(source As String)
            Call Me.source.Add(source)
        End Sub

    End Class
End Namespace
