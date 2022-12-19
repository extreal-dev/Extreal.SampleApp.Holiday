@echo off

typeperf -si 1 -o %1/CpuUtilization.csv -y "\processor(_Total)\%% Processor Time"
