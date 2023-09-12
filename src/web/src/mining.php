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
    public function push_links($links) {
        $links = json_decode($links);

        foreach($links as $link) {
            
        }
    }
}