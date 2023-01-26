from sys import argv #imports the argument variable module from sys

script, input_file = argv #sets the arguments for the module

def print_all(f): #define the function print_all with variable f
    print(f.read()) #reads the file which is passed in the function

def rewind(f): #define the function rewind which has variable f
    f.seek(0,0) #uses the seek function to go back to the beginning of the file

def print_a_line(line_count, f): #defines function print_a_line with two variables
    print(line_count, f.readline()) #print variable line_count, and then uses the readline function

current_file = open(input_file) #create a var with file object of our input file argument passed from the module

print("First let's print the whole file:\n")

print_all(current_file) #run the fn to read the entire file

print("Now let's rewind, kind of like a tape.")

rewind(current_file) #run the fn to go back to the biggening

print("Let's print three lines:")

#print three different lines using the fn print_a_line
current_line = 1
print_a_line(current_line, current_file)

current_line +=  1
print_a_line(current_line, current_file)

current_line += 1
print_a_line(current_line, current_file)
