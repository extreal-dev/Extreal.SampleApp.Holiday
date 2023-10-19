@echo off
setlocal

REM set exec_time=5400
set exec_time=10
set client_num=2


set /a player_lifetime=%exec_time%+60
set /a get_resource_lifetime=%player_lifetime%+%client_num%*10

set my_date=%date:/=%

mkdir ..\Data\%my_date% > NUL 2>&1
for /f "usebackq delims=" %%A in (`dir /AD /B ..\Data\%my_date% ^| find /c /v ""`) do set dir_count=%%A

set file_name=WebGLResourceUtilization.txt
echo %file_name%

typeperf -si 1 -sc %get_resource_lifetime% -o %file_name% -y "\processor(_Total)\%% Processor Time" "\Memory\Available MBytes" "\Network Interface(Realtek Gaming 2.5GbE Family Controller)\Bytes Received/sec" "\Network Interface(Realtek Gaming 2.5GbE Family Controller)\Bytes Sent/sec"

timeout %player_lifetime%

aws s3 cp %file_name% s3://extreal-webgl/PerformanceTest/Data/%my_date%/%dir_count%/

endlocal
