<?php

imports("MVC.controller");
imports("MVC.restrictions");

/** 
 * BioDeep公用的用户访问权限控制器
*/
class accessController extends controller {

    public function accessControl() {
        return true;  
    }
}