@echo off
setlocal EnableDelayedExpansion

set exec_time=5400
set client_num=8

set /a player_lifetime=%exec_time%+60
set /a get_cpu_lifetime=%player_lifetime%+%client_num%*10
set /a loop_count=%client_num%-1

set my_date=%date:/=%
set my_date=%my_date:~-8%
set my_date=%my_date:~-4%%my_date:~,4%

mkdir ..\Data\%my_date% > NUL 2>&1
for /f "usebackq delims=" %%A in (`dir /AD /B ..\Data\%my_date% ^| find /c /v ""`) do set dir_count=%%A

set work_dir=..\Data\%my_date%\%dir_count%
set cpu_file_name=%work_dir%\%INSTANCE_NAME%_CpuUtilization.csv
set pre_memory_file_name=%work_dir%\%INSTANCE_NAME%_MemoryUtilization
set pre_log_file_name=%work_dir%\Logs\%INSTANCE_NAME%_Log
mkdir %work_dir%\Logs

start GetCpuUtilization.bat %get_cpu_lifetime% %cpu_file_name%
for /l %%i in (0, 1, %loop_count%) do (
    set counter=%%i
    start C:\Windows\System32\cmd.exe /c ^
        ".\Holiday --memory-utilization-dump-file %pre_memory_file_name%!counter!.txt --send-message-period 5 > %pre_log_file_name%!counter!.log 2>&1"
    powershell sleep 10
)

timeout %player_lifetime%

taskkill /im Holiday.exe

timeout 5

aws s3 cp --recursive %work_dir%\ s3://extreal-webgl/PerformanceTest/Data/%my_date%/%dir_count%/

endlocal
