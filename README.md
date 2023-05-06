# Hungarian-Algorithm
Algorithm solving assignment problem on an edge-weighted graphs. Finds a maximum matching with a minimal weight in O(n^3) time.

# Example program run

Example program call:

`./Application.exe -i 2_2.txt`

Following options are supported:
- `-i` (**i**nput filename) to specify filename to read problem instance 
- `-o` (**o**utput filename) to specify filename to save solution to (optional)
- `-n` (**n**) to specify number of wells (optional, default=2)
- `-k` (**k**) to specify number of houses to which single well can supply water (optional, default=2)
- `-v` (**v**erbose) whether logs with current state of the program should be displayed (optional, default=false)
- `-s` (**s**how solution) whether found solution should be displayed on the console (optional, default=false)

Order of input arguments does not matter. All flags need to specified separately (for instance -vs is not supported).
If either n or k is specified new instance of the problem will be generated and saved with the specified input filename.
When generating new instance of the problem and either k or n is not specified default value which is equal to 2 will be used.

Full example program call (with generating new instance of problem - 5 wells, 30 houses, displaying logs, showing solution on console and writing solution to custom file):

`./Application.exe -i 5_6_generated.txt -o 5_6_solution -n 5 -k 6 -v -s`

If output filename is not specified calculated solution is saved in the same folder and the with the same name as input file but with `_output.txt` suffix.

If file with specified name already exists it will be overriden.
