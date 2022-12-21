@echo off
setlocal EnableDelayedExpansion

set my_time=%time: =0%
set my_time=%my_time:~,8%
set my_time=%my_time::=%
set my_date=%date:/=%
set my_date=%my_date:~-8%
set my_date=%my_date:~-4%%my_date:~,4%
set dir_name=%my_date%_%my_time%
mkdir ..\Data\%dir_name%

start GetCpuUtilization.bat ..\Data\%dir_name% %dir_name%
for /l %%i in (0, 1, 9) do (
    set counter=%%i
    start Holiday.exe ..\Data\%dir_name%\%dir_name%_%INSTANCE_NAME%_MemoryUtilization!counter!.txt
    powershell sleep 10
)

timeout 5400

taskkill /im Holiday.exe

timeout 30

aws s3 cp --recursive ..\Data\%dir_name%\ s3://extreal-dev/PerformanceTest/Data/%my_date%/

endlocal
