import os 
import os.path 
import shutil 
import time,  datetime

def duplicateFile(sourceFile,  startIndex, count):
	sourceFile = os.path.join(os.path.abspath('.'), sourceFile)
	for i in range(startIndex, startIndex + count):
		targetFile = os.path.join(os.path.abspath('.'), "Npc%d.xlsx"%i)
		open(targetFile, "wb").write(open(sourceFile, "rb").read())

targetDirs = [os.path.abspath('.')]
sourceDir = os.path.abspath('.')

print "\n>>>begin !\n"

duplicateFile("Npc0.xlsx", 1, 99)
	
print "\n>>>done !\n"
	









