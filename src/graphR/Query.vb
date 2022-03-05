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
    Public Function getKnowledge(kb As GraphPool, term As String, Optional cutoff As Double = 0) As list
        Dim data = kb _
            .GetKnowledgeData(term) _
            .Where(Function(i) i.Value >= cutoff) _
            .GroupBy(Function(b) b.Description) _
            .ToArray

        Return New list With {
           .slots = data _
              .ToDictionary(Function(i) i.Key,
                            Function(i)
                                Return CObj(i.Select(Function(j) j.Name).ToArray)
                            End Function)
        }
    End Function

End Module
