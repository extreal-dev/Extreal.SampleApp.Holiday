#!/bin/bash

date=`date +%Y%m%d`

mkdir -p ../Data/$date
mkdir -p ../Logs/$date
dir_count=`ls -l ../Data/$date | grep ^d | wc -l`

dir_name=Logs/$date/$dir_count
work_dir=../$dir_mame
memory_file_name=$work_dir/server_MemoryUtilization.txt
log_file_name=$work_dir/server.log
mkdir -p $work_dir

./HolidayServer.x86_64 --memory-utilization-dump-file $memory_file_name --max-capacity 95 > $log_file_name 2>&1
