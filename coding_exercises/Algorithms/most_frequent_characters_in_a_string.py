def most_frequent(s):
    d = dict()
    for letter in list(s):
        if letter not in d:
            d[letter] = 1
        elif letter in d:
            d[letter] += 1
    temp = []
    sorted_letters = []
    for k,v in d.items():
        temp.append((v, k))
    temp = sorted(temp, reverse = True)
    for freq, x in temp:
        sorted_letters.append(x)
    return sorted_letters




s = "leonardo di caprio"
print(most_frequent(s))
