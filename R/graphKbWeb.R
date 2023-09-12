imports "http" from "webKit";

#' Push the text graph data into the database
const push_graph = function(graph_df, graphdb = getOption("graphdb_web")) {
    const url = `${graphdb}/mining/push_links/`;
    const payload = {
        links: as.list(graph_df, byrow = TRUE)
    };
    const result = http::requests.post(url, payload) 
                |> http::content();

    str(result);
}