#!/bin/bash

exec_time=5400
client_num_per_server=8

lifetime=`expr $exec_time + $client_num_per_server \* 10 + 120`
date=`date +%Y%m%d`

mkdir -p ../Data/$date
dir_count=`ls -l ../Data/$date | grep ^d | wc -l`

dir_name="Data/$date/$dir_count"
work_dir="../$dir_name"
cpu_memory_file_name="$work_dir/appUsageServer_CpuMemoryUtilization.csv"
memory_file_name="../Logs/$date/$dir_count/appUsageServer_MemoryUtilization.txt"

mkdir $work_dir

~/dool/dool -Ttcglypmdrn --output $cpu_memory_file_name 1 $lifetime

aws s3 cp $cpu_memory_file_name s3://extreal-webgl/PerformanceTest/$dir_name/
aws s3 cp $memory_file_name s3://extreal-webgl/PerformanceTest/$dir_name/
