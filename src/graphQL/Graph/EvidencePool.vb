
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq

Namespace Graph

    Public Class EvidencePool

        ReadOnly category As Index(Of String)
        ReadOnly referenceData As Index(Of String)

        Default Public ReadOnly Property evidenceTerm(i As Integer) As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return referenceData(index:=i)
            End Get
        End Property

        Sub New(category As IEnumerable(Of String), referenceData As IEnumerable(Of String))
            Me.category = category.Indexing
            Me.referenceData = referenceData.Indexing
        End Sub

        Private Shared Function push(ByRef index As Index(Of String), data As String) As Integer
            If Not data Like index Then
                Call index.Add(data)
            End If

            Return index(data)
        End Function

        Public Function Join(exists As Evidence, xrefs As IEnumerable(Of String)) As Evidence
            Dim pointers As Integer() = xrefs _
                .Select(Function(ref)
                            Return push(referenceData, data:=ref)
                        End Function) _
                .ToArray

            exists.reference = exists.reference _
                .JoinIterates(pointers) _
                .Distinct _
                .ToArray

            Return exists
        End Function

        Public Function FindEvidence(term As Knowledge, category As String) As Evidence
            If Not category Like Me.category Then
                Return Nothing
            End If

            Dim ref As Integer = Me.category(category)

            For i As Integer = 0 To term.evidence.Count - 1
                If term.evidence(i).category = ref Then
                    Return term.evidence(i)
                End If
            Next

            Return Nothing
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CreateEvidence(category As String, xrefs As IEnumerable(Of String)) As Evidence
            Return New Evidence With {
                .category = push(Me.category, category),
                .reference = (From refer As String
                              In xrefs
                              Let rid As Integer = push(Me.referenceData, refer)
                              Select rid).ToArray
            }
        End Function

    End Class

    Public Class Evidence

        Public Property category As Integer
        Public Property reference As Integer()

        Public Overrides Function ToString() As String
            Return $"[{category}] {reference.JoinBy(", ")}"
        End Function

        Public Shared Function Union(evidences As IEnumerable(Of Evidence)) As IEnumerable(Of Evidence)
            Return From i As Evidence
                   In evidences
                   Group By i.category
                   Into Group
                   Let currentGroup = Group.ToArray
                   Let unionRefer = currentGroup _
                       .Select(Function(i) i.reference) _
                       .IteratesALL _
                       .Distinct _
                       .ToArray
                   Let categoryId As Integer = currentGroup.First.category
                   Select New Evidence With {
                       .category = categoryId,
                       .reference = unionRefer
                   }
        End Function

    End Class
End Namespace