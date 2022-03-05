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
    
    # str(data)
    
    Query::insert(kb, data[["id"]], data)


kb = MsgFile::open()

process_knowledge(kb, kegg)
process_knowledge(kb, hmdb)
process_knowledge(kb, chebi)
process_knowledge(kb, read.csv("./water.csv", row.names = None))
process_knowledge(kb, read.csv("./water_chebi.csv", row.names = None))

result = Query::query(kb, "Aspirin")

print(sapply(result, x -> toString(x)))

cat("\n\n")

MsgFile::save(kb, file = "./Aspirin.graph")

print("unweighted similarity between C01405 and HMDB0001879:")
print(Query::similarity(kb, "C01405", "HMDB0001879"))

print("unweighted similarity between Aspirin and ChEBI: 15365:")
print(Query::similarity(kb, "Aspirin", "ChEBI:15365"))

print("unweighted similarity between Aspirin and Acetylsalicylic acid:")
print(Query::similarity(kb, "Aspirin", "Acetylsalicylic acid"))

print("unweighted similarity between Aspirin and Water:")
print(Query::similarity(kb, "Aspirin", "Water"))

print("unweighted similarity between dihydrogen oxide and Water:")
print(Query::similarity(kb, "dihydrogen oxide", "water"))


