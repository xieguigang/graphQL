
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("graph_mysql")>
Public Module graphMySQL

    <ExportAPI("open.graphdb")>
    <RApiReturn("graph", "hash_index", "knowledge", "knowledge_vocabulary")>
    Public Function openGraphdb(user_name As String, password As String,
                                Optional host As String = "localhost",
                                Optional port As Integer = 3306,
                                Optional dbname As String = "graphql") As Object

        Dim uri As New ConnectionUri() With {
            .Database = dbname,
            .IPAddress = host,
            .Password = password,
            .Port = port,
            .User = user_name
        }

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"graph", New Model("graph", uri)},
                {"hash_index", New Model("hash_index", uri)},
                {"knowledge", New Model("knowledge", uri)},
                {"knowledge_vocabulary", New Model("knowledge_vocabulary", uri)}
            }
        }
    End Function

End Module
