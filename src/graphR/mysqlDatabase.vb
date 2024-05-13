#Region "Microsoft.VisualBasic::4d28e72a0e94d174091d323bdf97e23f, src\graphR\mysqlDatabase.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 315
    '    Code Lines: 214
    ' Comment Lines: 53
    '   Blank Lines: 48
    '     File Size: 12.26 KB


    ' Class MySqlDatabase
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' Module mysqlDatabaseTool
    ' 
    '     Function: [select], createFileDumpTask, dump_inserts, limit, open
    '               performance_counter, project, table, where, writeRows
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Data
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.LibMySQL.PerformanceCounter
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports renv = SMRUCC.Rsharp.Runtime

''' <summary>
''' a general mysql database model
''' </summary>
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

        Dim asserts As New List(Of FieldAssert)
        Dim parse As [Variant](Of Message, FieldAssert)
        Dim field As Expression = Nothing

        For Each ref As String In args.getNames
            If ref.IsPattern("[$]\d+") Then
                ' no name, is lazy expression
                field = args.getByName(ref)
                parse = table.conditionField(field, env)
            Else
                ' has name, is value equals test, example as a = b
                field = args.getByName(ref)
                parse = New FieldAssert(ref) = field.Evaluate(env)
            End If

            If parse Like GetType(Message) Then
                Return parse.TryCast(Of Message)
            End If

            asserts.Add(parse.TryCast(Of FieldAssert))
        Next

        Return table.where(asserts.ToArray)
    End Function

    <ExportAPI("project")>
    Public Function project(table As Model, field As String, Optional env As Environment = Nothing) As Object
        Dim reader As DataTableReader = table.select(field)
        Dim vals As New List(Of Object)

        Do While reader.Read
            vals.Add(reader.GetValue(0))
        Loop

        Return renv.asVector(vals.ToArray, reader.GetFieldType(0), env)
    End Function

    <ExportAPI("select")>
    Public Function [select](table As Model,
                             <RListObjectArgument>
                             <RLazyExpression>
                             Optional args As list = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim project As Expression() = args.data _
            .Select(Function(e) DirectCast(e, Expression)) _
            .ToArray
        Dim fields As New List(Of String)
        Dim parse As [Variant](Of Message, String)

        For Each field As Expression In project
            parse = field.projectField(env)

            If parse Like GetType(Message) Then
                Return parse.TryCast(Of Message)
            End If

            fields.Add(parse.TryCast(Of String))
        Next

        Dim reader As DataTableReader = table.select(fields.ToArray)
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }
        Dim rows As New List(Of Object())
        Dim fieldSize As Integer = reader.FieldCount

        Do While reader.Read
            Dim row As Object() = New Object(fieldSize - 1) {}

            For i As Integer = 0 To row.Length - 1
                row(i) = reader.GetValue(i)
            Next

            rows.Add(row)
        Loop

        For i As Integer = 0 To fieldSize - 1
            Dim offset As Integer = i
            Dim v As Object() = rows.Select(Function(r) r(offset)).ToArray
            Dim name As String = reader.GetName(offset)

            Call df.add(name, renv.UnsafeTryCastGenericArray(v))
        Next

        Return df
    End Function

    <ExportAPI("limit")>
    Public Function limit(table As Model, m As Integer, Optional n As Integer? = Nothing) As Object
        Return table.limit(m, n)
    End Function

    ''' <summary>
    ''' run the mysql performance counter in a given timespan perioid.
    ''' </summary>
    ''' <param name="mysql">mysql connection parameters for create a 
    ''' mysql performance counter <see cref="Logger"/> object.</param>
    ''' <param name="task">
    ''' the timespan value for run current performance counter task, value could be generates 
    ''' from the time related R# base function: 
    ''' 
    ''' ``hours``, ``minutes``, ``seconds``, ``days``, ``time_span``.
    ''' </param>
    ''' <param name="resolution">
    ''' the mysql performance counter data sampling time resolution value, 
    ''' time internal data unit in seconds.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' the return tuple list data has attribute data ``global_status``, is the raw data 
    ''' for the performance counter which is pulled from the mysql server.
    ''' </returns>
    <ExportAPI("performance_counter")>
    <RApiReturn("Bytes_received", "Bytes_sent",
                "Selects", "Inserts", "Deletes", "Updates",
                "Client_connections",
                "Innodb_buffer_pool_read_requests", "Innodb_buffer_pool_write_requests",
                "Innodb_data_read",
                "timestamp")>
    Public Function performance_counter(mysql As Object, task As TimeSpan,
                                        Optional resolution As Double = 1,
                                        Optional env As Environment = Nothing) As Object
        Dim mysqli As MySqli = Nothing

        If mysql Is Nothing Then
            Return Internal.debug.stop("the required mysqli connection object should not be nothing!", env)
        End If

        If TypeOf mysql Is MySqli Then
            mysqli = mysql
        ElseIf TypeOf mysql Is Model Then
            mysqli = DirectCast(mysql, Model).getDriver
        ElseIf TypeOf mysql Is MySqlDatabase Then
            mysqli = DirectCast(mysql, MySqlDatabase).getDriver
        Else
            Return Message.InCompatibleType(GetType(MySqli), mysql.GetType, env)
        End If

        Dim logger As New Logger(mysqli, resolution)
        Dim counter_data As New list With {
            .slots = logger _
                .Run(task) _
                .GetLogging _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return CObj(a.Value)
                              End Function)
        }

        Call counter_data.setAttribute("global_status", logger.GetGlobalStatus)

        Return counter_data
    End Function
End Module
