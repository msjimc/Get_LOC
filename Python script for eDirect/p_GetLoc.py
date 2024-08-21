#!/usr/bin/env python3
import sys
import os
import shutil
sys.path.insert(1, os.path.dirname(shutil.which('xtract')))
import edirect


#"/nobackup/msjimc/jenni/Analysis/MilkFat_/0_05/DeSeq2_analysis/MilkFat__standard_analysis_sig_Deseq2_names.csv"
inputFileName = sys.argv[1] 
outputFileName = sys.argv[2] 

inputFile = open(inputFileName, 'r')
outputFile = open(outputFileName, 'w')
counter = 0
lineNos = 0
print("Locs\tLines\tName")
lastAnswer=""
lastTerm=""

for line in inputFile:    
    items = line.split(",")
    name = items[len(items) - 1]
    name = name.strip()
    name = name.strip("\"")
    lineNos += 1    
    if name.startswith("LOC"): 
        counter += 1 
        if name == lastTerm:
            outputFile.write(lastAnswer + "\n")
        else:
            lastTerm = name
            task = f"esearch -db gene -query {name} | efetch -format gdc"
            answer = edirect.pipeline(task)        
            items = answer.split("\n")
            lastAnswer = name + "\t" + items[0] + "\t" + items[1]
            outputFile.write(lastAnswer + "\n")
            outputFile.flush()
        print(str(counter) + "\t" + str(lineNos) + "\t" + lastAnswer)
        
inputFile.close()
outputFile.close()