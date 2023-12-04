Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri

Public Class mysql : Inherits graphdbMySQL

    Public ReadOnly Property knowledge_cache As Model

    Public Sub New(uri As ConnectionUri)
        MyBase.New(uri)
        knowledge_cache = New Model("knowledge_cache", uri)
    End Sub

End Class
