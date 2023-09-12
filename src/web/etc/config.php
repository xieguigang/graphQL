<?php

if (!defined("AppViews")) {
    define("AppViews", dirname(__DIR__) . "/views");
}

define("APP_ROOT", dirname(__DIR__));

/**
 * 配置文件
 */
return [
    // 默认的数据库连接参数配置
	//
	// 使用默认的数据库配置：  $query = (new Table("tableName"))->where([....])->select();
	// 
    'DB_HOST' => "192.168.0.231",
    'DB_NAME' => 'graphql',
    'DB_USER' => 'root',
    'DB_PWD'  => '123456.Xieguigang',
    'DB_PORT' => '3306',

    // 框架配置参数
	"ERR_HANDLER_DISABLE" => "FALSE",
	"RFC7231"       => AppViews . "/http_errors",
    "CACHE" => false,
    "CACHE.MINIFY" => true,
    "APP_NAME" => "meta_discovery",
    "APP_TITLE" => "苏州帕诺米克生物医药科技有限公司 BioDeep™",
    "APP_VERSION" => "2.856.27.27989659-release",
	"MVC_VIEW_ROOT" => [
		"analysis"  => AppViews . "/analysis",
        "index" => AppViews,
        "browse" => AppViews . "/views",
        "view" => AppViews . "/views",
        "repository" => AppViews . "/repository",
        "workflow" => AppViews . "/workflow",
        "upload" => AppViews . "/upload"
    ]
];