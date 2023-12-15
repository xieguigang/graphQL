Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Public MustInherit Class SignatureVerifycation

    Public MustOverride Function Verify(seed As NetworkGraph, adjacent As NetworkGraph) As Boolean

End Class
