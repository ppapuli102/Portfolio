import math


def isPalindrome(s):
    s = s.lower()
    newString = []
    for ch in s:
        if ch.isalnum():
            newString.append(ch)
    if newString[::] == newString[::-1]:
        return True
    else:
        return False
