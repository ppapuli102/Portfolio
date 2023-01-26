def print_n(s,n):
    if n<=0:
        return
    print(s)
    print_n(s, n-1)

def do_n(fn, n):
    if n==0:
        return
    else:
        s="Hello"
        fn(s, n)
        do_n(print_n, n-1)


do_n(print_n, 3)
