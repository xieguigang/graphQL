require(graphQL);

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
const entities as string = readLines(entities_list);

print("loading graph database for run query...");

const kb = MsgFile::open(evidenceAggregate = TRUE);
