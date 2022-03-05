Imports Microsoft.VisualBasic.Data.GraphTheory

''' <summary>
''' A knowledge node in this graph database
''' </summary>
Public Class Knowledge : Inherits Vertex



End Class

Public Class Association : Inherits Edge(Of Knowledge)

    ''' <summary>
    ''' the meta data key name
    ''' </summary>
    ''' <returns></returns>
    Public Property type As String

End Class

Public Class KnowledgeDescription

    Public Property query As String
    Public Property target As String
    Public Property type As String
    Public Property confidence As Double
    Public Property relationship As Relationship

    Public Overrides Function ToString() As String
        If relationship = Relationship.is Then
            Return $"{query} is the {type} of {target} with confidence {confidence.ToString("F2")}"
        Else
            Return $"{query} has the {type} data '{target}' with confidence {confidence.ToString("F2")}"
        End If
    End Function

End Class

Public Enum Relationship
    [is]
    [has]
End Enum