import random

hist = {'a': 2, 'b': 1, 'c': 5}

def coin_flip(hist):
    t = []
    for key, value in hist.items():
        for i in range(value):
            t.append(key)
    return print(random.choice(t))

coin_flip(hist)
