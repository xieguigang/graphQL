import graphQL
import igraph

setwd(@dir)

kb = MsgFile::open(`${@dir}/aspirin.graph`)
# g = networkGraph(kb)
result = knowledgeCommunity(kb, indexBy = ["kegg","chembl","chebi","cas","formula","hmdb","pubchem"], eps = 1e-8)

g = result$graph

write.csv(result$raw, file = "./knowledges_raw.xls")
write.csv(result$knowledges, file = "./knowledges.xls")
save.network(g, file = "./aspirin_graph/", properties = "color")