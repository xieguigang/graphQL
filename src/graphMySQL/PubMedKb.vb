Imports graph.MySQL.pubmed
Imports Oracle.LinuxCompatibility.MySQL.Uri

Public Class PubMedKb : Inherits db_pubmed

    Public Sub New(mysqli As ConnectionUri)
        MyBase.New(mysqli)
    End Sub
End Class
