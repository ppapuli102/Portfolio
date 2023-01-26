"""
Given a clock time in hh:mm format, determine, to the nearest degree, the angle between the hour and the minute hands.

Bonus: When, during the course of a day, will the angle be zero?
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
mm = 45
print(clock_angle(hh, mm))
