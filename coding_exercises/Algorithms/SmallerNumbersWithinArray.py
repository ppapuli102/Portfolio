# Given the array nums, for each nums[i] find out how many numbers in the array
# are smaller than it. That is, for each nums[i] you have to count the number of
# valid j's such that j != i and nums[j] < nums[i]

def smallerNumbersThanCurrent(nums):
        res = []
        srtdnums = sorted(nums)
        for num in nums:
            res.append(srtdnums.index(num))
        return res




nums = [8, 1, 2, 2, 3]
print(smallerNumbersThanCurrent(nums))
