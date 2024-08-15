Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports Oracle.LinuxCompatibility.MySQL.Uri

Namespace graphdb

Public MustInherit Class db_graphdb : Inherits IDatabase
Protected ReadOnly m_graph As Model
Protected ReadOnly m_hash_index As Model
Protected ReadOnly m_knowledge As Model
Protected ReadOnly m_knowledge_cache As Model
Protected ReadOnly m_knowledge_vocabulary As Model
Protected Sub New(mysqli As ConnectionUri)
Call MyBase.New(mysqli)

Me.m_graph = model(Of graph)()
Me.m_hash_index = model(Of hash_index)()
Me.m_knowledge = model(Of knowledge)()
Me.m_knowledge_cache = model(Of knowledge_cache)()
Me.m_knowledge_vocabulary = model(Of knowledge_vocabulary)()
End Sub
End Class

End Namespace
