Imports System.IO
Imports graphQL

Public Class StorageProvider

    Public Shared Function Open(file As Stream) As GraphPool
        If file Is Nothing Then
            Return New GraphPool({}, {})
        Else

        End If
    End Function

End Class
