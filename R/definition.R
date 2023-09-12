const def_ignores = ["the","a","and","it"];

const definition = function(word, top = 30, graphdb = getOption("graphdb_web")) {
    const is = get_prompt("is", top = top, graphdb);
    const context_cos = is 
    |> which(x -> !((x$token) in def_ignores)) 
    |> lapply(function(t) {
        t$cos = prod(context_cosine(
            a = word, 
            b = t$token, 
            top = top, 
            graphdb = graphdb
        ));
        t;
    })
    |> which(t -> (t$cos) > 0)
    ;

    if (length(context_cos) == 0) {
        NULL;
    } else {
        let def = data.frame(
            def = context_cos@token,
            prob = as.numeric(context_cos@cos),
            w = as.numeric(context_cos@weight)
        );

        def[, "score"] = def$w * def$prob;
        def = def[order(def$score, decreasing = TRUE), ];
        def;
    }
}

const get_prompt = function(word, top = 10, graphdb = getOption("graphdb_web")) {
    const url = `${graphdb}/mining/get_prompt/?q=${urlencode(word)}&top=${top}`;
    const data = url |> http::requests.get() 
                     |> http::content()
                     ;

    if (as.integer(data$code) == 0) {
        data$info;
    } else {
        stop(data);
    }
}