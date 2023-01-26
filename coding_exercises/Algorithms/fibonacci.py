def fibonacci(n):
    if n == 1:
        return 1
    elif n == 2:
        return 1
    else:
        return (fibonacci(n-1) + fibonacci(n-2))

n = input("enter 'n': ")
print(fibonacci(float(n)))
