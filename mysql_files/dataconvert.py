#!/usr/bin/python3.4

infile = open("ohipfee_schedulemaster.txt", "r")
outfile = open("ohipfee_sql.txt", "w")

i = 0

for line in infile:
	outfile.write(str(i) + '\t' + line[0:4] + '\t' + line[4:12] + '\t' + line[42:49] + '.' + line[49:53] + '\n');
	i = i + 1

infile.close();
outfile.close();
