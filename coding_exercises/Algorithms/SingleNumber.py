def isSingle(lst):
    lst = sorted(lst)
    for i in range(0, len(lst), 2):
        if lst[i] not in lst[i+1:]:
            print(lst[i])



print(isSingle([1,1,4,2,2]))
