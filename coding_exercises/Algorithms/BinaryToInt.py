def getDecimalValue(head):
    length = len(head)-1
    power = 2**length
    res = 0
    for digit in head:
        res += digit*power
        power = power / 2
    print(res)

getDecimalValue([1,0,1,0,1,1])
