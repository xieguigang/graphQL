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
<Oracle.LinuxCompatibility.MySQL.Reflection.DbAttributes.TableName("mesh", Database:="pubmed", SchemaSQL:="
CREATE TABLE IF NOT EXISTS `mesh` (
  `id` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `mesh_id` VARCHAR(45) NOT NULL,
  `term` VARCHAR(255) NOT NULL COMMENT 'the pubmed mesh term',
  `category` VARCHAR(128) NULL,
  `db_xref` VARCHAR(64) NULL COMMENT 'the external database reference id of current mesh term',
  `db_name` VARCHAR(45) NULL COMMENT 'the  name of the external database',
  `add_time` DATETIME NOT NULL DEFAULT now(),
  `description` LONGTEXT NULL COMMENT 'the description text of current mesh term',
  PRIMARY KEY (`id`))
ENGINE = InnoDB;
")>
Public Class mesh: Inherits Oracle.LinuxCompatibility.MySQL.MySQLTable
#Region "Public Property Mapping To Database Fields"
    <DatabaseField("id"), PrimaryKey, AutoIncrement, NotNull, DataType(MySqlDbType.UInt32, "11"), Column(Name:="id"), XmlAttribute> Public Property id As UInteger
    <DatabaseField("mesh_id"), NotNull, DataType(MySqlDbType.VarChar, "45"), Column(Name:="mesh_id")> Public Property mesh_id As String
''' <summary>
''' the pubmed mesh term
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("term"), NotNull, DataType(MySqlDbType.VarChar, "255"), Column(Name:="term")> Public Property term As String
    <DatabaseField("category"), DataType(MySqlDbType.VarChar, "128"), Column(Name:="category")> Public Property category As String
''' <summary>
''' the external database reference id of current mesh term
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("db_xref"), DataType(MySqlDbType.VarChar, "64"), Column(Name:="db_xref")> Public Property db_xref As String
''' <summary>
''' the  name of the external database
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("db_name"), DataType(MySqlDbType.VarChar, "45"), Column(Name:="db_name")> Public Property db_name As String
    <DatabaseField("add_time"), NotNull, DataType(MySqlDbType.DateTime), Column(Name:="add_time")> Public Property add_time As Date
''' <summary>
''' the description text of current mesh term
''' </summary>
''' <value></value>
''' <returns></returns>
''' <remarks></remarks>
    <DatabaseField("description"), DataType(MySqlDbType.Text), Column(Name:="description")> Public Property description As String
#End Region
#Region "Public SQL Interface"
#Region "Interface SQL"
    Friend Shared ReadOnly INSERT_SQL$ = 
        <SQL>INSERT INTO `mesh` (`mesh_id`, `term`, `category`, `db_xref`, `db_name`, `add_time`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly INSERT_AI_SQL$ = 
        <SQL>INSERT INTO `mesh` (`id`, `mesh_id`, `term`, `category`, `db_xref`, `db_name`, `add_time`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly REPLACE_SQL$ = 
        <SQL>REPLACE INTO `mesh` (`mesh_id`, `term`, `category`, `db_xref`, `db_name`, `add_time`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');</SQL>

    Friend Shared ReadOnly REPLACE_AI_SQL$ = 
        <SQL>REPLACE INTO `mesh` (`id`, `mesh_id`, `term`, `category`, `db_xref`, `db_name`, `add_time`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');</SQL>

    Friend Shared ReadOnly DELETE_SQL$ =
        <SQL>DELETE FROM `mesh` WHERE `id` = '{0}';</SQL>

    Friend Shared ReadOnly UPDATE_SQL$ = 
        <SQL>UPDATE `mesh` SET `id`='{0}', `mesh_id`='{1}', `term`='{2}', `category`='{3}', `db_xref`='{4}', `db_name`='{5}', `add_time`='{6}', `description`='{7}' WHERE `id` = '{8}';</SQL>

#End Region

''' <summary>
''' ```SQL
''' DELETE FROM `mesh` WHERE `id` = '{0}';
''' ```
''' </summary>
    Public Overrides Function GetDeleteSQL() As String
        Return String.Format(DELETE_SQL, id)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `mesh` (`id`, `mesh_id`, `term`, `category`, `db_xref`, `db_name`, `add_time`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL() As String
        Return String.Format(INSERT_SQL, mesh_id, term, category, db_xref, db_name, MySqlScript.ToMySqlDateTimeString(add_time), description)
    End Function

''' <summary>
''' ```SQL
''' INSERT INTO `mesh` (`id`, `mesh_id`, `term`, `category`, `db_xref`, `db_name`, `add_time`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetInsertSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(INSERT_AI_SQL, id, mesh_id, term, category, db_xref, db_name, MySqlScript.ToMySqlDateTimeString(add_time), description)
        Else
        Return String.Format(INSERT_SQL, mesh_id, term, category, db_xref, db_name, MySqlScript.ToMySqlDateTimeString(add_time), description)
        End If
    End Function

''' <summary>
''' <see cref="GetInsertSQL"/>
''' </summary>
    Public Overrides Function GetDumpInsertValue(AI As Boolean) As String
        If AI Then
            Return $"('{id}', '{mesh_id}', '{term}', '{category}', '{db_xref}', '{db_name}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}', '{description}')"
        Else
            Return $"('{mesh_id}', '{term}', '{category}', '{db_xref}', '{db_name}', '{add_time.ToString("yyyy-MM-dd hh:mm:ss")}', '{description}')"
        End If
    End Function


''' <summary>
''' ```SQL
''' REPLACE INTO `mesh` (`id`, `mesh_id`, `term`, `category`, `db_xref`, `db_name`, `add_time`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL() As String
        Return String.Format(REPLACE_SQL, mesh_id, term, category, db_xref, db_name, MySqlScript.ToMySqlDateTimeString(add_time), description)
    End Function

''' <summary>
''' ```SQL
''' REPLACE INTO `mesh` (`id`, `mesh_id`, `term`, `category`, `db_xref`, `db_name`, `add_time`, `description`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');
''' ```
''' </summary>
    Public Overrides Function GetReplaceSQL(AI As Boolean) As String
        If AI Then
        Return String.Format(REPLACE_AI_SQL, id, mesh_id, term, category, db_xref, db_name, MySqlScript.ToMySqlDateTimeString(add_time), description)
        Else
        Return String.Format(REPLACE_SQL, mesh_id, term, category, db_xref, db_name, MySqlScript.ToMySqlDateTimeString(add_time), description)
        End If
    End Function

''' <summary>
''' ```SQL
''' UPDATE `mesh` SET `id`='{0}', `mesh_id`='{1}', `term`='{2}', `category`='{3}', `db_xref`='{4}', `db_name`='{5}', `add_time`='{6}', `description`='{7}' WHERE `id` = '{8}';
''' ```
''' </summary>
    Public Overrides Function GetUpdateSQL() As String
        Return String.Format(UPDATE_SQL, id, mesh_id, term, category, db_xref, db_name, MySqlScript.ToMySqlDateTimeString(add_time), description, id)
    End Function
#End Region

''' <summary>
                     ''' Memberwise clone of current table Object.
                     ''' </summary>
                     Public Function Clone() As mesh
                         Return DirectCast(MyClass.MemberwiseClone, mesh)
                     End Function
End Class


End Namespace
