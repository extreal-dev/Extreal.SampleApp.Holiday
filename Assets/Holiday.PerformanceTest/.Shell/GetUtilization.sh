#!/bin/bash

dir_name="Data_`date +%Y%m%d%H%M%S`"
file_name=`date +dstat_%Y%m%d-%H%M%S.csv`
mkdir $dir_name
cd $dir_name
~/dool/dool -Ttcglypmdrn --output $file_name 1 5400
aws s3 cp $file_name s3://extreal-dev/Server/$dir_name/
