require(GCModeller);
require(graphQL);

imports "pubmed" from "kb";

let articles = as.data.frame(pubmed::parse(readText(`${@dir}/pubmed-Aspirin-set.txt`)));

print(articles$abstract);

for(str in articles$abstract) {
    let g = text_graph(str);

    for(par in g) {
        print(par);
        graphQL::push_graph(par);
    }

    # stop();
}