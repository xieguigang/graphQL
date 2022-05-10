require(graphQL);
require(JSON);

#' title: Run graph database query
#' author: xieguigang <xie.guigang@gcmodeller.org>
#' description: Run graph database query for query 
#'    entity network data based on a given list of 
#'    the entity terms.

[@info "An id list text file that used for run data query.
   the text file format of this argument should be a list
   file that contains multiple lines data, and each line in
   this file data is a entity reference term string to run 
   database query."]
[@type "filepath"]
const entities_list as string = ?"--entityList" || stop("No entity list id was provided!");
[@info "A graph database file. the graph database mode that 
   created via this package file should be checked of the
   option: ``evidenceAggregate = TRUE``!"]
[@type "filepath"]
const graphDb as string = ?"--graphDb" || stop("no database connection is provided!");
[@info "the json result output file path"]
const savefile as string = ?"--save" || `${dirname(entities_list)}/${basename(entities_list)}.query.json`;
const entities as string = readLines(entities_list);

print("run query for a given entity list:");
print(entities);
print("loading graph database for run query...");
print(graphDb);

const kb = MsgFile::open(graphDb, evidenceAggregate = TRUE, noGraph = TRUE);
const queryResult = entities 
|> unique 
|> lapply(term -> kb |> query(term), names = entities)
;

queryResult
|> json_encode()
|> writeLines(con = savefile)
;