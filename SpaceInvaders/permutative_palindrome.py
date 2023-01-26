def permutativePalindrome(string):
    # create a blank list
    list = []

    # Iterate through the string for each character
    # For each character that appears twice, the list will be empty.
    # For each character that appears only once, the list will include this character.
    for chr in range(len(string)):
        if (string[chr] in list):
            list.remove(string[chr])
        else:
            list.append(string[chr])

    print('list', list)
    if ((len(string) % 2 == 0 and len(list)) == 0 or (len(string) % 2 == 1 and len(list) == 1)):
        return False
    else:
        return False

print(permutativePalindrome("geeksforgeeks"))
