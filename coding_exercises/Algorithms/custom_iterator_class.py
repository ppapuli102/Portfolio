"""
Given a sorted list of integers, square the elements and give the output in sorted order.

For example, given [-9, -2, 0, 2, 3], return [0, 4, 4, 9, 81].
"""

class Two_D_Iterator:
    i = 0
    j = 0

    def __init__(self, arr):
        self.arr = arr

    def next(self):
        val = arr[self.i][self.j]
        self.UpdateJValue()
        return val

    def UpdateJValue(self):
        self.j += 1
        try:
            arr[self.i][self.j]
        except:
            self.j = 0
            self.UpdateIValue()

    def UpdateIValue(self):
        if self.has_next():
            self.i += 1
            self.CheckForEmptyArray()
        else:
            return print("ERROR")

    def CheckForEmptyArray(self):
        if self.i > len(arr) - 1:
            return
        elif len(arr[self.i]) == 0:
            self.i += 1

    def has_next(self):
        return arr[len(arr)-1][len(arr[-1])-1] != arr[self.i][self.j]



arr = [[1, 2], [3], [], [4, 5, 6], [1,4,6,8]]
my_iterator = Two_D_Iterator(arr)

print(my_iterator.next(), '\n')
print(my_iterator.next(), '\n')
print(my_iterator.next(), '\n')
print(my_iterator.next(), '\n')
print(my_iterator.next(), '\n')
print(my_iterator.next(), '\n')
print(my_iterator.next(), '\n')
print(my_iterator.next(), '\n')
print(my_iterator.next(), '\n')
print(my_iterator.next(), '\n')
print(my_iterator.next(), '\n')
