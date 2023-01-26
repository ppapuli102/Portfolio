"""
The edit distance between two strings refers to the minimum number of character insertions, deletions, and substitutions required to change one string to the other.
For example, the edit distance between “kitten” and “sitting” is three: substitute the “k” for “s”, substitute the “e” for “i”, and append a “g”.

Given two strings, compute the edit distance between them.
"""

def edit_distance(s1, s2):
    """
    ::type s1: string,
    ::type s2: string,
    ::rtype: int
    """
    txt = f.read()
    res = list()
    for x in range(n):
        res.append(txt[x])
    return ''.join(res)



s1 = "kitten"
s2 = "sitting"

print(edit_distance(fin, n))
