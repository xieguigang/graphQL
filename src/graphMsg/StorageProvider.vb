Imports System.IO
Imports graphQL
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.Data.IO.MessagePack.Serialization
Imports Microsoft.VisualBasic.Language

Public Class StorageProvider

    <MessagePackMember(0)> Public Property terms As KnowledgeMsg()
    <MessagePackMember(1)> Public Property links As LinkMsg()

    Public Shared Function Save(kb As GraphPool, file As Stream) As Boolean
        Dim terms = KnowledgeMsg.GetTerms(kb).ToArray
        Dim links = LinkMsg.GetRelationships(kb).ToArray
        Dim pack As New StorageProvider With {
            .links = links,
            .terms = terms
        }

        Try
            Call MsgPackSerializer.SerializeObject(pack, file)
            Call file.Flush()
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Public Shared Function Open(file As Stream) As GraphPool
        If file Is Nothing Then
            Return New GraphPool({}, {})
        Else
            Return CreateQuery(MsgPackSerializer.Deserialize(Of StorageProvider)(file))
        End If
    End Function

    Public Shared Function CreateQuery(pack As StorageProvider) As GraphPool
        Dim terms As New Dictionary(Of String, Knowledge)
        Dim links As New List(Of Association)

        For Each v As KnowledgeMsg In pack.terms
            terms(v.guid.ToString) = New Knowledge With {
                .ID = v.guid,
                .label = v.term
            }
        Next
        For Each l As LinkMsg In pack.links
            links += New Association With {
                .type = l.type,
                .U = terms(l.u.ToString),
                .V = terms(l.v.ToString),
                .weight = l.weight
            }
        Next

        Return New GraphPool(terms, links)
    End Function

End Class
