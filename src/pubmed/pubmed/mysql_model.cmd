REM @echo off

SET Reflector="../../mysqli/App/Reflector.exe"
SET sql_src="../../../mysql/pubmed.sql"

%Reflector% --reflects /sql %sql_src% -o ./ /namespace pubmed --language visualbasic /split

pause