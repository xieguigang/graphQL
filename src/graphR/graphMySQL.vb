
Imports graph.MySQL
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

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

    <ExportAPI("add_term")>
    Public Function add_term(graphdb As graphMySQL,
                             term As String,
                             <RListObjectArgument>
                             Optional metadata As list = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim meta_str As Dictionary(Of String, String()) = metadata.AsGeneric(Of String())(env)
        Dim result = graphdb.Add(term, meta_str)

        Return result
    End Function

End Module
