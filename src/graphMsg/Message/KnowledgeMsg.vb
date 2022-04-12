Imports graphQL.Graph
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Message

    Public Class KnowledgeMsg

        ''' <summary>
        ''' the unique reference id of current knowledge node in the graph.
        ''' </summary>
        ''' <returns></returns>
        <MessagePackMember(0)> Public Property guid As Integer
        <MessagePackMember(1)> Public Property term As String
        <MessagePackMember(2)> Public Property mentions As Integer
        <MessagePackMember(3)> Public Property type As Integer
        <MessagePackMember(4)> Public Property isMaster As Boolean
        <MessagePackMember(5)> Public Property referenceSources As Integer()

        Public Overrides Function ToString() As String
            Return term
        End Function

        Public Shared Iterator Function GetTerms(kb As GraphPool, Optional ref As IndexByRef = Nothing) As IEnumerable(Of KnowledgeMsg)
            Dim allTypes = kb.vertex.Select(Function(i) i.type).Distinct.Indexing
            Dim allSources = kb.vertex _
                .Select(Function(i) i.source) _
                .IteratesALL _
                .Distinct _
                .Indexing

            If Not ref Is Nothing Then
                ref.types = allTypes.Objects
                ref.source = allSources.Objects
            End If

            For Each v As Knowledge In kb.vertex
                Yield New KnowledgeMsg With {
                    .guid = v.ID,
                    .term = v.label,
                    .mentions = v.mentions,
                    .type = allTypes.IndexOf(v.type),
                    .isMaster = v.isMaster,
                    .referenceSources = allSources.IndexOf(v.source.Distinct)
                }
            Next
        End Function
    End Class
End Namespace