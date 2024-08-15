require(GCModeller);

imports "pubmed" from "kb";

let articles = parse.article_set("F:\\pubmed\\pubmed24n0002.xml.gz");

str(as.data.frame(articles));