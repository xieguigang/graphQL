Imports graphQL
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("Query")>
Public Module Query

    ''' <summary>
    ''' insert a knowledge node into the graph pool
    ''' </summary>
    ''' <param name="knowledge"></param>
    ''' <param name="meta"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("insert")>
    Public Function insert(kb As GraphPool, knowledge As String,
                           Optional meta As list = Nothing,
                           Optional env As Environment = Nothing) As Object

        Call kb.AddKnowledge(knowledge, meta.AsGeneric(Of String())(env))
        Return kb
    End Function

    <ExportAPI("query")>
    Public Function getKnowledge(kb As GraphPool, term As String, Optional cutoff As Double = 0) As KnowledgeDescription()
        Dim data = kb _
            .GetKnowledgeData(term) _
            .Where(Function(i) i.confidence >= cutoff) _
            .ToArray

        Return data
    End Function

    <ExportAPI("equals")>
    Public Function isEquals(kb As GraphPool, x As String, y As String, Optional cutoff As Double = 0.6) As Object
        Return kb.Similar(x, y) >= cutoff
    End Function
End Module
