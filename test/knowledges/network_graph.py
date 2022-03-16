import graphQL
import igraph

setwd(@dir)

kb = MsgFile::open(`${@dir}/aspirin.graph`)
g = networkGraph(kb)

save.network(g, file = "./aspirin_graph/")