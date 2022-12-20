@echo off
setlocal EnableDelayedExpansion

set my_time_long=%time: =0%
set my_time=%my_time_long:~,8%
set my_date=%date: =%
set dir_name=Data_%my_date:/=%%my_time::=%
mkdir %dir_name%

start /d %dir_name% GetCpuUtilization.bat
for /l %%i in (0, 1, 9) do (
    set counter=%%i
    start Holiday.exe %dir_name%/MemoryUtilization!counter!.txt
    powershell sleep 10
)

endlocal

timeout 5400

taskkill /im Holiday.exe
taskkill /im cmd.exe
