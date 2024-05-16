#Region "Microsoft.VisualBasic::8d7f9d21c9ce2bf5b4d49364cdd5104c, src\graphMySQL\graphdb\hash_index.vb"

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

    '   Total Lines: 177
    '    Code Lines: 83
    ' Comment Lines: 72
    '   Blank Lines: 22
    '     File Size: 7.37 KB


    ' Class hash_index
    ' 
    '     Properties: add_time, hashcode, id, map, synonym
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
''' DROP TABLE IF EXISTS `hash_index`;
''' /*!40101 SET @saved_cs_client     = @@character_set_client */;
''' /*!50503 SET character_set_client = utf8mb4 */;
''' CREATE TABLE `hash_index` (
'''   `id` int unsigned NOT NULL AUTO_INCREMENT,
'''   `synonym` text NOT NULL,
'''   `hashcode` char(32) NOT NULL COMMENT 'tolower(md5(tolower(synonym)))',
'''   `map` int unsigned NOT NULL COMMENT 'the synonym id mapping to the knowledge data term',
'''   `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
'''   PRIMARY KEY (`id`),
'''   UNIQUE KEY `id_UNIQUE` (`id`),
'''   KEY `hash_map` (`hashcode`)
''' ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='the synonym list of the knowledge data';
''' /*!40101 SET character_set_client = @saved_cs_client */;
''' 
''' --
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("hash_index", Database:="graphql", SchemaSQL:="
CREATE TABLE `hash_index` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `synonym` text NOT NULL,
  `hashcode` char(32) NOT NULL COMMENT 'tolower(md5(tolower(synonym)))',
  `map` int unsigned NOT NULL COMMENT 'the synonym id mapping to the knowledge data term',
  `add_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  KEY `hash_map` (`hashcode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='the synonym list of the knowledge data';
")>
Public Class hash_index: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="id"), XmlAttribute> Public Property id As UInteger
    <DatabaseField("synonym"), NotNull, DataType(MySqlDbType.Text), Column(Name:="synonym")> Public Property synonym As String
''' <summary>
''' tolower(md5(tolower(synonym)))
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("hashcode"), NotNull, DataType(MySqlDbType.VarChar, "32"), Column(Name:="hashcode")> Public Property hashcode As String
''' <summary>
''' the synonym id mapping to the knowledge data term
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("map"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="map")> Public Property map As UInteger
    <DatabaseField("add_time"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="add_time")> Public Property add_time As Date
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `hash_index` (`synonym`, `hashcode`, `map`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `hash_index` (`id`, `synonym`, `hashcode`, `map`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `hash_index` (`synonym`, `hashcode`, `map`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `hash_index` (`id`, `synonym`, `hashcode`, `map`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `hash_index` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `hash_index` SET `id`='{0}', `synonym`='{1}', `hashcode`='{2}', `map`='{3}', `add_time`='{4}' WHERE `id` = '{5}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `hash_index` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `hash_index` (`id`, `synonym`, `hashcode`, `map`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, synonym, hashcode, map, MySqlScript.ToMySqlDateTimeString(add_time))
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `hash_index` (`id`, `synonym`, `hashcode`, `map`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, synonym, hashcode, map, MySqlScript.ToMySqlDateTimeString(add_time))
        Else
        Return String.Format(INSERT_SQL, synonym, hashcode, map, MySqlScript.ToMySqlDateTimeString(add_time))
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{synonym}', '{hashcode}', '{map}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}')"
        Else
            Return $"('{synonym}', '{hashcode}', '{map}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `hash_index` (`id`, `synonym`, `hashcode`, `map`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, synonym, hashcode, map, MySqlScript.ToMySqlDateTimeString(add_time))
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `hash_index` (`id`, `synonym`, `hashcode`, `map`, `add_time`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, synonym, hashcode, map, MySqlScript.ToMySqlDateTimeString(add_time))
        Else
        Return String.Format(REPLACE_SQL, synonym, hashcode, map, MySqlScript.ToMySqlDateTimeString(add_time))
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `hash_index` SET `id`='{0}', `synonym`='{1}', `hashcode`='{2}', `map`='{3}', `add_time`='{4}' WHERE `id` = '{5}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, synonym, hashcode, map, MySqlScript.ToMySqlDateTimeString(add_time), id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As hash_index
                         Return DirectCast(MyClass.MemberwiseClone, hash_index)
                     End Function
End Class


End Namespace
