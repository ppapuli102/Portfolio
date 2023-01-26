def generate(numRows):
    # our null case
    if (numRows == 0):
        return []
    # our first base case
    if (numRows == 1):
        return [[1]]
    # our second base case
    if (numRows == 2):
        return [[1], [1,1]]

    # for triangles with greater than 2 rows we use our second base case to create a triangle we can start working with to edit
    Triangle = generate(2)

    # iterate from the second row all the way to the last row
    for i in range(2, numRows):
        # Create a blank row we can append to
        row = []
        # We will always start with 1 in the row of the triangle
        row.append(1)
        # for each additional row, we must add more numbers based on the previous numbers added together
        for j in range(1, i):
            row.append(Triangle[i - 1][j - 1] + Triangle[i - 1][j])
        # Each row will always end with 1
        row.append(1)
        # append our newly created row to our base triangle
        Triangle.append(row)

    return Triangle

print(generate(10))
