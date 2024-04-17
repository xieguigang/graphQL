
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Closure
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.DataSets
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Operators
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class MySqlDatabase : Inherits IDatabase

    Public Sub New(mysqli As ConnectionUri)
        MyBase.New(mysqli)
    End Sub
End Class

''' <summary>
''' MySQL (/ˌmaɪˌɛsˌkjuːˈɛl/) is an open-source relational database management system (RDBMS).
''' Its name is a combination of "My", the name of co-founder Michael Widenius's daughter My,
''' and "SQL", the acronym for Structured Query Language. A relational database organizes data 
''' into one or more data tables in which data may be related to each other; these relations 
''' help structure the data. SQL is a language that programmers use to create, modify and extract
''' data from the relational database, as well as control user access to the database. In 
''' addition to relational databases and SQL, an RDBMS like MySQL works with an operating system 
''' to implement a relational database in a computer's storage system, manages users, allows 
''' for network access and facilitates testing database integrity and creation of backups.
''' </summary>
<Package("mysql")>
Module mysqlDatabaseTool

    ''' <summary>
    ''' open a mysql connection, construct a database model
    ''' </summary>
    ''' <param name="user_name"></param>
    ''' <param name="password"></param>
    ''' <param name="dbname"></param>
    ''' <param name="host"></param>
    ''' <param name="port"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("open")>
    <RApiReturn(GetType(IDatabase))>
    Public Function open(Optional user_name As String = Nothing,
                         Optional password As String = Nothing,
                         Optional dbname As String = Nothing,
                         Optional host As String = "localhost",
                         Optional port As Integer = 3306,
                         Optional error_log As String = Nothing,
                         Optional timeout As Integer = -1,
                         Optional connection_uri As String = Nothing,
                         Optional general As Boolean = False,
                         Optional env As Environment = Nothing)

        Dim url As ConnectionUri
        Dim db As IDatabase

        If Not connection_uri.StringEmpty Then
            url = CType(connection_uri, ConnectionUri)
            url.error_log = error_log
            url.TimeOut = timeout
        Else
            url = New ConnectionUri With {
                .Database = dbname,
                .IPAddress = host,
                .Password = password,
                .Port = port,
                .User = user_name,
                .error_log = error_log,
                .TimeOut = timeout
            }

            If user_name.StringEmpty Then
                Return Internal.debug.stop("mysql user name could not be empty!", env)
            ElseIf password.StringEmpty Then
                Return Internal.debug.stop("mysql user password could not be empty!", env)
            ElseIf host.StringEmpty Then
                Return Internal.debug.stop("mysql host should not be empty!", env)
            ElseIf port <= 0 Then
                Return Internal.debug.stop("mysql network services tcp port should be a positive number!", env)
            End If
        End If

        If general Then
            Return New MySqlDatabase(url)
        End If

        Dim type As RType = env.globalEnvironment.GetType(url.Database)

        Try
            db = Activator.CreateInstance(type:=type.raw, url)
        Catch ex As Exception
            Return Internal.debug.stop(ex, env)
        End Try

        Return db
    End Function

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

    <ExportAPI("create_filedump")>
    Public Function createFileDumpTask(dir As String, Optional env As Environment = Nothing) As DumpTaskRunner
        Return New DumpTaskRunner(
            EXPORT:=dir,
            truncate:=True,
            singleTransaction:=False,
            echo:=env.globalEnvironment.verboseOption,
            AI:=False,
            bufferSize:=1024
        )
    End Function

    <ExportAPI("write_dumps")>
    Public Function writeRows(dump As DumpTaskRunner, <RRawVectorArgument> data As Object, Optional env As Environment = Nothing) As Object
        Dim pack As pipeline = pipeline.TryCreatePipeline(Of MySQLTable)(data, env)

        If pack.isError Then
            Return pack.getError
        End If

        Return dump.DumpRows(pack.populates(Of MySQLTable)(env))
    End Function

    <ExportAPI("table")>
    Public Function table(mysql As IDatabase, name As String) As Model
        Return mysql.CreateModel(name)
    End Function

    <ExportAPI("where")>
    Public Function where(table As Model,
                          <RListObjectArgument>
                          <RLazyExpression> args As list,
                          Optional env As Environment = Nothing) As Object

        Dim conditions As Expression() = args.data _
            .Select(Function(e) DirectCast(e, Expression)) _
            .ToArray
        Dim asserts As New List(Of FieldAssert)
        Dim parse As [Variant](Of Message, FieldAssert)

        For Each field As Expression In conditions
            parse = table.conditionField(field, env)

            If parse Like GetType(Message) Then
                Return parse.TryCast(Of Message)
            End If

            asserts.Add(parse.TryCast(Of FieldAssert))
        Next

        Return table.where(asserts.ToArray)
    End Function

    <Extension>
    Private Function conditionField(table As Model, field As Expression, env As Environment) As [Variant](Of Message, FieldAssert)
        If TypeOf field Is BinaryExpression Then
        ElseIf TypeOf field Is BinaryBetweenExpression Then
            Dim bin As BinaryBetweenExpression = DirectCast(field, BinaryBetweenExpression)
            Dim name = ValueAssignExpression.GetSymbol(bin.left)
            Dim range = bin.right.Evaluate(env)
            Dim fieldName As FieldAssert = table.field(name)

            If TypeOf range Is list Then
                Throw New NotImplementedException
            ElseIf TypeOf range Is vector OrElse range.GetType.IsArray Then
                Return fieldName.in(CLRVector.asCharacter(range))
            ElseIf TypeOf range Is Message Then
                Return DirectCast(range, Message)
            Else
                Throw New NotImplementedException
            End If
        ElseIf TypeOf field Is BinaryInExpression Then

        ElseIf TypeOf field Is FunctionInvoke Then
        Else
            Return Internal.debug.stop(New NotImplementedException(field.ToString), env)
        End If
    End Function

    <ExportAPI("select")>
    Public Function [select](table As Model,
                             <RListObjectArgument>
                             Optional args As list = Nothing,
                             Optional env As Environment = Nothing) As Object

        Throw New NotImplementedException
    End Function

    <ExportAPI("limit")>
    Public Function limit(table As Model, m As Integer, Optional n As Integer? = Nothing) As Object
        Return table.limit(m, n)
    End Function
End Module
