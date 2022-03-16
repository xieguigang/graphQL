import graphQL
import igraph

setwd(@dir)

kb = MsgFile::open(`${@dir}/aspirin.graph`)
g = networkGraph(kb)
g = Kosaraju.SCCs(g)

save.network(g, file = "./aspirin_graph/")