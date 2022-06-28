Imports System.IO
Imports System.Runtime.CompilerServices
Imports graphMsg.Message
Imports graphQL.Graph
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq

Module EvidenceLinks

    Friend Function SaveEvidence(evidences As EvidenceMsg(), pack As StreamPack) As Dictionary(Of String, Integer)
        Dim blocks = evidences _
            .GroupBy(Function(i)
                         Return i.ref.ToString.Last.ToString
                     End Function) _
            .ToArray
        Dim buffer As Stream
        Dim summary As New Dictionary(Of String, Integer)

        For Each block In blocks
            evidences = block.ToArray
            buffer = pack.OpenBlock($"evidences/{block.Key}.dat")

            Call MsgPackSerializer.SerializeObject(evidences, buffer, closeFile:=True)
            Call summary.Add(block.Key, evidences.Length)
        Next

        Return summary
    End Function

    ''' <summary>
    ''' attach evidence data for each knowledge terms
    ''' </summary>
    ''' 
    <Extension>
    Public Sub LoadEvidence(terms As Dictionary(Of String, Knowledge), pack As StreamPack)
        Dim files As StreamBlock() = pack.files

        For Each item As StreamBlock In files.Where(Function(t) t.fullName.StartsWith("/evidences"))
            Dim list = MsgPackSerializer.Deserialize(Of EvidenceMsg())(pack.OpenBlock(item))
            Dim evidences As IEnumerable(Of Evidence)

            For Each evi As EvidenceMsg In list
                evidences = evi.data _
                    .Select(Function(i)
                                Return New Evidence With {
                                    .category = i.ref,
                                    .reference = i.data
                                }
                            End Function) _
                    .ToArray

                ' terms(evi.ref.ToString).evidence.Clear()
                terms(evi.ref.ToString).evidence.AddRange(evidences)
            Next
        Next
    End Sub
End Module
