def uniqueNonRepeat(S):
    res = []
    for chr in S:
        if S.count(chr) == 1:
            res.append(chr)
    return len(res)


S = "ALABAMAA"
print(uniqueNonRepeat(S))
