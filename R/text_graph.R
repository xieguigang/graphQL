# web api handler for create graph kb
const text_graph = function(text, phase_size = 6) { 
    let tokens = text 
    |> trim(characters = " ,.?;':") 
    |> strsplit("\s+")
    ;
    
    if (is.list(tokens)) {
        tokens = tokens |> lapply(t -> tokens_trim(t));
    } else {
        tokens = list(single = tokens_trim(tokens)); 
    }    

    lapply(tokens, t -> one_graph(t, phase_size = phase_size));
}

const tokens_trim = function(tokens) {
    tokens = sapply(tokens, si -> si |> trim(characters = " ,.?;'():"));
    tokens = tokens[nchar(tokens) > 0];

    tolower(tokens);
}

#' The text data will be indexed via the FNV-1a hash algorithm
#' 
#' @return a dataframe object that contains the graph link data
#' 
const one_graph = function(tokens, phase_size = 3) {
    const g = list();    

    for(i in phase_size:[length(tokens) - 1]) {
        g[[length(g) + 1]] = graph_link(tokens[i], tokens[i+1], w = 1);

        # from
        for (j in [i-phase_size]:[i-1]) {
            g[[length(g) + 1]] = graph_link(tokens[j], tokens[j+1], w = 1 / [i - j]);
        }
        # to
        for(j in [i+1]:[i + phase_size]) {
            g[[length(g) + 1]] = graph_link(tokens[j], tokens[j+1], w = 1 / [j - i]);
        }
    }

    const graph_df = data.frame(
        from   = g@from,
        to     = g@to,
        from_i = g@from_i,
        to_i   = g@to_i,
        w      = g@w,
        index  = `${g@from_i} | ${g@to_i}`
    );
    const meltdown = graph_df |> groupBy("index");
    const graph_out = data.frame(
        from   = sapply(meltdown, d -> .Internal::first(d$from)),
        to     = sapply(meltdown, d -> .Internal::first(d$to)),
        from_i = sapply(meltdown, d -> .Internal::first(d$from_i)),
        to_i   = sapply(meltdown, d -> .Internal::first(d$to_i)),
        w      = sapply(meltdown, d -> sum(d$w)),
        size   = sapply(meltdown, d -> nrow(d))
    );

    graph_out;
}

const graph_link = function(from, to, w = 1) {
    if (any([is.null(from), is.null(to)])) {
        NULL;
    } else {
        list(
            from   = from, to = to,
            from_i = FNV1a_hashcode(from),
            to_i   = FNV1a_hashcode(to),
            w      = w
        );
    }
}