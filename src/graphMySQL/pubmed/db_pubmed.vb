Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri

Namespace pubmed

Public MustInherit Class db_pubmed : Inherits IDatabase
Protected ReadOnly m_articles As Model
Protected ReadOnly m_fulltext As Model
Protected ReadOnly m_mesh As Model
Protected ReadOnly m_metadata As Model
Protected Sub New(mysqli As ConnectionUri)
Call MyBase.New(mysqli)

Me.m_articles = model(Of articles)()
Me.m_fulltext = model(Of fulltext)()
Me.m_mesh = model(Of mesh)()
Me.m_metadata = model(Of metadata)()
End Sub
End Class

End Namespace
