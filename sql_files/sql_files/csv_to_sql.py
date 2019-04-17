#/bin/user/python

infile = open("ohipfee_sql.csv", "r")
outfile = open("ohipfee_insert.sql", "w")

chunk_size = 1000

i = 0

header = """
USE [EMS2]
GO
"""

def do_chunk(infile, outfile, chunk):
	global i
	data = []
	for i2 in range(chunk):
		line = infile.readline()[:-1].replace("\'", "\\\'").strip()
		if line == '':
			break
		data.append('\t({0})'.format(','.join(["'{0}'".format(col.strip()) for col in line.split(",")])))
	
	outfile.write("INSERT INTO [MasterBillingCode] VALUES\n")
	outfile.write(',\n'.join(data))
	outfile.write(";")
	
	i = i + len(data)
	
	print("Writing chunk with {0} rows".format(len(data)))
	return len(data) == chunk

outfile.write(header)

while(do_chunk(infile, outfile, chunk_size)):
	print("{0} values completed".format(i))

print("Finished {0} rows".format(i))

infile.close()
outfile.close()
