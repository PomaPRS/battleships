echo off

set curPath=%~dp0
set startegiesPath=%curPath%strategies\
set testerFileName=Battleships.AiTester.exe
set testerPath=%curPath%tester\%testerFileName%

set args=
For /R "%startegiesPath%" %%I In (*.exe) Do (
	call set "args=%%args%% "%%I""
)

call "%testerPath%"%args%
pause