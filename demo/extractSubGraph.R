require(igraph);

const raw = igraph::read.network(
	`${dirname(@script)}/data/`
);

for(key in ["return"]) {
	raw
	|> subgraphFromPoint(fromPoint = key)
	|> save.network(
		file       = `${dirname(@script)}/subgraph/${key}/`,
		properties = "PageRank"
	)
	;
}