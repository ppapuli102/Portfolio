"""
Given a clock time in hh:mm format, determine, to the nearest degree, the angle between the hour and the minute hands.

Bonus: When, during the course of a day, will the angle be zero?
"""

def convert_to_egyptian_num(a, b):
    """
    ::type a: int,
    ::type b: int,
    ::rtype: s
    """
    if a > 1:
        return ('1 / ' + '{} + '.format(b)) * a
    if a == 1:
        return '{} / {}'.format(a,b)

a = 4
b = 13
print(convert_to_egyptian_num(a, b))
