import graphQL

kb = MsgFile::open()

for file in list.files("K:\metadb\graphDb", pattern = "*.graph"):   
    kb = Query::join(kb, MsgFile::open(file))

MsgFile::save(kb, file = "K:\metadb/metadb.graph")
