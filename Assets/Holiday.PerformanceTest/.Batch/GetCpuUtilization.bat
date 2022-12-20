@echo off

typeperf -si 1 -o CpuUtilization.csv -y "\processor(_Total)\%% Processor Time"
