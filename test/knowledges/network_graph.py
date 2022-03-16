import graphQL
import igraph

setwd(@dir)

kb = MsgFile::open(`${@dir}/aspirin.graph`)
g = networkGraph(kb)
result = knowledgeCommunity(g, common.type = ["formula","chembl","chebi","knapsack"], eps = 1e-8)

g = result$graph

write.csv(result$knowledges, file = "./knowledges.xls")
save.network(g, file = "./aspirin_graph/", properties = "color")