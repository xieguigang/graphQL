imports ["Query", "MsgFile", "KnowledgeGraph"] from "graphR";
imports "pubmed" from "kb";

require(GCModeller);

const .onLoad = function() {
	cat("\n");
	cat("NoSQL graph database engine writen in VisualBasic\n");
	cat("for R# language\n");
	cat("\n");
	cat("Github: https://github.com/xieguigang/graphQL\n");
	cat("\n");

	options(graphdb_web = "http://novocell.mzkit.org:88");
}