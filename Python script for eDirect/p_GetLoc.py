#!/usr/bin/env python3
import sys
import os
import shutil
sys.path.insert(1, os.path.dirname(shutil.which('xtract')))
import edirect

inputFileName = sys.argv[1] 
outputFileName = sys.argv[2] 
splitCharacter = sys.argv[3]
indexColumn = int(sys.argv[4]) - 1

inputFile = open(inputFileName, 'r')
outputFile = open(outputFileName, 'w')
counter = 0
lineNos = 0
print("IDs\tLines\tName")
lastAnswer=""
lastTerm=""

for line in inputFile:    
    items = line.split(splitCharacter)
    if indexColumn < len(items):
        name = items[indexColumn]
    else:
        name ="-"
    name = name.strip()
    name = name.strip("\"")
    lineNos += 1    
    if (name == "-" or name == "SYMBOL" or name == "RefSeq") == False: 
        counter += 1 
        if name == lastTerm:
            outputFile.write(lastAnswer + "\n")
        else:
            lastTerm = name
            task = f"esearch -db gene -query {name}[Gene Name] | efetch -format gdc"
            answer = edirect.pipeline(task)        
            items = answer.split("\n")
            lastAnswer = name + "\t" + items[0] + "\t" + items[1]
            outputFile.write(lastAnswer + "\n")
            outputFile.flush()
        print(str(counter) + "\t" + str(lineNos) + "\t" + lastAnswer)
    else:
        outputFile.write("-\t-\t-\n")
        outputFile.flush()
        print("-\t-\t-")
        
inputFile.close()
outputFile.close()