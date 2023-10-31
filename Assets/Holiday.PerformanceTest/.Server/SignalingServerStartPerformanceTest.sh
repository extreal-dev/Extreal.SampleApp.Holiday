#!/bin/bash

exec_time=3800
client_num_per_server=9

lifetime=`expr $exec_time + $client_num_per_server \* 10 + 180`
date=`date +%Y%m%d`

mkdir -p ../Data/$date
dir_count=`ls -l ../Data/$date | grep ^d | wc -l`

dir_name="Data/$date/$dir_count"
work_dir="../$dir_name"
cpu_memory_file_name="$work_dir/signalingServer_CpuMemoryUtilization.csv"

mkdir $work_dir

~/dool/dool -Ttcglypmdrn --output $cpu_memory_file_name 1 $lifetime

aws s3 cp $cpu_memory_file_name s3://extreal-webgl/PerformanceTest/$dir_name/

sleep 100
mkdir $work_dir/tmp
aws s3 cp --recursive s3://extreal-webgl/PerformanceTest/$dir_name/ $work_dir/tmp/

for i in `seq 1 $((client_num_per_server - 1))`
do
    python3 DataAnalysis.py $work_dir/tmp/_MemoryUtilization${i}\*
    aws s3 cp $work_dir/tmp/_MemoryUtilization${i}.csv s3://extreal-webgl/PerformanceTest/$dir_name/
done

mv $work_dir/tmp/NGO_Client_00?_MemoryUtilization.csv $work_dir
rm -rf $work_dir/tmp
