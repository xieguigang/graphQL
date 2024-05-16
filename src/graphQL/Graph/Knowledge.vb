#Region "Microsoft.VisualBasic::a0248f8f5e578f12ecceb23e9e50740c, src\graphQL\Graph\Knowledge.vb"

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

    '   Total Lines: 59
    '    Code Lines: 15
    ' Comment Lines: 35
    '   Blank Lines: 9
    '     File Size: 1.92 KB


    '     Class Knowledge
    ' 
    '         Properties: evidence, isMaster, mentions, source, type
    ' 
    '         Sub: AddReferenceSource
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory

Namespace Graph

    ''' <summary>
    ''' A knowledge node in this graph database
    ''' </summary>
    Public Class Knowledge : Inherits Vertex

        ''' <summary>
        ''' the count of the current knowledge term 
        ''' has been mentioned in the knowledge 
        ''' network.
        ''' </summary>
        ''' <returns></returns>
        Public Property mentions As Integer

        ''' <summary>
        ''' the data type of the current term data.
        ''' </summary>
        ''' <returns></returns>
        Public Property type As String

        ''' <summary>
        ''' current knowledge term is a master source term?
        ''' </summary>
        ''' <returns>
        ''' true - is a master source term, 
        ''' false - is a meta data term
        ''' </returns>
        Public Property isMaster As Boolean

        ''' <summary>
        ''' the data source of this knowledge term node
        ''' </summary>
        ''' <returns></returns>
        Public Property source As New List(Of String)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 20220413
        ''' 
        ''' 在这里原先是直接使用一个字符串字典来表示的，但是在处理大型数据集的时候
        ''' 在指向相同对象实体的词条之间冗余的字符串会造成大量的内存浪费
        ''' 所以在这里改为指针数据以减少内存冗余信息，降低内存占用率
        ''' </remarks>
        Public Property evidence As New List(Of Evidence)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub AddReferenceSource(source As String)
            Call Me.source.Add(source)
        End Sub

    End Class
End Namespace
