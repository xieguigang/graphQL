#' Open the mysql database connection to the pubmed database
#' 
const open_pubmed = function(user, passwd, 
                             dbname = "pubmed", 
                             host = "localhost", 
                             port = 3306) {

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

    for(let article in tqdm(articles)) {

    }
}