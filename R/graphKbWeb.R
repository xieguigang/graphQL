imports "http" from "webKit";

#' Push the text graph data into the database
const push_graph = function(graph_df, graphdb = getOption("graphdb_web")) {
    const url = `${graphdb}/mining/push_links/`;
    const tokens = graph_tokens(graph_df);
    const payload = {
        links: as.list(graph_df, byrow = TRUE),
        tokens: as.list(tokens, byrow = TRUE)
    };
    const result = http::requests.post(url, payload) 
                |> http::content();

    str(result);
    # stop();
}

const graph_tokens = function(graph_df) {
    const tokens = append(graph_df$from, graph_df$to);
    const index = append(graph_df$from_i, graph_df$to_i);
    const tokens_set = groupBy(data.frame(token = tokens, index), "token");

    print("get token set:");
    print(names(tokens_set));

    data.frame(
        token = names(tokens_set),
        index = sapply(tokens_set, t -> .Internal::first(t$index)),
        size = sapply(tokens_set, t -> nrow(t))
    );
}

const token_vector = function(token, graphdb = getOption("graphdb_web")) {
    const url = `${graphdb}/mining/get_vector/?word=${urlencode(token)}`;
    const pull = url 
    |> http::requests.get() 
    |> http::content()
    ;

    if (as.integer(pull$code) == 0) {
        const li = pull$info;
        const vector = data.frame(
            "token" = 
        );

        print(vector);

        stop();
    } else {
        stop(pull);
    }
}

const context_cosine = function(a, b, graphdb = getOption("graphdb_web")) {
    const va = token_vector(a, graphdb);
    const vb = token_vector(b, graphdb);

    stop();
}