@echo off

typeperf -si 1 -sc 5500 -o %1\%2_%INSTANCE_NAME%_CpuUtilization.csv -y "\processor(_Total)\%% Processor Time"
exit
