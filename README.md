# Get LOC

<img align="right" src="Guide/images/introdution.jpg">

This repository contains a Desktop application and an independent python script that allows you to search the NCBI Gene website for the Gene name and brief description of a gene based on its gene SYMBOL or RefSeq accession ID. While the application and script duplicate each others functions they work in different ways:

### Application: Get_LOC
* Searchers the NCBI web site directly using this url (where {term} is replaced by the gene SYMBOL or accession ID)-  
>  https://www.ncbi.nlm.nih.gov/gene/?term={term}&report=docsum&format=text          
* This program has no dependances other than the .NET framework
* While primarily a Windows program it can run on a range of Linux and macOS computers with the aid of Wine ([website](https://www.winehq.org/) and [guide](https://github.com/msjimc/RunningWindowsProgramsOnLinux)).

### Python script: p_Get_LOC.py   
* Searchers NCBI's Gene dataset using Python implementation of EDirect using the edirect.pipeline process with this command -   
> esearch -db gene -query {name}[Gene Name] | efetch -format gdc     

* The script requires that EDirect is installed in your computer.
* Since eDirect is targeted to Linux and macOS but can be run on WIndows with the Cygwin Unix-emulation environment.  

## Considerations
Neither of the methods is fast, its designed to process sequences that can't readily be processed by other means and you should aim to process a limited dataset rather than all genes in a large dataset. This is important for the application as NCBI monitors its webpage and excessive use may lead to a temporary ban from using its site from your IP address.   

## Guide

* [User guide - Application](Guide/README.md) 
* [User guide - Script](Python%20script%20for%20eDirect//README.md) 

## Download
* [Compiled Desktop program](Program/)
* [Python script](Python%20script%20for%20eDirect/)