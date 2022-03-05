import graphQL

setwd(@dir)

kegg = read.csv("./kegg.csv", row.names = None)
hmdb = read.csv("./hmdb.csv", row.names = None)
chebi = read.csv("./chebi.csv", row.names = None)

# print(kegg, max.print = 6)
# print(hmdb, max.print = 6)
# print(chebi, max.print = 6)

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

result = Query::query(kb, "Aspirin")

print(sapply(result, x -> toString(x)))

MsgFile::save(kb, file = "./Aspirin.graph")

print("unweighted similarity between C01405 and HMDB0001879:")
print(Query::similarity(kb, "C01405", "HMDB0001879"))

print("weighted similarity between C01405 and HMDB0001879:")
print(Query::similarity(kb, "C01405", "HMDB0001879", name = 0.1, formula = 0.45, xref = 0.45))