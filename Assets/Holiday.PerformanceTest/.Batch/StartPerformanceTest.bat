@echo off
setlocal EnableDelayedExpansion

set my_time=%time: =0%
set my_time=%my_time:~,8%
set my_time=%my_time::=%
set my_date=%date:/=%
set my_date=%my_date:~-8%
set my_date=%my_date:~-4%%my_date:~,4%
set dir_name=%my_date%_%my_time%
mkdir ..\Data\%dir_name%\Logs

start GetCpuUtilization.bat ..\Data\%dir_name% %dir_name%
for /l %%i in (0, 1, 17) do (
    set counter=%%i
    start C:\Windows\System32\cmd.exe /c ^
        ".\Holiday.exe ..\Data\%dir_name%\%dir_name%_%INSTANCE_NAME%_MemoryUtilization!counter!.txt > ..\Data\%dir_name%\Logs\%dir_name%_%INSTANCE_NAME%_log!counter!.txt 2>&1"
    powershell sleep 10
)

timeout 5460

taskkill /im Holiday.exe

timeout 5

aws s3 cp --recursive ..\Data\%dir_name%\ s3://extreal-dev/PerformanceTest/Data/%my_date%/

endlocal
