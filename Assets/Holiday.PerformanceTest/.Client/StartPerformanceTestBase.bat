@echo off
setlocal EnableDelayedExpansion

set exec_time=3600
set room_count=3
set process_count=20
set group_capacity=100

set /a player_lifetime=%exec_time%+(%room_count%-1)*%process_count%*10+120
set /a get_cpu_lifetime=%player_lifetime%+%process_count%*10
set /a loop_count=%process_count%-1

set my_date=%date:/=%
set my_date=%my_date:~8%%my_date:~4,4%

mkdir ..\Data\%my_date% > NUL 2>&1
for /f "usebackq delims=" %%A in (`dir /AD /B ..\Data\%my_date% ^| find /c /v ""`) do set dir_count=%%A

set work_dir=..\Data\%my_date%\%dir_count%
set cpu_file_name=%work_dir%\%INSTANCE_NAME%_CpuUtilization.csv
set pre_memory_file_name=%work_dir%\%INSTANCE_NAME%_MemoryUtilization
set pre_multiplay_file_name=%work_dir%\%INSTANCE_NAME%_MultiplayStatus
set pre_text_chat_file_name=%work_dir%\%INSTANCE_NAME%_TextChatStatus
set pre_voice_chat_file_name=%work_dir%\%INSTANCE_NAME%VoiceChatStatus
set pre_log_file_name=%work_dir%\Logs\%INSTANCE_NAME%_Log

if %1==include_host (
    set /a loop_start_index=1
) else (
    set /a loop_start_index=0
)
if %2==suppress_multiplay (
    set arg_for_multiplay=--suppress-multiplay
) else (
    set pre_arg_for_multiplay=--multiplay-status-dump-file %pre_multiplay_file_name%
)
if %3==suppress_text_chat (
    set arg_for_text_chat=--suppress-text-chat
) else (
    set pre_arg_for_text_chat=--text-chat-status-dump-file %pre_text_chat_file_name%
)
if %4==suppress_voice_chat (
    set arg_for_voice_chat=--suppress-voice-chat
) else (
    set pre_arg_for_voice_chat=--voice-chat-status-dump-file %pre_voice_chat_file_name%
)
if not "%5"=="" (
    set space_name=%5
) else (
    set space_name=VirtualSpace
)

set group_name=%space_name%

mkdir %work_dir%\Logs

start GetCpuUtilization.bat %get_cpu_lifetime% %cpu_file_name%

if %1==include_host (
    set counter=0
    call :SetCountToArgs !counter!
    start C:\Windows\System32\cmd.exe /c ^
        ^" ^
        .\Holiday ^
        --memory-utilization-dump-file %pre_memory_file_name%!counter!.txt ^
        -l %player_lifetime% ^
        --group-name %group_name% ^
        --space-name %space_name% ^
        --group-capacity %group_capacity% ^
        !arg_for_multiplay! ^
        !arg_for_text_chat! ^
        !arg_for_voice_chat! ^
        ^> %pre_log_file_name%!counter!.log 2^>^&1 ^
        ^"
    powershell sleep 10
)

for /l %%i in (%loop_start_index%, 1, %loop_count%) do (
    set counter=%%i
    call :SetCountToArgs !counter!
    start C:\Windows\System32\cmd.exe /c ^
        ^" ^
        .\Holiday ^
        --memory-utilization-dump-file %pre_memory_file_name%!counter!.txt ^
        -l %player_lifetime% ^
        -r Client ^
        --group-name %group_name% ^
        --space-name %space_name% ^
        !arg_for_multiplay! ^
        !arg_for_text_chat! ^
        !arg_for_voice_chat! ^
        ^> %pre_log_file_name%!counter!.log 2^>^&1 ^
        ^"
    powershell sleep 10
)

timeout %player_lifetime%
timeout 10

aws s3 cp --recursive %work_dir%\ s3://extreal-webgl/PerformanceTest/Data/%my_date%/%dir_count%/

endlocal
exit /b

:SetCountToArgs
if defined pre_arg_for_multiplay (
    set arg_for_multiplay=%pre_arg_for_multiplay%%1.txt
)
if defined pre_arg_for_text_chat (
    set arg_for_text_chat=%pre_arg_for_text_chat%%1.txt
)
if defined pre_arg_for_voice_chat (
    set arg_for_voice_chat=%pre_arg_for_voice_chat%%1.txt
)
exit /b
