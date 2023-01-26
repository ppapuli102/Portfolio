"""
Write a function, throw_dice(N, faces, total), that determines how many ways it is possible to throw N dice with some number of faces each to get a specific total.

For example, throw_dice(3, 6, 7) should equal 15.
"""

def throw_dice(N, faces, total):
    """
    ::type N: int,
    ::type faces: int,
    ::type total: int,
    ::rtype: int
    """
    first_letters = [f[0].lower() for f in ls]
    last_letters = [l[-1].lower() for l in ls]

    if sorted(first_letters) == sorted(last_letters):
        return True
    return False





N = 2
faces = 6
total = 4

print(throw_dice(N, faces, total))
