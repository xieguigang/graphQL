@echo off

SET drive=%~d0
SET R_HOME=%drive%\GCModeller\src\R-sharp\App
SET Rscript="%R_HOME%/Rscript.exe"
SET REnv="%R_HOME%/R#.exe"

%Rscript% --build /src ../ /save ../graphQL.zip --skip-src-build
%REnv% --install.packages ../graphQL.zip

pause