#!/bin/bash

dir_name="Data_`date "+%Y%m%d%H%M%S"`"
mkdir $dir_name
cd $dir_name
~/dool/dool -Ttcglypmdrn --output `date +dstat_%Y%m%d-%H%M%S.csv`
