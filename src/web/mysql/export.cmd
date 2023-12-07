@echo off

SET reflector="\graphQL\src\mysqli\App\Reflector.exe"
SET sql_file="E:\graphQL\src\web\mysql\graphdb.sql"

%reflector% --reflects /sql %sql_file% /namespace "mysql" /split