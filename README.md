# Hungarian-Algorithm
Algorithm solving assignment problem on an edge-weighted graphs. Finds a maximum matching with a minimal weight in O(n^3) time.

# Example program run

Example program call:
`./Application.exe -o 2_2.txt`

Following options are supported:
- `-o` to specify filename to read problem instance
- `-n` to specify number of wells
- `-k` to specify number of houses to which single well can supply water

Order of input arguments does not matter. If either n or k is specified new instance of the problem will be generated and saved to with specified filename. When generating new instance of the problem and either k or n is not specified default value which is equal to 2 will be used. If file with specified name already exists it will be overriden.

Full example program call (with generating new instance of problem - 5 wells, 30 houses)
`./Application.exe -o 2_2.txt -n 5 -k 6`
