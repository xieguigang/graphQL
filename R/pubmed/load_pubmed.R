imports "mysql" from "graphR";
imports "graph_mysql" from "graphR";

#' Open the mysql database connection to the pubmed database
#' 
const open_pubmed = function(user, passwd, 
                             dbname = "pubmed", 
                             host = "localhost", 
                             port = 3306, 
                             workdir = "./") {

    mysql::open(user, passwd, 
        dbname = dbname,
        host = host,
        port = port,
        error_log = file.path(workdir, `pubmed_${md5(toString(now()))}.log`)
    );
}

#' load the pubmed article set stream file and push into database
#' 
#' @param pubmed the mysql connection to the pubmed database, which 
#'    could be generates via the ``open_pubmed`` function.
#' @param file the local file path to the pubmed article set database 
#'    file.
#' 
const load_pubmed = function(pubmed, file) {
    let articles = parse.article_set(file);
    let article_tbl = pubmed |> table("articles");
    let abstract_tbl = pubmed |> table("fulltext");
    let mesh_tbl = pubmed |> table("mesh");
    let graph = pubmed |> table("metadata");

    for(let article in tqdm(articles)) {
        article <- summary(article);

        if (!(article_tbl |> check(id = article$PMID))) {
            article_tbl |> add(
                id = article$PMID,
                authors = paste(article$authors, sep = ", "),
                title = article$title,
                journal = article$journal,
                doi = article$doi,
                year = article$year 
            );  
        }

        if (!(abstract_tbl |> check(id = article$PMID))) {
            abstract_tbl |> add(
                id = article$PMID,
                abstract = article$abstract
            );
        }

        let mesh_terms = article$mesh;

        for(let term_id in names(mesh_terms)) {
            let term = mesh_terms[[term_id]];

            if (!(mesh_tbl |> check(mesh_id = term_id))) {
                mesh_tbl |> add(
                    mesh_id = term_id,
                    term = term
                );
            }

            term_id = mesh_tbl 
            |> where(mesh_id = term_id) 
            |> find()
            ;

            if (is.null(term_id)) {
                print(`save mesh term error: '${term}'.`);
            } else {
                if (!(graph |> check(pubmed_id = article$PMID, 
                        mesh_id = term_id$id))) {

                    graph |> add(
                        pubmed_id = article$PMID, 
                        mesh_id = term_id$id
                    );
                }
            }
        }
    }
}