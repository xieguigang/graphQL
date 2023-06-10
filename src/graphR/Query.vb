Imports graphQL
Imports graphQL.Graph
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' graph database knowledge data query and insert
''' </summary>
<Package("Query")>
Public Module Query

    ''' <summary>
    ''' insert a knowledge node into the graph pool
    ''' </summary>
    ''' <param name="knowledge"></param>
    ''' <param name="meta"></param>
    ''' <param name="selfReference">
    ''' the database graph link can be build internal the identical database source
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("insert")>
    Public Function insert(kb As GraphModel, knowledge As String, type As String,
                           Optional meta As list = Nothing,
                           Optional selfReference As Boolean = True,
                           Optional env As Environment = Nothing) As Object

        If knowledge.StringEmpty Then
            Call env.AddMessage({
                $"empty reference term of the knowledge data!",
                $"type: {type}",
                $"meta: {jsonlite.toJSON(meta, env)}"
            }, MSG_TYPES.WRN)
            Return Nothing
        End If

        Dim err As Message = Nothing
        Dim metadata As Dictionary(Of String, String()) = meta.AsGeneric(Of String())(env, err:=err)

        If metadata Is Nothing AndAlso Not err Is Nothing Then
            Return err
        ElseIf TypeOf kb Is GraphPool Then
            Call DirectCast(kb, GraphPool).AddKnowledge(knowledge, type, metadata, selfReference)
        ElseIf TypeOf kb Is EvidenceGraph Then
            Call DirectCast(kb, EvidenceGraph).AddKnowledge(knowledge, type, metadata, selfReference)
        Else
            Throw New NotImplementedException
        End If

        Return kb
    End Function

    ''' <summary>
    ''' ignores the given data types when build graph links.
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="ignores"></param>
    ''' <returns></returns>
    <ExportAPI("ignore.evidenceLink")>
    Public Function ignoreEvidenceLink(kb As EvidenceGraph, <RRawVectorArgument> ignores As Object) As EvidenceGraph
        Dim types As String() = CLRVector.asCharacter(ignores)

        For Each type As String In types
            Call kb.AddIgnores(type)
        Next

        Return kb
    End Function

    <ExportAPI("join")>
    Public Function join(kb1 As GraphModel, kb2 As GraphModel) As GraphModel
        If kb1 Is Nothing Then
            Return kb2
        ElseIf kb2 Is Nothing Then
            Return kb1
        Else
            Return kb1.JoinGraph(kb2)
        End If
    End Function

    ''' <summary>
    ''' query knowledge data for a given term
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="term"></param>
    ''' <param name="cutoff"></param>
    ''' <returns></returns>
    <ExportAPI("query")>
    <RApiReturn(GetType(KnowledgeDescription))>
    Public Function getKnowledge(kb As GraphModel, term As String,
                                 Optional cutoff As Double = 0,
                                 Optional env As Environment = Nothing) As Object

        If kb Is Nothing Then
            Return Internal.debug.stop("the required knowledge database can not be nothing!", env)
        ElseIf TypeOf kb Is GraphPool Then
            Dim data = DirectCast(kb, GraphPool) _
                .GetKnowledgeData(term) _
                .Where(Function(i) i.confidence >= cutoff) _
                .ToArray

            Return data
        ElseIf TypeOf kb Is EvidenceGraph Then
            Dim evidences As EvidenceGraph = DirectCast(kb, EvidenceGraph)
            Dim entities As Knowledge() = evidences.GetMappingTerms(term).ToArray
            Dim output As New list With {.slots = New Dictionary(Of String, Object)}

            For Each node As Knowledge In entities
                Dim evidenceList As New list With {.slots = New Dictionary(Of String, Object)}
                Dim evidenceRaw = evidences.evidences.LoadEvidenceData(node.evidence).ToArray

                For Each item In evidenceRaw
                    Call evidenceList.add(item.Key, item.Value)
                Next

                Dim nodeData As New list With {
                    .slots = New Dictionary(Of String, Object) From {
                        {NameOf(Knowledge.mentions), node.mentions},
                        {NameOf(Knowledge.source), node.source.ToArray},
                        {NameOf(Knowledge.label), node.label},
                        {NameOf(Knowledge.type), node.type},
                        {NameOf(Knowledge.evidence), evidenceList}
                    }
                }
            Next

            Return output
        Else
            Return Message.InCompatibleType(GetType(GraphPool), kb.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' measure the similarity or identical between two 
    ''' knowledge terms based on the knowledge network 
    ''' that we've build.
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="weight"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("similarity")>
    Public Function isEquals(kb As GraphPool, x As String, y As String,
                             <RListObjectArgument>
                             Optional weight As list = Nothing,
                             Optional env As Environment = Nothing) As Double

        Return kb.Similar(x, y, weight.AsGeneric(Of Double)(env))
    End Function
End Module
