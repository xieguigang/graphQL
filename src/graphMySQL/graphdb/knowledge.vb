#Region "Microsoft.VisualBasic::f78e55f28a39f5e165f218c7b70b4c03, src\graphMySQL\graphdb\knowledge.vb"

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

    '   Total Lines: 238
    '    Code Lines: 97
    ' Comment Lines: 119
    '   Blank Lines: 22
    '     File Size: 12.34 KB


    ' Class knowledge
    ' 
    '     Properties: add_time, description, display_title, graph_size, id
    '                 key, knowledge_term, node_type
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
''' DROP TABLE IF EXISTS `knowledge`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!50503 SET character_set_client = utf8mb4 */;
''' CREATE TABLE `knowledge` (
'''   `id` int unsigned NOT NULL AUTO_INCREMENT COMMENT 'usually be the FN-1a hashcode of the ''key + node_type'' term',
'''   `key` char(32) NOT NULL COMMENT 'the unique key of current knowledge node data, md5 value of the lcase(key)',
'''   `display_title` varchar(4096) NOT NULL COMMENT 'the raw text of current knowledge node data',
'''   `node_type` int unsigned NOT NULL COMMENT 'the node type enumeration number value, string value could be found in the knowledge vocabulary table',
'''   `graph_size` int unsigned NOT NULL DEFAULT '0' COMMENT 'the number of connected links to current knowledge node',
'''   `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'add time of current knowledge node data',
'''   `knowledge_term` int unsigned NOT NULL DEFAULT '0' COMMENT 'default zero means not assigned, and any positive integer means this property data has been assigned to a specific knowledge',
'''   `description` longtext NOT NULL COMMENT 'the description text about current knowledge data',
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `id_UNIQUE` (`id`) /*!80000 INVISIBLE */,
'''   KEY `key_index` (`key`),
'''   KEY `type_index` (`node_type`),
'''   KEY `find_key` (`key`,`node_type`),
'''   KEY `sort_count` (`graph_size`) /*!80000 INVISIBLE */,
'''   KEY `sort_time` (`add_time`),
'''   KEY `check_term` (`knowledge_term`),
'''   KEY `query_next_term` (`knowledge_term`,`node_type`),
'''   KEY `link_term` (`id`,`node_type`,`knowledge_term`),
'''   KEY `sort_count_desc` (`graph_size` DESC)
''' ) ENGINE=InnoDB AUTO_INCREMENT=7617576 DEFAULT CHARSET=utf8mb3 COMMENT='knowlege data pool';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("knowledge", Database:="graphql", SchemaSQL:="
CREATE TABLE `knowledge` (
  `id` int unsigned NOT NULL AUTO_INCREMENT COMMENT 'usually be the FN-1a hashcode of the ''key + node_type'' term',
  `key` char(32) NOT NULL COMMENT 'the unique key of current knowledge node data, md5 value of the lcase(key)',
  `display_title` varchar(4096) NOT NULL COMMENT 'the raw text of current knowledge node data',
  `node_type` int unsigned NOT NULL COMMENT 'the node type enumeration number value, string value could be found in the knowledge vocabulary table',
  `graph_size` int unsigned NOT NULL DEFAULT '0' COMMENT 'the number of connected links to current knowledge node',
  `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'add time of current knowledge node data',
  `knowledge_term` int unsigned NOT NULL DEFAULT '0' COMMENT 'default zero means not assigned, and any positive integer means this property data has been assigned to a specific knowledge',
  `description` longtext NOT NULL COMMENT 'the description text about current knowledge data',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`) /*!80000 INVISIBLE */,
  KEY `key_index` (`key`),
  KEY `type_index` (`node_type`),
  KEY `find_key` (`key`,`node_type`),
  KEY `sort_count` (`graph_size`) /*!80000 INVISIBLE */,
  KEY `sort_time` (`add_time`),
  KEY `check_term` (`knowledge_term`),
  KEY `query_next_term` (`knowledge_term`,`node_type`),
  KEY `link_term` (`id`,`node_type`,`knowledge_term`),
  KEY `sort_count_desc` (`graph_size` DESC)
) ENGINE=InnoDB AUTO_INCREMENT=7617576 DEFAULT CHARSET=utf8mb3 COMMENT='knowlege data pool';
")>
Public Class knowledge: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' usually be the FN-1a hashcode of the ''key + node_type'' term
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="id"), XmlAttribute> Public Property id As UInteger
''' <summary>
''' the unique key of current knowledge node data, md5 value of the lcase(key)
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("key"), NotNull, DataType(MySqlDbType.VarChar, "32"), Column(Name:="key")> Public Property key As String
''' <summary>
''' the raw text of current knowledge node data
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("display_title"), NotNull, DataType(MySqlDbType.VarChar, "4096"), Column(Name:="display_title")> Public Property display_title As String
''' <summary>
''' the node type enumeration number value, string value could be found in the knowledge vocabulary table
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("node_type"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="node_type")> Public Property node_type As UInteger
''' <summary>
''' the number of connected links to current knowledge node
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("graph_size"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="graph_size")> Public Property graph_size As UInteger
''' <summary>
''' add time of current knowledge node data
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("add_time"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="add_time")> Public Property add_time As Date
''' <summary>
''' default zero means not assigned, and any positive integer means this property data has been assigned to a specific knowledge
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("knowledge_term"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="knowledge_term")> Public Property knowledge_term As UInteger
''' <summary>
''' the description text about current knowledge data
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("description"), NotNull, DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `knowledge` (`key`, `display_title`, `node_type`, `graph_size`, `add_time`, `knowledge_term`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `knowledge` (`id`, `key`, `display_title`, `node_type`, `graph_size`, `add_time`, `knowledge_term`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `knowledge` (`key`, `display_title`, `node_type`, `graph_size`, `add_time`, `knowledge_term`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `knowledge` (`id`, `key`, `display_title`, `node_type`, `graph_size`, `add_time`, `knowledge_term`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `knowledge` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `knowledge` SET `id`='{0}', `key`='{1}', `display_title`='{2}', `node_type`='{3}', `graph_size`='{4}', `add_time`='{5}', `knowledge_term`='{6}', `description`='{7}' WHERE `id` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `knowledge` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `knowledge` (`id`, `key`, `display_title`, `node_type`, `graph_size`, `add_time`, `knowledge_term`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, key, display_title, node_type, graph_size, MySqlScript.ToMySqlDateTimeString(add_time), knowledge_term, description)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `knowledge` (`id`, `key`, `display_title`, `node_type`, `graph_size`, `add_time`, `knowledge_term`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, key, display_title, node_type, graph_size, MySqlScript.ToMySqlDateTimeString(add_time), knowledge_term, description)
        Else
        Return String.Format(INSERT_SQL, key, display_title, node_type, graph_size, MySqlScript.ToMySqlDateTimeString(add_time), knowledge_term, description)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{key}', '{display_title}', '{node_type}', '{graph_size}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}', '{knowledge_term}', '{description}')"
        Else
            Return $"('{key}', '{display_title}', '{node_type}', '{graph_size}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}', '{knowledge_term}', '{description}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `knowledge` (`id`, `key`, `display_title`, `node_type`, `graph_size`, `add_time`, `knowledge_term`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, key, display_title, node_type, graph_size, MySqlScript.ToMySqlDateTimeString(add_time), knowledge_term, description)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `knowledge` (`id`, `key`, `display_title`, `node_type`, `graph_size`, `add_time`, `knowledge_term`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, key, display_title, node_type, graph_size, MySqlScript.ToMySqlDateTimeString(add_time), knowledge_term, description)
        Else
        Return String.Format(REPLACE_SQL, key, display_title, node_type, graph_size, MySqlScript.ToMySqlDateTimeString(add_time), knowledge_term, description)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `knowledge` SET `id`='{0}', `key`='{1}', `display_title`='{2}', `node_type`='{3}', `graph_size`='{4}', `add_time`='{5}', `knowledge_term`='{6}', `description`='{7}' WHERE `id` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, key, display_title, node_type, graph_size, MySqlScript.ToMySqlDateTimeString(add_time), knowledge_term, description, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As knowledge
                         Return DirectCast(MyClass.MemberwiseClone, knowledge)
                     End Function
End Class


End Namespace
