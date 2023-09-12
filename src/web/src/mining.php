<?php

define("SITE_PATH", __DIR__);

include __DIR__ . "/../etc/bootstrap.php";

/**
 * text mining helper
*/
class App {

    /**
     * @access *
    */
    public function index() {
        View::Display();
    }

    /**
     * push new graph link or update the existed graph link its weight value
     * 
     * @uses api
     * @method POST
    */
    public function push_links($links, $tokens) {
        $links  = json_decode($links, true);
        $tokens = json_decode($tokens, true);
        $node   = new Table(["text_mining" => "word_token"]);
        $graph  = new Table(["text_mining" => "text_graph"]);

        foreach($tokens as $token) {
            $n = $token["size"];

            if (Utils::isDbNull($node->where(["id" => $token["index"]])->find())) {
                $node->add([
                    "id" => $token["index"],
                    "token" => $token["token"],
                    "count" => $n
                ]);
            } else {
                $node->where(["id" => $token["index"]])
                ->limit(1)
                ->save([
                    "count" => "~count + $n"
                ])
                ;
            }
        }

        foreach($links as $link) {
            $q = $graph->where([
                "from" => $link["from_i"],
                "to" => $link["to_i"]
            ])->find();
            $w = $link["w"];

            if (Utils::isDbNull($q)) {
                $graph->add([
                    "from" => $link["from_i"],
                    "to" => $link["to_i"],
                    "weight" => $w
                ]);
            } else {
                $graph->where([
                    "id" => $q["id"]
                ])
                ->limit(1)
                ->save([
                    "weight" => "~weight + $w"
                ]);
            }
        }

        controller::success(1);
    }
}