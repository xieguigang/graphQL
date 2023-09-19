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
     * context analysis
     * 
     * @uses api
     * @method GET
    */
    public function get_vector($word, $top = 10) {
        $i = $this->find_token($word);

        if (Utils::isDbNull($i)) {
            controller::error("Unable to find word token '$word'!", 404);
        } else {
            $left = $this->graph
                ->left_join("word_token")
                ->on(["word_token" => "id", "text_graph" => "from"])
                ->where(["to" => $i["id"]])
                ->order_by("`weight` / text_graph.`count`", true)
                ->limit($top)
                ->select(["token","`text_graph`.`weight` / text_graph.`count` as weight"])
                ;
            $right = $this->graph
                ->left_join("word_token")
                ->on(["word_token" => "id", "text_graph" => "to"])
                ->where(["from" => $i["id"]])
                ->order_by("`weight` / text_graph.`count`", true)
                ->limit($top)
                ->select(["token","`text_graph`.`weight` / text_graph.`count` as weight"])
                ;

            controller::success(["left" => $left, "right" => $right]);
        }
    }

    /**
     * get a word token that next to word q
     * 
     * @uses api
     * @method GET
    */
    public function get_prompt($q, $top = 10) {
        $i = $this->find_token($q);

        if (Utils::isDbNull($i)) {
            controller::error("Unable to find word token '$q'!", 404);
        } else {           
            $right = $this->graph
            ->left_join("word_token")
            ->on(["word_token" => "id", "text_graph" => "to"])
            ->where(["from" => $i["id"]])
            ->order_by("`weight` / text_graph.`count`", true)
            ->limit($top)
            ->select(["token","`text_graph`.`weight` / text_graph.`count` as weight"])
            ;

            controller::success($right, $this->graph->getLastMySql());
        }
    }

    /**
     * get weight of the specific graph link
     * 
     * @uses api
     * @method GET
    */
    public function get_weight($i, $j) {
        $hash1 = $this->find_token($i);
        $hash2 = $this->find_token($j);

        if (Utils::isDbNull($hash1)) {
            controller::error("Unable to find word token '$i'!", 404);
        } else if (Utils::isDbNull($hash2)) {
            controller::error("Unable to find word token '$j'!", 404);
        } else {
            $ij = $this->graph->where([
                "from" => $hash1["id"],
                "to" => $hash2["id"]
            ])->find();
            $ji = $this->graph->where([
                "to" => $hash1["id"],
                "from" => $hash2["id"]
            ])->find();

            if (Utils::isDbNull($ij)) {
                $ij = 0;
            } else {
                $ij = $ij["weight"];
            }
            if (Utils::isDbNull($ji)) {
                $ji = 0;
            } else {
                $ji = $ji["weight"];
            }

            controller::success([
                "weight" => [$ij, $ji],
                "w1" => $hash1["count"],
                "w2" => $hash2["count"],
                "i" => $i,
                "j" => $j
            ]);
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
                    "weight" => $w,
                    "count" => 1
                ]);
            } else {
                $this->graph->where([
                    "id" => $q["id"]
                ])
                ->limit(1)
                ->save([
                    "weight" => "~weight + $w",
                    "count" => "~count + 1"
                ]);
            }
        }

        controller::success(1);
    }

    private function find_token($token) {
        return $this->node
            ->where(["token" => strtolower(urldecode($token))])
            ->find();
    }

    public function assign_class($token, $class) {
        # find token
        $hash1 = $this->find_token($token);

        if (Utils::isDbNull($hash1)) {
            controller::error("the given token '$token' could not be found in database!");
        }

        $class_label = strtolower(urldecode($class));
        $class_data = (new Table(["text_mining" => "token_class"]))
            ->where(["class" => $class_label])
            ->find();

        if (Utils::isDbNull($class_data)) {
            # add new class tag
            (new Table(["text_mining" => "token_class"]))->add(["class" => $class_label]);

            $class_data = (new Table(["text_mining" => "token_class"]))
            ->where(["class" => $class_label])
            ->find();
        }

        $this->node->where([
            "id" => $hash1["id"]
        ])->save([
            "class" => $class_data["id"]
        ]);

        controller::success(1);
    }
}