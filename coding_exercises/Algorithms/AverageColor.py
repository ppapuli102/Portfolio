# Solution in Python

def AverageColor(color1, color2):
    """
        string color1,
        string color2,
        rtype string
    """

    import textwrap
    import math

    def HexToDec(hex):
        """
            string hex,
            rtype int
        """
        return int(hex, 16)

    def DecToHex(dec):
        """
            int dec,
            rtype string
        """
        return '{}'.format(hex(dec))

    rgbAverage = []
    rgb1 = textwrap.wrap(color1, 2)
    rgb2 = textwrap.wrap(color2, 2)

    for element in [rgb1, rgb2]:
        i = 0
        for color in element:
            element[i] = HexToDec(color)
            i += 1

    for i in range(len(rgb1)):
        rgbAverage.append(math.floor((rgb1[i] + rgb2[i]) / 2))

    for i in range(len(rgbAverage)):
        rgbAverage[i] = DecToHex(rgbAverage[i])

    rgbAnswer = "{}{}{}".format(rgbAverage[0],rgbAverage[1], rgbAverage[2])

    return rgbAnswer

AverageColor("000000", "FFFFFF")
