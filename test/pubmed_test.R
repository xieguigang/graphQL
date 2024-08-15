require(GCModeller);

imports "pubmed" from "kb";

let articles = parse.article_set("F:\\pubmed\\pubmed24n0002.xml.gz");

# str(as.data.frame(articles));

for(let article in articles[1:100]) {
str(summary(article));
}

