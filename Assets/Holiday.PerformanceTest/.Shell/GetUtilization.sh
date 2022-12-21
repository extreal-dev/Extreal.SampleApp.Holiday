#!/bin/bash

date=`date +%Y%m%d`
time=`date +%H%M%S`
dir_name="${date}_$time"
file_name="${dir_name}_server_CpuMemoryUtilization.csv"

mkdir ../Data/$dir_name
cd ../Data/$dir_name

~/dool/dool -Ttcglypmdrn --output $file_name 1 150
aws s3 cp $file_name s3://extreal-dev/PerformanceTest/Data/$date/
