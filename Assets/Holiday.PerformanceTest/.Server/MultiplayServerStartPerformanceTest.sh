#!/bin/bash

exec_time=3600
client_num_per_server=20
room_num=3

lifetime=`expr $exec_time + $client_num_per_server \* $room_num \* 10 + 180`
date=`date +%Y%m%d`

mkdir -p ../Data/$date
dir_count=`ls -l ../Data/$date | grep ^d | wc -l`

dir_name="Data/$date/$dir_count"
work_dir="../$dir_name"
cpu_memory_file_name="$work_dir/MultiplayServer_CpuMemoryUtilization.csv"

mkdir $work_dir

~/dool/dool -Ttcglypmdrn --output $cpu_memory_file_name 1 $lifetime

aws s3 cp $cpu_memory_file_name s3://extreal-webgl/PerformanceTest/$dir_name/
