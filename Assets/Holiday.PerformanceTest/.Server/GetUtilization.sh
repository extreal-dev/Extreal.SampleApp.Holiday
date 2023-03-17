#!/bin/bash

exec_time=5400
client_num_per_server=18

lifetime=`expr $exec_time + $client_num_per_server \* 10 + 120`

date=`date +%Y%m%d`

mkdir -p ../Data/$date
dir_count=`ls -l ../Data/$date | grep ^d | wc -l`

dir_name=Data/$date/$dir_count
work_dir="../$dir_name"
cpu_memory_file_name="$work_dir/server_CpuMemoryUtilization.csv"
memory_file_name="../Logs/$date/$dir_count/server_MemoryUtilization.txt"

mkdir $work_dir

~/dool/dool -Ttcglypmdrn --output $cpu_memory_file_name 1 $lifetime
./StopServer.sh

aws s3 cp $cpu_memory_file_name s3://extreal-dev/PerformanceTest/$dir_name/
aws s3 cp $memory_file_name s3://extreal-dev/PerformanceTest/$dir_name/

sleep 100
mkdir $work_dir/tmp
aws s3 cp --recursive s3://extreal-dev/PerformanceTest/$dir_name/ $work_dir/tmp/

for i in `seq 1 5`
do
    python3 DataAnalysis.py $work_dir/tmp/NGO_Client_00${i}_MemoryUtilization\*
    aws s3 cp $work_dir/tmp/NGO_Client_00${i}_MemoryUtilization.csv s3://extreal-dev/PerformanceTest/$dir_name/
done

mv $work_dir/tmp/NGO_Client_00?_MemoryUtilization.csv $work_dir
rm -rf $work_dir/tmp
