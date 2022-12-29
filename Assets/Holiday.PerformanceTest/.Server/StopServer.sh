#!/bin/bash

pid=`ps aux | grep HolidayServer.x86_64 | awk 'NR == 1 { print $2 }'`
sudo kill $pid
