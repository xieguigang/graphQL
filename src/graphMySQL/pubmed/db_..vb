Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri

Namespace pubmed

Public MustInherit Class db_. : Inherits IDatabase
Protected ReadOnly articles As Model
Protected ReadOnly fulltext As Model
Protected ReadOnly mesh As Model
Protected ReadOnly metadata As Model
Protected Sub New(mysqli As ConnectionUri)
Call MyBase.New(mysqli)

Me.articles = model(Of articles)()
Me.fulltext = model(Of fulltext)()
Me.mesh = model(Of mesh)()
Me.metadata = model(Of metadata)()
End Sub
End Class

End Namespace
