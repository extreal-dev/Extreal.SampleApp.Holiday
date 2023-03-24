#!/bin/bash

if [[ $1 == "" ]]; then
    work_dir="."
else
    work_dir=$1
fi

memory_file_name="$work_dir/server_MemoryUtilization.txt"
log_file_name="$work_dir/server.log"
mkdir -p $work_dir

./HolidayServer.x86_64 --memory-utilization-dump-file $memory_file_name --max-capacity 95 > $log_file_name 2>&1
