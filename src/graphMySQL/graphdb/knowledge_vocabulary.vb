#Region "Microsoft.VisualBasic::41a5b9f8702d71ca3635869be26646bc, src\graphMySQL\graphdb\knowledge_vocabulary.vb"

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

    '   Total Lines: 232
    '    Code Lines: 96
    ' Comment Lines: 114
    '   Blank Lines: 22
    '     File Size: 11.86 KB


    ' Class knowledge_vocabulary
    ' 
    '     Properties: add_time, ancestor, color, count, description
    '                 hashcode, id, level, vocabulary
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
''' DROP TABLE IF EXISTS `knowledge_vocabulary`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!50503 SET character_set_client = utf8mb4 */;
''' CREATE TABLE `knowledge_vocabulary` (
'''   `id` int unsigned NOT NULL AUTO_INCREMENT,
'''   `vocabulary` varchar(1024) NOT NULL COMMENT 'the short knowledge term type',
'''   `hashcode` varchar(32) NOT NULL,
'''   `ancestor` int unsigned NOT NULL DEFAULT '0' COMMENT 'the parent node of current ontology term',
'''   `level` int unsigned NOT NULL DEFAULT '0' COMMENT 'the level of current ontology tern node on the family tree',
'''   `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
'''   `color` char(7) NOT NULL DEFAULT '#123456' COMMENT 'html color code of current knowledge ontology term',
'''   `count` int unsigned NOT NULL DEFAULT '0' COMMENT 'hit counts in the knowledge table',
'''   `description` mediumtext COMMENT 'the description text value about current type of the knowledge term',
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `id_UNIQUE` (`id`),
'''   UNIQUE KEY `vocabulary_UNIQUE` (`vocabulary`),
'''   KEY `term_index` (`vocabulary`),
'''   KEY `color_index` (`color`),
'''   KEY `sort_count` (`count`),
'''   KEY `sort_time` (`add_time`),
'''   KEY `find_hash` (`hashcode`)
''' ) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb3 COMMENT='the knowledge term type, category or class data label. the word ontology class data table';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
''' 
''' /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
''' /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
''' /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
''' /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
''' /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
''' /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
''' /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
''' 
''' -- Dump completed on 2023-12-06  8:40:45
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("knowledge_vocabulary", Database:="graphql", SchemaSQL:="
CREATE TABLE `knowledge_vocabulary` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `vocabulary` varchar(1024) NOT NULL COMMENT 'the short knowledge term type',
  `hashcode` varchar(32) NOT NULL,
  `ancestor` int unsigned NOT NULL DEFAULT '0' COMMENT 'the parent node of current ontology term',
  `level` int unsigned NOT NULL DEFAULT '0' COMMENT 'the level of current ontology tern node on the family tree',
  `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `color` char(7) NOT NULL DEFAULT '#123456' COMMENT 'html color code of current knowledge ontology term',
  `count` int unsigned NOT NULL DEFAULT '0' COMMENT 'hit counts in the knowledge table',
  `description` mediumtext COMMENT 'the description text value about current type of the knowledge term',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  UNIQUE KEY `vocabulary_UNIQUE` (`vocabulary`),
  KEY `term_index` (`vocabulary`),
  KEY `color_index` (`color`),
  KEY `sort_count` (`count`),
  KEY `sort_time` (`add_time`),
  KEY `find_hash` (`hashcode`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb3 COMMENT='the knowledge term type, category or class data label. the word ontology class data table';
")>
Public Class knowledge_vocabulary: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="id"), XmlAttribute> Public Property id As UInteger
''' <summary>
''' the short knowledge term type
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("vocabulary"), NotNull, DataType(MySqlDbType.VarChar, "1024"), Column(Name:="vocabulary")> Public Property vocabulary As String
    <DatabaseField("hashcode"), NotNull, DataType(MySqlDbType.VarChar, "32"), Column(Name:="hashcode")> Public Property hashcode As String
''' <summary>
''' the parent node of current ontology term
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("ancestor"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="ancestor")> Public Property ancestor As UInteger
''' <summary>
''' the level of current ontology tern node on the family tree
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("level"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="level")> Public Property level As UInteger
    <DatabaseField("add_time"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="add_time")> Public Property add_time As Date
''' <summary>
''' html color code of current knowledge ontology term
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("color"), NotNull, DataType(MySqlDbType.VarChar, "7"), Column(Name:="color")> Public Property color As String
''' <summary>
''' hit counts in the knowledge table
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("count"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="count")> Public Property count As UInteger
''' <summary>
''' the description text value about current type of the knowledge term
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `knowledge_vocabulary` (`vocabulary`, `hashcode`, `ancestor`, `level`, `add_time`, `color`, `count`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `knowledge_vocabulary` (`id`, `vocabulary`, `hashcode`, `ancestor`, `level`, `add_time`, `color`, `count`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `knowledge_vocabulary` (`vocabulary`, `hashcode`, `ancestor`, `level`, `add_time`, `color`, `count`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `knowledge_vocabulary` (`id`, `vocabulary`, `hashcode`, `ancestor`, `level`, `add_time`, `color`, `count`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `knowledge_vocabulary` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `knowledge_vocabulary` SET `id`='{0}', `vocabulary`='{1}', `hashcode`='{2}', `ancestor`='{3}', `level`='{4}', `add_time`='{5}', `color`='{6}', `count`='{7}', `description`='{8}' WHERE `id` = '{9}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `knowledge_vocabulary` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `knowledge_vocabulary` (`id`, `vocabulary`, `hashcode`, `ancestor`, `level`, `add_time`, `color`, `count`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, vocabulary, hashcode, ancestor, level, MySqlScript.ToMySqlDateTimeString(add_time), color, count, description)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `knowledge_vocabulary` (`id`, `vocabulary`, `hashcode`, `ancestor`, `level`, `add_time`, `color`, `count`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, vocabulary, hashcode, ancestor, level, MySqlScript.ToMySqlDateTimeString(add_time), color, count, description)
        Else
        Return String.Format(INSERT_SQL, vocabulary, hashcode, ancestor, level, MySqlScript.ToMySqlDateTimeString(add_time), color, count, description)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{vocabulary}', '{hashcode}', '{ancestor}', '{level}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}', '{color}', '{count}', '{description}')"
        Else
            Return $"('{vocabulary}', '{hashcode}', '{ancestor}', '{level}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}', '{color}', '{count}', '{description}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `knowledge_vocabulary` (`id`, `vocabulary`, `hashcode`, `ancestor`, `level`, `add_time`, `color`, `count`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, vocabulary, hashcode, ancestor, level, MySqlScript.ToMySqlDateTimeString(add_time), color, count, description)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `knowledge_vocabulary` (`id`, `vocabulary`, `hashcode`, `ancestor`, `level`, `add_time`, `color`, `count`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, vocabulary, hashcode, ancestor, level, MySqlScript.ToMySqlDateTimeString(add_time), color, count, description)
        Else
        Return String.Format(REPLACE_SQL, vocabulary, hashcode, ancestor, level, MySqlScript.ToMySqlDateTimeString(add_time), color, count, description)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `knowledge_vocabulary` SET `id`='{0}', `vocabulary`='{1}', `hashcode`='{2}', `ancestor`='{3}', `level`='{4}', `add_time`='{5}', `color`='{6}', `count`='{7}', `description`='{8}' WHERE `id` = '{9}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, vocabulary, hashcode, ancestor, level, MySqlScript.ToMySqlDateTimeString(add_time), color, count, description, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As knowledge_vocabulary
                         Return DirectCast(MyClass.MemberwiseClone, knowledge_vocabulary)
                     End Function
End Class


End Namespace
