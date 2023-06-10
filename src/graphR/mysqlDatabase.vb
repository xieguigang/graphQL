
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("mysql")>
Module mysqlDatabase

    ''' <summary>
    ''' dump the inserts transaction mysql file
    ''' </summary>
    ''' <param name="data">A collection of the mysql row data for insert into the database</param>
    ''' <param name="dir"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("dump_inserts")>
    Public Function dump_inserts(<RRawVectorArgument> data As Object, dir As String, Optional env As Environment = Nothing) As Object
        Dim pack As pipeline = pipeline.TryCreatePipeline(Of MySQLTable)(data, env)

        If pack.isError Then
            Return pack.getError
        End If

        Call pack.populates(Of MySQLTable)(env).ProjectDumping(dir, truncate:=True)

        Return Nothing
    End Function
End Module
