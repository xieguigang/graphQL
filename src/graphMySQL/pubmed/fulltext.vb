REM  Oracle.LinuxCompatibility.MySQL.CodeSolution.VisualBasic.CodeGenerator
REM  MYSQL Schema Mapper
REM      for Microsoft VisualBasic.NET 1.0.0.0

REM  Dump @9/23/2024 08:33:08 AM


Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes
Imports MySqlScript = Oracle.LinuxCompatibility.MySQL.Scripting.Extensions

Namespace pubmed

''' <summary>
''' ```SQL
''' ```
''' </summary>
''' <remarks></remarks>
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("fulltext", Database:="pubmed", SchemaSQL:="
CREATE TABLE IF NOT EXISTS `fulltext` (
  `id` INT UNSIGNED NOT NULL COMMENT 'the pubmed id',
  `abstract` MEDIUMTEXT NULL,
  `fulltext` LONGTEXT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB;
")>
Public Class fulltext: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' the pubmed id
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="id"), XmlAttribute> Public Property id As UInteger
    <DatabaseField("abstract"), DataType(MySqlDbType.Text), Column(Name:="abstract")> Public Property abstract As String
    <DatabaseField("fulltext"), DataType(MySqlDbType.Text), Column(Name:="fulltext")> Public Property fulltext As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `fulltext` (`id`, `abstract`, `fulltext`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `fulltext` (`id`, `abstract`, `fulltext`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `fulltext` (`id`, `abstract`, `fulltext`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `fulltext` (`id`, `abstract`, `fulltext`) VALUES ('{0}', '{1}', '{2}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `fulltext` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `fulltext` SET `id`='{0}', `abstract`='{1}', `fulltext`='{2}' WHERE `id` = '{3}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `fulltext` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `fulltext` (`id`, `abstract`, `fulltext`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, abstract, fulltext)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `fulltext` (`id`, `abstract`, `fulltext`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, abstract, fulltext)
        Else
        Return String.Format(INSERT_SQL, id, abstract, fulltext)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{abstract}', '{fulltext}')"
        Else
            Return $"('{id}', '{abstract}', '{fulltext}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `fulltext` (`id`, `abstract`, `fulltext`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, abstract, fulltext)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `fulltext` (`id`, `abstract`, `fulltext`) VALUES ('{0}', '{1}', '{2}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, abstract, fulltext)
        Else
        Return String.Format(REPLACE_SQL, id, abstract, fulltext)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `fulltext` SET `id`='{0}', `abstract`='{1}', `fulltext`='{2}' WHERE `id` = '{3}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, abstract, fulltext, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As fulltext
                         Return DirectCast(MyClass.MemberwiseClone, fulltext)
                     End Function
End Class


End Namespace
