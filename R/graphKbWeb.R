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

const token_vector = function(token, top = 10, graphdb = getOption("graphdb_web")) {
    const url = `${graphdb}/mining/get_vector/?word=${urlencode(token)}&top=${top}`;
    const pull = url 
    |> http::requests.get() 
    |> http::content()
    ;

    if (as.integer(pull$code) == 0) {
        const li = pull$info;
        const tleft = li$left;
        const tright = li$right;
        const left = data.frame(
            "token" = tleft@token,
            "w" = tleft@weight
        );
        const right = data.frame(
            "token" = tright@token,
            "w" = tright@weight
        );

        list(left, right);
    } else {
        stop(pull);
    }
}

const context_cosine = function(a, b, top = 10, graphdb = getOption("graphdb_web")) {
    const va = token_vector(a, top, graphdb);
    const vb = token_vector(b, top, graphdb);
    const cos_a = __cosine(va$left, vb$left);
    const cos_b = __cosine(va$right, vb$right);
    const jad_a = __jaccard(va$left, vb$left);
    const jad_b = __jaccard(va$right, vb$right);

    [
        cos_a, cos_b, 
        jad_a, jad_b, 
        (cos_a + cos_b) / 2, 
        (jad_a + jad_b) / 2
    ];
}

const __jaccard = function(va, vb) {
    const U = length(unique(append(va$token, vb$token)));
    const I = length(intersect(va$token, vb$token));

    I / U;
}

const __cosine = function(va, vb) {
    const token_union = unique(append(va$token, vb$token));
    const la = as.list(va, byrow = TRUE);
    const lb = as.list(vb, byrow = TRUE);

    names(la) = va$token;
    names(lb) = vb$token;

    va = sapply(token_union, ti -> ifelse(ti in la, la[[ti]]$w, 0.0));
    vb = sapply(token_union, ti -> ifelse(ti in lb, lb[[ti]]$w, 0.0));

    math::cosine(va, vb);
}