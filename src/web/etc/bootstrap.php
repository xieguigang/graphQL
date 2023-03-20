<?php

define("YEAR", date("Y"));
define("APP_DEBUG", true);
define("FRAMEWORK_DEBUG", true);

if (array_key_exists("REDIRECT_URL", $_SERVER)) {
    define("URL", $_SERVER["REDIRECT_URL"]);
}

# 公用模块的引用将会通过这个文件来统一完成初始化

# WebFramework
include __DIR__ . "/../framework/php.NET/package.php";
include __DIR__ . "/accessController.php";

// 框架加载配置文件，并执行所请求的控制器函数

dotnet::AutoLoad(__DIR__ . "/config.php");
dotnet::HandleRequest(new App(), new accessController());

// 脚本执行结束