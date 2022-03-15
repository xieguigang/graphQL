Imports graphQL
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.DBSCAN
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.DataMining.UMAP
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
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

    <ExportAPI("graphUMAP")>
    Public Function graphUMAP(g As NetworkGraph, Optional eps As Double = 0.1) As Object
        Dim labels As String() = Nothing
        Dim umap As Umap = g.RunUMAP(labels)
        Dim embedding = umap.GetEmbedding
        Dim raw As ClusterEntity() = labels _
            .Select(Function(id, i)
                        Dim vec = embedding(i)
                        Dim point As New ClusterEntity With {
                            .uid = id,
                            .entityVector = vec
                        }

                        Return point
                    End Function) _
            .ToArray

        ' run dbscan
        Dim dbscan As New DbscanAlgorithm(Of ClusterEntity)(Function(x, y) x.entityVector.EuclideanDistance(y.entityVector))
        Dim result = dbscan.ComputeClusterDBSCAN(raw, eps, 5)
        Dim mat As New List(Of ClusterEntity)
        Dim idx As Integer = 0

        For Each group In result
            idx += 1

            For Each x In group
                x.cluster = idx
            Next

            mat.AddRange(group)
        Next

        Dim frame As New dataframe With {
            .rownames = mat.Select(Function(r) r.uid).ToArray,
            .columns = New Dictionary(Of String, Array) From {
                {"group", mat.Select(Function(r) r.cluster).ToArray}
            }
        }

        frame.columns("x") = mat.Select(Function(r) r.entityVector(0)).ToArray
        frame.columns("y") = mat.Select(Function(r) r.entityVector(1)).ToArray
        frame.columns("z") = mat.Select(Function(r) r.entityVector(2)).ToArray

        Return frame
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
                                       Optional eps As Double = 0.001,
                                       Optional unweighted As Boolean = False) As list

        Dim commons As Index(Of String) = DirectCast(REnv.asVector(Of String)(common_type), String()).Indexing
        Dim copy As New NetworkGraph

        If commons.Count > 0 Then
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
                    u:=copy.GetElementByID(edge.U.label),
                    v:=copy.GetElementByID(edge.V.label),
                    weight:=edge.weight,
                    data:=edge.data.Clone
                )
            Next
        Else
            copy = g
        End If

        If unweighted Then
            Call Communities.AnalysisUnweighted(copy)
        Else
            Call Communities.Analysis(copy, eps:=eps)
        End If

        If commons.Count > 0 Then
            For Each v As Node In copy.vertex
                g.GetElementByID(v.label).data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
            Next
        End If

        Dim knowledges As New List(Of EntityObject)
        Dim communityList = g.vertex _
            .GroupBy(Function(v)
                         Return v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)
                     End Function) _
            .ToArray

        For Each term In communityList
            Dim hits As Index(Of String) = term.Select(Function(v) v.label).Indexing
            Dim metadata = g.graphEdges _
                .Where(Function(url)
                           Return url.U.label Like hits OrElse url.V.label Like hits
                       End Function) _
                .Select(Function(url) {url.U, url.V}) _
                .IteratesALL _
                .GroupBy(Function(v) v.label) _
                .Select(Function(v) v.First) _
                .GroupBy(Function(v)
                             Return v.data("knowledge_type")
                         End Function) _
                .ToArray
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
                {"graph", g},
                {"knowledges", knowledges.ToArray}
            }
        }

        Return rtvl
    End Function
End Module
