

def sockMerchant(n, ar):
    '''
    Return an integer representing the number of matching pairs of
    socks that are available to sell.

    n = number of socks
    ar = array of socks represented by integers
    '''
    arrSum = []
    for i in ar:
        print('i ', i)
        if ar.count(i) >= 2:
            print('arcount ', ar.count(i))
            if ar.count(i) not in arrSum:
                arrSum.append(ar.count(i))
                print('arrsum ', arrSum)

    return arrSum[0]/2 + arrSum[1]/2

n = 9
ar = [5, 5, 5, 6, 6, 7, 8, 9, 5]

print(sockMerchant(n, ar))
