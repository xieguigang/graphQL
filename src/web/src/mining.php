<?php

define("SITE_PATH", __DIR__);

include __DIR__ . "/../etc/bootstrap.php";

/**
 * text mining helper
*/
class App {

    /**
     * @var Table
    */
    private $node;   
    /**
     * @var Table
    */
    private $graph;  

    function __construct() { 
        $this->node  = new Table(["text_mining" => "word_token"]);
        $this->graph = new Table(["text_mining" => "text_graph"]);
    }

    /**
     * @access *
    */
    public function index() {
        View::Display();
    }

    /**
     * get word weight vector
     * 
     * @uses api
     * @method GET
    */
    public function get_vector($word, $top = 10) {
        $i = $this->node->where(["token" => strtolower(urldecode($word))])->find();

        if (Utils::isDbNull($i)) {
            controller::error("Unable to find word token '$word'!", 404);
        } else {
            $left = $this->graph->left_join("word_token")->on(["word_token" => "id", "text_graph" => "from"])->where(["to" => $i["id"]])->order_by("weight", true)->limit($top)->select();
            $right = $this->graph->left_join("word_token")->on(["word_token" => "id", "text_graph" => "to"])->where(["from" => $i["id"]])->order_by("weight", true)->limit($top)->select();

            controller::success(["left" => $left, "right" => $right]);
        }
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

        foreach($tokens as $token) {
            $n = $token["size"];

            if (Utils::isDbNull($this->node->where(["id" => $token["index"]])->find())) {
                $this->node->add([
                    "id" => $token["index"],
                    "token" => $token["token"],
                    "count" => $n
                ]);
            } else {
                $this->node->where(["id" => $token["index"]])
                ->limit(1)
                ->save([
                    "count" => "~count + $n"
                ])
                ;
            }
        }

        foreach($links as $link) {
            $q = $this->graph->where([
                "from" => $link["from_i"],
                "to" => $link["to_i"]
            ])->find();
            $w = $link["w"];

            if (Utils::isDbNull($q)) {
                $this->graph->add([
                    "from" => $link["from_i"],
                    "to" => $link["to_i"],
                    "weight" => $w
                ]);
            } else {
                $this->graph->where([
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