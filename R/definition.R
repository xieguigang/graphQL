const definition = function(word, graphdb = getOption("graphdb_web")) {
    const is = get_prompt("is", graphdb);

    print(is);
}

const get_prompt = function(word, graphdb = getOption("graphdb_web")) {
    const url = `${graphdb}/mining/get_prompt/?q=${urlencode(word)}`;
    const data = url |> http::requests.get() 
                     |> http::content()
                     ;

    if (as.integer(data$info) == 0) {
        data$info;
    } else {
        stop(data);
    }
}