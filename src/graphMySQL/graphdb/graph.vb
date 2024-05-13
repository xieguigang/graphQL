#Region "Microsoft.VisualBasic::5b67ea33e1e950da6f266bdbc17d3597, src\graphMySQL\graphdb\graph.vb"

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

    '   Total Lines: 215
    '    Code Lines: 91
    ' Comment Lines: 102
    '   Blank Lines: 22
    '     File Size: 10.02 KB


    ' Class graph
    ' 
    '     Properties: add_time, from_node, id, link_type, note
    '                 to_node, weight
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
''' DROP TABLE IF EXISTS `graph`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!50503 SET character_set_client = utf8mb4 */;
''' CREATE TABLE `graph` (
'''   `id` int unsigned NOT NULL AUTO_INCREMENT,
'''   `from_node` int unsigned NOT NULL COMMENT 'the unique id of the knowledge data',
'''   `to_node` int unsigned NOT NULL COMMENT 'the unique id of the knowledge data',
'''   `link_type` int unsigned NOT NULL COMMENT 'the connection type between the two knowdge node, the enumeration text string value could be found in the knowledge vocabulary table',
'''   `weight` double unsigned NOT NULL DEFAULT '0' COMMENT 'weight value of current connection link',
'''   `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'the create time of the current knowledge link',
'''   `note` text COMMENT 'description text about current knowledge link',
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `id_UNIQUE` (`id`),
'''   KEY `source_index` (`from_node`),
'''   KEY `target_index` (`to_node`),
'''   KEY `term_index` (`link_type`),
'''   KEY `node_data_idx2` (`to_node`,`from_node`),
'''   KEY `node_data_idx` (`from_node`,`to_node`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=11502735 DEFAULT CHARSET=utf8mb3 COMMENT='the connection links between the knowledge nodes data';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("graph", Database:="graphql", SchemaSQL:="
CREATE TABLE `graph` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `from_node` int unsigned NOT NULL COMMENT 'the unique id of the knowledge data',
  `to_node` int unsigned NOT NULL COMMENT 'the unique id of the knowledge data',
  `link_type` int unsigned NOT NULL COMMENT 'the connection type between the two knowdge node, the enumeration text string value could be found in the knowledge vocabulary table',
  `weight` double unsigned NOT NULL DEFAULT '0' COMMENT 'weight value of current connection link',
  `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'the create time of the current knowledge link',
  `note` text COMMENT 'description text about current knowledge link',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `source_index` (`from_node`),
  KEY `target_index` (`to_node`),
  KEY `term_index` (`link_type`),
  KEY `node_data_idx2` (`to_node`,`from_node`),
  KEY `node_data_idx` (`from_node`,`to_node`)
) ENGINE=InnoDB AUTO_INCREMENT=11502735 DEFAULT CHARSET=utf8mb3 COMMENT='the connection links between the knowledge nodes data';
")>
Public Class graph: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="id"), XmlAttribute> Public Property id As UInteger
''' <summary>
''' the unique id of the knowledge data
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("from_node"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="from_node")> Public Property from_node As UInteger
''' <summary>
''' the unique id of the knowledge data
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("to_node"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="to_node")> Public Property to_node As UInteger
''' <summary>
''' the connection type between the two knowdge node, the enumeration text string value could be found in the knowledge vocabulary table
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("link_type"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="link_type")> Public Property link_type As UInteger
''' <summary>
''' weight value of current connection link
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("weight"), NotNull, DataType(MySqlDbType.Double), Column(Name:="weight")> Public Property weight As Double
''' <summary>
''' the create time of the current knowledge link
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("add_time"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="add_time")> Public Property add_time As Date
''' <summary>
''' description text about current knowledge link
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("note"), DataType(MySqlDbType.Text), Column(Name:="note")> Public Property note As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `graph` (`from_node`, `to_node`, `link_type`, `weight`, `add_time`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `graph` (`id`, `from_node`, `to_node`, `link_type`, `weight`, `add_time`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `graph` (`from_node`, `to_node`, `link_type`, `weight`, `add_time`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `graph` (`id`, `from_node`, `to_node`, `link_type`, `weight`, `add_time`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `graph` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `graph` SET `id`='{0}', `from_node`='{1}', `to_node`='{2}', `link_type`='{3}', `weight`='{4}', `add_time`='{5}', `note`='{6}' WHERE `id` = '{7}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `graph` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `graph` (`id`, `from_node`, `to_node`, `link_type`, `weight`, `add_time`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, from_node, to_node, link_type, weight, MySqlScript.ToMySqlDateTimeString(add_time), note)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `graph` (`id`, `from_node`, `to_node`, `link_type`, `weight`, `add_time`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, from_node, to_node, link_type, weight, MySqlScript.ToMySqlDateTimeString(add_time), note)
        Else
        Return String.Format(INSERT_SQL, from_node, to_node, link_type, weight, MySqlScript.ToMySqlDateTimeString(add_time), note)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{from_node}', '{to_node}', '{link_type}', '{weight}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}', '{note}')"
        Else
            Return $"('{from_node}', '{to_node}', '{link_type}', '{weight}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}', '{note}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `graph` (`id`, `from_node`, `to_node`, `link_type`, `weight`, `add_time`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, from_node, to_node, link_type, weight, MySqlScript.ToMySqlDateTimeString(add_time), note)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `graph` (`id`, `from_node`, `to_node`, `link_type`, `weight`, `add_time`, `note`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, from_node, to_node, link_type, weight, MySqlScript.ToMySqlDateTimeString(add_time), note)
        Else
        Return String.Format(REPLACE_SQL, from_node, to_node, link_type, weight, MySqlScript.ToMySqlDateTimeString(add_time), note)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `graph` SET `id`='{0}', `from_node`='{1}', `to_node`='{2}', `link_type`='{3}', `weight`='{4}', `add_time`='{5}', `note`='{6}' WHERE `id` = '{7}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, from_node, to_node, link_type, weight, MySqlScript.ToMySqlDateTimeString(add_time), note, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As graph
                         Return DirectCast(MyClass.MemberwiseClone, graph)
                     End Function
End Class


End Namespace
