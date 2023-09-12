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

    
}