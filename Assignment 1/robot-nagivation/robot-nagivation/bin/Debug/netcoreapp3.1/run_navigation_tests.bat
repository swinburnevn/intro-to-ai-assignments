@echo off
setlocal EnableDelayedExpansion

set _agents=bfs,dfs,gbfs,astar,ucs,iddfs
set _maps=RobotNav-test.txt,robot-nav-map2.txt,robot-nav-map3.txt,robot-nav-map4.txt
set _logfile=testlog.txt
set _APPNAME=robot-nagivation.exe

set count=0

echo. 
echo. [*] Running test suite on %_APPNAME%
echo. [ ]  Agents : %_agents%
echo. [ ]  Maps   : %_maps%
echo. [ ]  Log    : %_logfile%

echo. Output log: %_APPNAME% > %_logfile%
echo.
echo. --------------------------------------------------
echo. 

for %%G in (%_agents%) do (
    for %%H in (%_maps%) do (
		set/a count+=1

		echo. [!count!	] Running: '%_APPNAME% %%H %%G ss 0 true'
		%_APPNAME% %%H %%G ss 0 true >> %_logfile%
		set/a count+=1
		echo. [!count!	] Running: '%_APPNAME% %%H %%G ss 0'
		%_APPNAME% %%H %%G ss 0 >> %_logfile%
		
		
    )
)
echo.
echo. --------------------------------------------------
echo. [!] Test suite finished
pause >nul