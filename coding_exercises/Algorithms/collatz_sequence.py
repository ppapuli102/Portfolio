"""
Given a string, determine whether any permutation of it is a palindrome.

For example, carrace should return true, since it can be rearranged to form racecar, which is a palindrome.
daily should return false, since there's no rearrangement that can form a palindrome.
"""

def collatz_sequence(n):
    """
    ::type n: int,
    ::rtype: int
    """
    c = 0
    while n != 1:
        if n % 2 == 0:
            n /= 2
            c += 1
        elif n % 2 == 1:
            n = 3 * n + 1
            c += 1
    known_sequences[n] = c
    return c


known_sequences = dict()
n = 524
print(collatz_sequence(n))

#print(max([collatz_sequence(i) for i in range(1,1000000)]))
