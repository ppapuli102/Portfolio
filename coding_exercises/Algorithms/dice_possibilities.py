"""
Implement a 2D iterator class. It will be initialized with an array of arrays, and should implement the following methods:

next(): returns the next element in the array of arrays. If there are no more elements, raise an exception.
has_next(): returns whether or not the iterator still has elements left.
For example, given the input [[1, 2], [3], [], [4, 5, 6]], calling next() repeatedly should output 1, 2, 3, 4, 5, 6.

Do not use flatten or otherwise clone the arrays. Some of the arrays can be empty.
"""

def throw_dice(faces, N, total):
    """
    ::type N: int,
    ::type faces: int,
    ::type total: int,
    ::rtype: int
    """
    table=[[0]*(total+1) for i in range(N+1)] #Initialize all entries as 0

    for j in range(1,min(faces+1,total+1)): #Table entries for only one dice
        table[1][j]=1

    # Fill rest of the entries in table using recursive relation
    # i: number of dice, j: sum
    for i in range(2,N+1):
        for j in range(1,total+1):
            for k in range(1,min(faces+1,j)):
                table[i][j]+=table[i-1][j-k]

    return table[-1][-1]




N = 3
faces = 6
total = 7

print(throw_dice(faces, N, total))
