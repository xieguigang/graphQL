
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