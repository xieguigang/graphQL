import graphQL

setwd(@dir)

kegg = read.csv("./kegg.csv", row.names = None)
hmdb = read.csv("./hmdb.csv", row.names = None)
chebi = read.csv("./chebi.csv", row.names = None)

print(kegg)
print(hmdb)
print(chebi)
