require(graphQL);
require(igraph);

setwd(@dir);

g = "./joinTest\aspirin.graph"
|> MsgFile::read.graph()
;

save.network(g, file = "./network/", properties = ["knowledge_type", "source"]);