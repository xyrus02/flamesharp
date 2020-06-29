@echo off

REM build
call build.cmd --batch

REM determine build output
set flamesharpexecutable=src\main\Net.Ktrix.Flamesharp.Cli\bin\Release\net472\flamesharp.exe

REM check executable
if not exist %flamesharpexecutable% goto no_build

REM run
cls
%flamesharpexecutable% --resolution 512x384 --trace --verbosity debug src\test\sierpinskiquilt.json 

REM done
goto exit

:no_build
echo ERROR: Build output not found. Please build the project first or look for errors above. 1>&2
goto exit

:exit
if "%1"=="--batch" goto eof
echo Press any key to continue . . .
pause > nul

:eof
