#Region "Microsoft.VisualBasic::65e68475c8615a9f5ee33d29720d7a4d, src\graphQL\Graph\GraphModel.vb"

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

    '   Total Lines: 18
    '    Code Lines: 10
    ' Comment Lines: 5
    '   Blank Lines: 3
    '     File Size: 587 B


    '     Class GraphModel
    ' 
    '         Function: GetElementById
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory

Namespace Graph

    Public MustInherit Class GraphModel : Inherits Graph(Of Knowledge, Association, GraphModel)

        ''' <summary>
        ''' get knowledge node element by id
        ''' </summary>
        ''' <param name="ref"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetElementById(ref As String) As Knowledge
            Return vertices(ref)
        End Function
    End Class
End Namespace
