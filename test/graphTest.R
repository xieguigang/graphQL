require(graphQL);
require(igraph);

setwd(@dir);

g = "./joinTest\aspirin.graph"
|> MsgFile::read.graph()
;

save.network(g, file = "./network/raw/", properties = ["knowledge_type", "source"]);
i=0;

for(part in g |> knowledgeIslands) {
	save.network(g, file = `./network/graph_${i=i+1}/`, properties = ["knowledge_type", "source"]);
}