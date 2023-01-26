def commonChars(A):
        original = list(A[0])
        for word in A[1:]:
            newlist = []
            for chr in word:
                if chr in original:
                    newlist.append(chr)
                    original.remove(chr)
            original = newlist

        return sorted(original)

def smallerNumbersThanCurrent(nums):
    res = []
    srtdnums = sorted(nums)
    for num in nums:
        res.append(srtdnums.index(num))
    return res



nums = [7,7,7,7]
print(smallerNumbersThanCurrent(nums))

test = ['bella', 'label', 'roller']
#print(commonChars(A))
