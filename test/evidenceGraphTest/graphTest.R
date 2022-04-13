require(graphQL);
require(igraph);

setwd(@dir);

g = "./aspirin.graph"
|> MsgFile::read.graph()
;
kb = MsgFile::open("./aspirin.graph");

rendering = function(graph, savefile) {
	require(igraph);
	require(ggplot);
	require(JSON);
	
	let g = graph
	|> louvain_cluster()
	|> compute.network()	
	;

	g 
	|> extractKnowledgeTerms()
	|> niceTerms(kb, indexBy = ["cas", "kegg", "formula", "inchikey", "hmdb"])
	|> write.csv(file = `${dirname(savefile)}/${basename(savefile)}_terms.csv`)
	;

	bitmap(file = savefile, size = [1920, 1440], dpi = 300) {
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