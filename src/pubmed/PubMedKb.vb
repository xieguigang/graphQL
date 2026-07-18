Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports PubMedMirror.pubmed

Public Class PubMedKb : Inherits db_pubmed

    Public Sub New(mysqli As ConnectionUri)
        MyBase.New(mysqli)
    End Sub
End Class
