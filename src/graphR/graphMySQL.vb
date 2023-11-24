
Imports graph.MySQL
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("graph_mysql")>
Public Module graphMySQLTool

    <ExportAPI("open.graphdb")>
    <RApiReturn(GetType(graphMySQL))>
    Public Function openGraphdb(user_name As String, password As String,
                                Optional host As String = "localhost",
                                Optional port As Integer = 3306,
                                Optional dbname As String = "graphql") As Object

        Return New graphMySQL(uri:=New ConnectionUri() With {
            .Database = dbname,
            .IPAddress = host,
            .Password = password,
            .Port = port,
            .User = user_name
        })
    End Function

    <ExportAPI("as.knowledge_builder")>
    Public Function KnowlegdeBuilder(graphdb As graphMySQL) As KnowlegdeBuilder
        Return New KnowlegdeBuilder(graphdb)
    End Function

    <ExportAPI("add_term")>
    Public Function add_term(graphdb As graphMySQL,
                             term As String,
                             Optional category As String = "unclass",
                             <RListObjectArgument>
                             Optional metadata As list = Nothing,
                             Optional env As Environment = Nothing) As Object

        If metadata.length = 1 AndAlso TypeOf metadata.data.First Is list Then
            metadata = metadata.data.First
        End If

        Dim meta_str As Dictionary(Of String, String()) = metadata.AsGeneric(Of String())(env)
        Dim result = graphdb.Add(term, category, meta_str)

        Return result
    End Function

    <ExportAPI("pull_nextGraph")>
    Public Function pullNextGraph(graphdb As KnowlegdeBuilder, <RRawVectorArgument> vocabulary As Object, Optional env As Environment = Nothing) As Object
        Dim cats = CLRVector.asCharacter(vocabulary)
        Dim g As NetworkGraph = graphdb.PullNextGraph(cats)

    End Function

End Module
