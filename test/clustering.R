require(igraph);

setwd(@dir);

data = read.csv("facebook_combined.txt", tsv = TRUE, check.modes = FALSE);

print(data, max.print = 13);