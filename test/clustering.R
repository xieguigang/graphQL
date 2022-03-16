require(igraph);
require(graphQL);

setwd(@dir);

data = read.csv("facebook_combined.txt", check.modes = FALSE);
colnames(data) = ["from", "to"];

print(data, max.print = 13);

g = igraph::graph(from = data[, "from"], to = data[, "to"]);

print(g);

c = graphUMAP(g);

print(c, max.print = 13);

write.csv(c, file = "./cluster.csv", row.names = TRUE);

g = g |> louvain_cluster();


v = V(g);

type = igraph::class(v);
id = igraph::xref(v);

save.network(g, file = "./graph/");

write.csv(data.frame(id, type), file = "./node_types.csv", row.names = FALSE);