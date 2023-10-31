@echo off
setlocal

set exec_time=3800
set client_num=9


set /a player_lifetime=%exec_time%+60
set /a get_resource_lifetime=%player_lifetime%+%client_num%*10

set my_date=%date:/=%

for /f "usebackq delims=" %%A in (`dir /AD /B ..\Data\%my_date% ^| find /c /v ""`) do set dir_count=%%A
mkdir ..\Data\%my_date%\%dir_count% > NUL 2>&1

set file_name=WebGLResourceUtilization.csv
set file_path=..\Data\%my_date%\%dir_count%\
echo %file_path%%file_name%

typeperf -si 1 -sc %get_resource_lifetime% -o %file_path%%file_name% -y "\processor(_Total)\%% Processor Time" "\Memory\Available Bytes" "\Network Interface(Realtek Gaming 2.5GbE Family Controller)\Bytes Received/sec" "\Network Interface(Realtek Gaming 2.5GbE Family Controller)\Bytes Sent/sec"

timeout 100

aws s3 cp %file_name% s3://extreal-webgl/PerformanceTest/Data/%my_date%/%dir_count%/

endlocal
