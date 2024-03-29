﻿
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

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
Module mysqlDatabase

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
    Public Function open(user_name As String, password As String, dbname As String,
                         Optional host As String = "localhost",
                         Optional port As Integer = 3306,
                         Optional env As Environment = Nothing)

        Dim type As RType = env.globalEnvironment.GetType(dbname)
        Dim url As New ConnectionUri With {
            .Database = dbname,
            .IPAddress = host,
            .Password = password,
            .Port = port,
            .User = user_name
        }
        Dim db As IDatabase = Activator.CreateInstance(type:=type.raw, url)

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
End Module
