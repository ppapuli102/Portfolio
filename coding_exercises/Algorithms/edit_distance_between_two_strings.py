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
    #print('s1 sorted: ', sorted(s1))
    #print('s2 sorted: ', sorted(s2))
    res = 0
    my_list = list()

    if len(s1) > len(s2):
        bigger = s1
        smaller = s2
    elif len(s1) < len(s2):
        bigger = s2
        smaller = s1

    for chr in bigger:
        my_list.append((chr, abs(smaller.count(chr) - bigger.count(chr))))
    for e, v in set(my_list):
        res += v

    return res


s1 = "party"
s2 = "park"

print(edit_distance(s1, s2))
