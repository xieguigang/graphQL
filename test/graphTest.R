require(graphQL);
require(igraph);

setwd(@dir);

g = "./joinTest\aspirin.graph"
|> MsgFile::read.graph()
;


rendering = function(graph, savefile) {
	require(igraph);
	require(ggplot);
	require(JSON);
	
	let g = graph
	|> compute.network()
	;

	bitmap(file = savefile, size = [1920, 1200], dpi = 300) {
		ggplot(g) 
		+ geom_edge_link() 
		+ geom_node_point(aes(
			size = ggraph::map("degree", [9, 50]), 
			fill = ggraph::map("group", "paper"))
		) 
		+ geom_node_text(aes(size = ggraph::map("degree", [4, 9]))) 
		+ layout_springforce(iterations = 100)
		;
	}
}


save.network(g, file = "./network/raw/", properties = ["knowledge_type", "source"]);
rendering(g, "./network/raw.png");
i=0;

for(part in g |> knowledgeIslands) {
	save.network(part, file = `./network/graph_${i=i+1}/`, properties = ["knowledge_type", "source"]);
	rendering(part, `./network/graph_${i}.png`);
}