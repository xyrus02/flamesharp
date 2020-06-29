@echo off

REM check for dotnet
where dotnet >nul 2>nul
if %errorlevel% neq 0 goto no_dotnet

REM build project
dotnet build -c Release src\flamesharp.sln

REM done!
goto exit

:no_dotnet
echo ERROR: dotnet was not found. Please install the latest .NET Core SDK from https://dot.net 1>&2
goto exit

:exit
if "%1"=="--batch" goto eof
echo Press any key to continue . . .
pause > nul

:eof
