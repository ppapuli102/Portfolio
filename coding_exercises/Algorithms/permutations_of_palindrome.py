"""
Using a read7() method that returns 7 characters from a file, implement readN(n) which reads n characters.

For example, given a file with the content “Hello world”, three read7() returns “Hello w”, “orld” and then “”
"""

def palindrome_permutations(s):
    """
    ::type s: string,
    ::rtype: bool
    """
    his = create_histogram(s)
    temp = list()
    for k, v in his.items():
        if v % 2 == 0:
            continue
        elif v % 2 == 1:
            temp.append(k)

    if len(temp) > 1:
        return False
    else:
        return True

def create_histogram(s):
    his = {}
    for chr in s:
        if chr not in his:
            his[chr] = 1
        elif chr in his:
            his[chr] += 1
    return his


s = 'tattarrattat'
print(sorted(s))
print(palindrome_permutations(s))
