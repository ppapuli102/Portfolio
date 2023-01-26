import random

def split_list(lst):
    random.shuffle(lst)
    half = len(lst)//2
    return lst[:half], lst[half:]

data_team = ['peter', 'augustine', 'chris', 'qili', 'xinhan', 'celine', 'ramy', 'tianmu', 'darish', 'hang', 'william']
print(split_list(data_team))
