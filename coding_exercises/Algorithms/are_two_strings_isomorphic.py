"""
This problem was asked by Bloomberg.

Determine whether there exists a one-to-one character mapping from one string s1 to another s2.

For example, given s1 = abc and s2 = bcd, return true since we can map a to b, b to c, and c to d.

Given s1 = foo and s2 = bar, return false since the o cannot map to two characters.
"""

def isomorphic_strings(s1, s2):
    """
    ::type s1: string,
    ::type s2: string,
    ::rtype: boolean
    """
    return len(s1) == len(s2) & len(set(s1)) == len(s1) & len(set(s2)) == len(s2)




s1 = 'abc'
s2 = 'bcd'

print(isomorphic_strings(s1, s2))
