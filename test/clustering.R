require(igraph);
require(graphQL);

setwd(@dir);

data = read.csv("facebook_combined.txt", check.modes = FALSE);
colnames(data) = ["from", "to"];

print(data, max.print = 13);

g = igraph::graph(from = data[, "from"], to = data[, "to"]);

print(g);

g = graphUMAP(g);