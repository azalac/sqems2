#!/usr/bin/python3.4

infile = open("ohipfee_schedulemaster.raw", "r")
outfile = open("ohipfee_sql.csv", "w")

i = 0

for line in infile:
	outfile.write(str(i) + ', ' + line[0:4] + ', ' + line[4:12] + ', ' + line[42:49] + '.' + line[49:53] + '\n');
	i = i + 1

infile.close();
outfile.close();
