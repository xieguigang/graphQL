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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("articles", Database:="pubmed", SchemaSQL:="
CREATE TABLE IF NOT EXISTS `articles` (
  `id` INT UNSIGNED NOT NULL COMMENT 'the pubmed id',
  `authors` MEDIUMTEXT NULL,
  `title` VARCHAR(512) NOT NULL,
  `journal` VARCHAR(255) NOT NULL,
  `doi` VARCHAR(64) NULL COMMENT 'the doi reference of this article',
  `year` INT UNSIGNED NOT NULL COMMENT 'published year',
  PRIMARY KEY (`id`))
ENGINE = InnoDB;
")>
Public Class articles: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
''' <summary>
''' the pubmed id
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("id"), PrimaryKey, NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="id"), XmlAttribute> Public Property id As UInteger
    <DatabaseField("authors"), DataType(MySqlDbType.Text), Column(Name:="authors")> Public Property authors As String
    <DatabaseField("title"), NotNull, DataType(MySqlDbType.VarChar, "512"), Column(Name:="title")> Public Property title As String
    <DatabaseField("journal"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="journal")> Public Property journal As String
''' <summary>
''' the doi reference of this article
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("doi"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="doi")> Public Property doi As String
''' <summary>
''' published year
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("year"), NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="year")> Public Property year As UInteger
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `articles` (`id`, `authors`, `title`, `journal`, `doi`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `articles` (`id`, `authors`, `title`, `journal`, `doi`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `articles` (`id`, `authors`, `title`, `journal`, `doi`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `articles` (`id`, `authors`, `title`, `journal`, `doi`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `articles` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `articles` SET `id`='{0}', `authors`='{1}', `title`='{2}', `journal`='{3}', `doi`='{4}', `year`='{5}' WHERE `id` = '{6}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `articles` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `articles` (`id`, `authors`, `title`, `journal`, `doi`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, id, authors, title, journal, doi, year)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `articles` (`id`, `authors`, `title`, `journal`, `doi`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, authors, title, journal, doi, year)
        Else
        Return String.Format(INSERT_SQL, id, authors, title, journal, doi, year)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{authors}', '{title}', '{journal}', '{doi}', '{year}')"
        Else
            Return $"('{id}', '{authors}', '{title}', '{journal}', '{doi}', '{year}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `articles` (`id`, `authors`, `title`, `journal`, `doi`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, id, authors, title, journal, doi, year)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `articles` (`id`, `authors`, `title`, `journal`, `doi`, `year`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, authors, title, journal, doi, year)
        Else
        Return String.Format(REPLACE_SQL, id, authors, title, journal, doi, year)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `articles` SET `id`='{0}', `authors`='{1}', `title`='{2}', `journal`='{3}', `doi`='{4}', `year`='{5}' WHERE `id` = '{6}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, authors, title, journal, doi, year, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As articles
                         Return DirectCast(MyClass.MemberwiseClone, articles)
                     End Function
End Class


End Namespace
