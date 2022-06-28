Imports System.Runtime.CompilerServices
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Module EvidenceStream

    <Extension>
    Public Function Load(pack As StreamPack) As EvidencePool
        Dim index As IndexByRef = StorageProvider.GetKeywords("meta/evidences.msg", pack:=pack)
        Dim pool As New EvidencePool(index.types, index.source)

        Return pool
    End Function

End Module
