#!/bin/bash

for i in `seq -f %02g 1 15`
do
	echo ${i}
	python3 DataAnalysis.py 5/NGO_Client_0${i}_MemoryUtilization\*
done