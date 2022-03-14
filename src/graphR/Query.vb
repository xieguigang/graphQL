Imports graphQL
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
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
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("insert")>
    Public Function insert(kb As GraphPool, knowledge As String, type As String,
                           Optional meta As list = Nothing,
                           Optional env As Environment = Nothing) As Object

        If knowledge.StringEmpty Then
            Call env.AddMessage({
                $"empty reference term of the knowledge data!",
                $"type: {type}",
                $"meta: {jsonlite.toJSON(meta, env)}"
            }, MSG_TYPES.WRN)
            Return Nothing
        End If

        Call kb.AddKnowledge(knowledge, type, meta.AsGeneric(Of String())(env))
        Return kb
    End Function

    ''' <summary>
    ''' query knowledge data for a given term
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <param name="term"></param>
    ''' <param name="cutoff"></param>
    ''' <returns></returns>
    <ExportAPI("query")>
    Public Function getKnowledge(kb As GraphPool, term As String, Optional cutoff As Double = 0) As KnowledgeDescription()
        Dim data = kb _
            .GetKnowledgeData(term) _
            .Where(Function(i) i.confidence >= cutoff) _
            .ToArray

        Return data
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

    ''' <summary>
    ''' export the graph database as the 
    ''' network graph model for run 
    ''' algorithm debug.
    ''' </summary>
    ''' <param name="kb"></param>
    ''' <returns>
    ''' nodes meta: knowledge_type
    ''' </returns>
    <ExportAPI("networkGraph")>
    Public Function networkGraph(kb As GraphPool) As NetworkGraph
        Return kb.createGraph
    End Function

    ''' <summary>
    ''' export knowledge terms based on the network community algorithm
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="common_type">
    ''' all of the type defined from this parameter will be removed from 
    ''' the community algorithm due to the reason of common type always 
    ''' be a hub node in the network, will create a false knowledge community 
    ''' result. example as formula string in chemical data knowledges 
    ''' will groups all Isomer compounds with the same formula string as 
    ''' one identical metabolite.
    ''' </param>
    ''' <returns>
    ''' this function returns a tuple list with two elements inside:
    ''' 
    ''' 1. ``graph`` - is the knowledge network graph data with community 
    '''                tags and trimmed data.
    ''' 2. ``knowledges`` - a table dataset that contains knowledge data 
    '''                     entities that detects from the network graph 
    '''                     community data result.
    ''' </returns>
    <ExportAPI("knowledgeCommunity")>
    Public Function knowledgeCommunity(g As NetworkGraph,
                                       <RRawVectorArgument(GetType(String))>
                                       Optional common_type As Object = Nothing,
                                       Optional eps As Double = 0.001) As list

        Dim commons As Index(Of String) = DirectCast(REnv.asVector(Of String)(common_type), String()).Indexing
        Dim copy As New NetworkGraph

        For Each v As Node In g.vertex.ToArray
            If Not v.data("knowledge_type") Like commons Then
                Call copy.CreateNode(v.label, v.data.Clone)
            End If
        Next

        For Each edge As Edge In g.graphEdges
            If (edge.U.data("knowledge_type") Like commons) OrElse (edge.V.data("knowledge_type") Like commons) Then
                Continue For
            End If

            Call copy.CreateEdge(
                u:=g.GetElementByID(edge.U.label),
                v:=g.GetElementByID(edge.V.label),
                weight:=edge.weight,
                data:=edge.data.Clone
            )
        Next

        Dim knowledges As New List(Of EntityObject)
        Dim communityList = Communities.Analysis(copy, eps:=eps) _
            .vertex _
            .GroupBy(Function(v)
                         Return v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
                     End Function) _
            .ToArray

        For Each term In communityList
            Dim metadata = term.GroupBy(Function(v) v.data("knowledge_type")).ToArray
            Dim props As New Dictionary(Of String, String)

            For Each p In metadata
                Call props.Add(p.Key, p.Select(Function(v) v.label).JoinBy("; "))
            Next

            Call knowledges.Add(New EntityObject With {
                .ID = term.Key,
                .Properties = props
            })
        Next

        Dim rtvl As New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"graph", copy},
                {"knowledges", knowledges.ToArray}
            }
        }

        Return rtvl
    End Function
End Module
