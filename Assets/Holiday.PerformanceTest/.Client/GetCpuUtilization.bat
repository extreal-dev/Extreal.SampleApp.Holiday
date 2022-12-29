@echo off

set lifetime=%1
set file_name=%2
typeperf -si 1 -sc %lifetime% -o %file_name% -y "\processor(_Total)\%% Processor Time"
exit
