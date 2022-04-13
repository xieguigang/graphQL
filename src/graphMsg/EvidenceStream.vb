Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports graphMsg.Message
Imports graphQL.Graph

Module EvidenceStream

    <Extension>
    Public Function Load(zip As ZipArchive) As EvidencePool
        Dim index As IndexByRef = StorageProvider.GetKeywords("meta/evidences.msg", pack:=zip)
        Dim pool As New EvidencePool(index.types, index.source)

        Return pool
    End Function

End Module
