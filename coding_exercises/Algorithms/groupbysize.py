def groupThePeople(groupSizes):
    dic = {}
    for i,v in enumerate(groupSizes):
        if v in dic:
            dic[v].append(i)
        else:
            dic[v] = [i]
    res = []

    for size in dic:
        while len(dic[size]) >= size:
            res.append(dic[size][:size])
            del dic[size][:size]
    return res

groupSizes = [3,3,3,3,3,1,3]
print(groupThePeople(groupSizes))
