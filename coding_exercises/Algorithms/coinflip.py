import math

def zeroCoin(n):
    return math.ceil(math.log(n, 2))

print(zeroCoin(10))
