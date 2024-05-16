#Region "Microsoft.VisualBasic::97a37bd4528ead97a4ecc855eb0a66b7, src\graphMySQL\graphdb\knowledge_cache.vb"

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

    '   Total Lines: 175
    '    Code Lines: 89
    ' Comment Lines: 64
    '   Blank Lines: 22
    '     File Size: 7.96 KB


    ' Class knowledge_cache
    ' 
    '     Properties: add_time, hashcode, id, knowledge, note
    '                 seed_id, term
    ' 
    '     Function: Clone, GetDeleteSQL, GetDumpInsertValue, (+2 Overloads) GetInsertSQL, (+2 Overloads) GetReplaceSQL
    '               GetUpdateSQL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @2/28/2024 01:11:44 PM


Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace graphdb

''' <summary>
''' ```SQL
''' 
''' --
''' 
''' DROP TABLE IF EXISTS `knowledge_cache`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!50503 SET character_set_client = utf8mb4 */;
''' CREATE TABLE `knowledge_cache` (
'''   `id` int unsigned NOT NULL AUTO_INCREMENT,
'''   `seed_id` int unsigned NOT NULL,
'''   `term` varchar(4096) NOT NULL,
'''   `hashcode` int unsigned NOT NULL,
'''   `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
'''   `knowledge` longtext NOT NULL,
'''   `note` text,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `id_UNIQUE` (`id`),
'''   UNIQUE KEY `seed_id_UNIQUE` (`seed_id`),
'''   KEY `term_index` (`hashcode`),
'''   KEY `sort_time` (`add_time`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("knowledge_cache", Database:="graphql", SchemaSQL:="
CREATE TABLE `knowledge_cache` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `seed_id` int unsigned NOT NULL,
  `term` varchar(4096) NOT NULL,
  `hashcode` int unsigned NOT NULL,
  `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `knowledge` longtext NOT NULL,
  `note` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  UNIQUE KEY `seed_id_UNIQUE` (`seed_id`),
  KEY `term_index` (`hashcode`),
  KEY `sort_time` (`add_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
")>
Public Class knowledge_cache: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="id"), XmlAttribute> Public Property id As UInteger
    <DatabaseField("seed_id"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="seed_id")> Public Property seed_id As UInteger
    <DatabaseField("term"), NotNull, DataType(MySqlDbType.VarChar, "4096"), Column(Name:="term")> Public Property term As String
    <DatabaseField("hashcode"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="hashcode")> Public Property hashcode As UInteger
    <DatabaseField("add_time"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="add_time")> Public Property add_time As Date
    <DatabaseField("knowledge"), NotNull, DataType(MySqlDbType.Text), Column(Name:="knowledge")> Public Property knowledge As String
    <DatabaseField("note"), DataType(MySqlDbType.Text), Column(Name:="note")> Public Property note As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `knowledge_cache` (`seed_id`, `term`, `hashcode`, `add_time`, `knowledge`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `knowledge_cache` (`id`, `seed_id`, `term`, `hashcode`, `add_time`, `knowledge`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `knowledge_cache` (`seed_id`, `term`, `hashcode`, `add_time`, `knowledge`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `knowledge_cache` (`id`, `seed_id`, `term`, `hashcode`, `add_time`, `knowledge`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `knowledge_cache` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `knowledge_cache` SET `id`='{0}', `seed_id`='{1}', `term`='{2}', `hashcode`='{3}', `add_time`='{4}', `knowledge`='{5}', `note`='{6}' WHERE `id` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `knowledge_cache` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `knowledge_cache` (`id`, `seed_id`, `term`, `hashcode`, `add_time`, `knowledge`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, seed_id, term, hashcode, MySqlScript.ToMySqlDateTimeString(add_time), knowledge, note)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `knowledge_cache` (`id`, `seed_id`, `term`, `hashcode`, `add_time`, `knowledge`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, seed_id, term, hashcode, MySqlScript.ToMySqlDateTimeString(add_time), knowledge, note)
        Else
        Return String.Format(INSERT_SQL, seed_id, term, hashcode, MySqlScript.ToMySqlDateTimeString(add_time), knowledge, note)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{seed_id}', '{term}', '{hashcode}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}', '{knowledge}', '{note}')"
        Else
            Return $"('{seed_id}', '{term}', '{hashcode}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}', '{knowledge}', '{note}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `knowledge_cache` (`id`, `seed_id`, `term`, `hashcode`, `add_time`, `knowledge`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, seed_id, term, hashcode, MySqlScript.ToMySqlDateTimeString(add_time), knowledge, note)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `knowledge_cache` (`id`, `seed_id`, `term`, `hashcode`, `add_time`, `knowledge`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, seed_id, term, hashcode, MySqlScript.ToMySqlDateTimeString(add_time), knowledge, note)
        Else
        Return String.Format(REPLACE_SQL, seed_id, term, hashcode, MySqlScript.ToMySqlDateTimeString(add_time), knowledge, note)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `knowledge_cache` SET `id`='{0}', `seed_id`='{1}', `term`='{2}', `hashcode`='{3}', `add_time`='{4}', `knowledge`='{5}', `note`='{6}' WHERE `id` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, seed_id, term, hashcode, MySqlScript.ToMySqlDateTimeString(add_time), knowledge, note, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As knowledge_cache
                         Return DirectCast(MyClass.MemberwiseClone, knowledge_cache)
                     End Function
End Class


End Namespace
