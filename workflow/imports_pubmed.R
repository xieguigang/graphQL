require(GCModeller);
require(graphQL);

imports "pubmed" from "kb";

let articles = as.data.frame(pubmed::parse(readText(`${@dir}/pubmed-Aspirin-set.txt`)));

print(articles);

