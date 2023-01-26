"""
A Collatz sequence in mathematics can be defined as follows. Starting with any positive integer:

if n is even, the next number in the sequence is n / 2
if n is odd, the next number in the sequence is 3n + 1
It is conjectured that every such sequence eventually reaches the number 1. Test this conjecture.

Bonus: What input n <= 1000000 gives the longest sequence?
"""

def clock_angle(hh, mm):
    """
    ::type a: int,
    ::type b: int,
    ::rtype: s
    """
    if hh == 12:
        hour_distance = 0
    else:
        hour_distance = hh * 30
    minute_distance = mm * 6

    return abs(hour_distance - minute_distance)

    print(hour_distance, minute_distance)

hh = 12
mm = 15
print(clock_angle(hh, mm))
