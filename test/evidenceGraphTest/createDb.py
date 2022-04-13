import graphQL

setwd(@dir)

kegg  = read.csv("../knowledges/kegg.csv", row.names = None)
hmdb  = read.csv("../knowledges/hmdb.csv", row.names = None)
chebi = read.csv("../knowledges/chebi.csv", row.names = None)

print(kegg, max.print = 6)
# print(hmdb, max.print = 6)
# print(chebi, max.print = 6)

def process_knowledge(kb, data, type):
    data = as.list(data, byrow = True)
    data = groupBy(data, x -> x[["key"]])
    data = lapply(data, x -> sapply(x, i -> i[["value"]]), x -> x[["key"]])
    
    str(data)
    
    Query::insert(kb, data[["id"]], type, data)


kb = MsgFile::open(evidenceAggregate = True)

process_knowledge(kb, kegg,"kegg")
process_knowledge(kb, hmdb,"hmdb")
process_knowledge(kb, chebi,"chebi")
process_knowledge(kb, read.csv("../knowledges/water.csv", row.names = None),"kegg")
process_knowledge(kb, read.csv("../knowledges/water_chebi.csv", row.names = None),"chebi")

kb2 = MsgFile::open(evidenceAggregate = True)


process_knowledge(kb, read.csv("../knowledges/leucine/hmdb.csv", row.names = None), "hmdb")
process_knowledge(kb, read.csv("../knowledges/leucine/kegg.csv", row.names = None), "kegg")

process_knowledge(kb2, read.csv("../knowledges/isoleucine/chebi.csv", row.names = None), "chebi")
process_knowledge(kb2, read.csv("../knowledges/isoleucine/kegg.csv", row.names = None), "kegg")
process_knowledge(kb2, read.csv("../knowledges/isoleucine/hmdb.csv", row.names = None), "hmdb")

# MsgFile::save(kb, file = "./kb1.graph")
# MsgFile::save(kb2, file = "./kb2.graph")

kb = Query::join(kb2, kb)

MsgFile::save(kb, file = "./aspirin.graph")
