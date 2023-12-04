@echo off

SET reflector="D:\graphQL\src\mysqli\App\Reflector.exe"
SET current_schema="./text_mining_current.sql"
SET updates_schema="./text_mining.sql"
SET updates_report="./text_mining_current_upgrades_to_text_mining.schema_compares.report.md"

%reflector% --compares /current %current_schema% /updates %updates_schema% /output %updates_report%

SET current_schema="./graphdb.sql"
SET updates_schema="./graphdb_upgrade.sql"
SET updates_report="./graphdb_upgrades.report.md"

%reflector% --compares /current %current_schema% /updates %updates_schema% /output %updates_report%