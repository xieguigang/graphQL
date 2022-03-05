import graphQL

setwd(@dir)

kegg = read.csv("./kegg.csv", row.names = None)
hmdb = read.csv("./hmdb.csv", row.names = None)
chebi = read.csv("./chebi.csv", row.names = None)

print(kegg)
print(hmdb)
print(chebi)

def process_knowledge(kb, data):
    data = as.list(data, byrow = True)
    data = groupBy(data, x -> x[["key"]])
    data = lapply(data, x -> sapply(x, i -> i[["value"]]), x -> x[["key"]])
    
    str(data)
    
    Query::insert(kb, data[["id"]], data)


kb = MsgFile::open()

process_knowledge(kb, kegg)
process_knowledge(kb, hmdb)
process_knowledge(kb, chebi)

str(Query::query(kb, "Aspirin"))