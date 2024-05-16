#Region "Microsoft.VisualBasic::05be6154cfd43a32609e976e638a83f2, src\graphMySQL\ColorHashCode.vb"

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

    '   Total Lines: 76
    '    Code Lines: 53
    ' Comment Lines: 9
    '   Blank Lines: 14
    '     File Size: 2.33 KB


    ' Class ColorHashCode
    ' 
    '     Function: HexColor, HslColor, Hue, Lightness, RgbColor
    '               Saturation
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Imaging

Public Class ColorHashCode

#Region "Parameters for create HSL color"

    ''' <summary>
    ''' range of the hue value, value should be in range of [0,360]
    ''' </summary>
    Dim m_hue As DoubleRange() = {}
    ''' <summary>
    ''' Saturation
    ''' </summary>
    Dim m_S As Double() = {0.35, 0.5, 0.65}
    ''' <summary>
    ''' Lightness
    ''' </summary>
    Dim m_L As Double() = {0.35, 0.5, 0.65}

#End Region

    Public Function Hue(ParamArray ranges As DoubleRange()) As ColorHashCode
        m_hue = ranges
        Return Me
    End Function

    Public Function Saturation(ParamArray linear As Double()) As ColorHashCode
        m_S = linear
        Return Me
    End Function

    Public Function Lightness(ParamArray linear As Double()) As ColorHashCode
        m_L = linear
        Return Me
    End Function

    Public Function RgbColor(value As String) As Color
        Dim hsl = HslColor(value)
        Dim color = hsl.ToRGB
        Return color
    End Function

    Public Function HexColor(value As String) As String
        Dim color = HslColor(value).ToRGB
        Dim hex = color.ToHtmlColor
        Return hex
    End Function

    Public Function HslColor(value As String) As HSLColor
        Dim h, s, l As Double
        Dim hash As ULong = BKDRHash.GenerateVersion3(value)

        If m_hue.TryCount > 0 Then
            Dim rangeIndex As Integer = CInt(hash Mod Convert.ToUInt64(m_hue.Length))
            Dim hueValue As DoubleRange = m_hue(rangeIndex)
            Dim hueResolution = Convert.ToUInt64(727)

            h = (hash / Convert.ToUInt64(m_hue.Count) Mod hueResolution) *
                (Convert.ToUInt64(hueValue.Max) - Convert.ToUInt64(hueValue.Min)) / hueResolution +
                Convert.ToUInt64(hueValue.Min)
        Else
            h = hash Mod 359 ' note that 359 is a prime
        End If

        hash = hash / 360
        s = m_S(hash Mod CULng(m_S.Length))
        hash = hash / m_S.Length
        l = m_L(hash Mod CULng(m_L.Length))

        Return New HSLColor(h, s, l)
    End Function
End Class
