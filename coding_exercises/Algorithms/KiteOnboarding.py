# Welcome to...
#
#         `hmy+.                   ://:
#        .mMMMMMNho:`              NMMm
#       :NMMMMMMMMMMMds/.`         NMMm            :ss:
#      +NMMMMMMMMMMMMMMMMmy+       NMMm           -MMMM-   ---`
#    `oMMMMMMMMMMMMMMMMMMMMo       NMMm            /ss/   :MMM+
#   `yMMMMMMMMNshmNMMMMMMMN`       NMMm                   /MMM+          `````
#  .dMMMMMMMMm/hmhssydmMMM+        NMMm    `/yhhy. shhy ohmMMMmhhhh.  ./ydmmmdho-
#  omMMMMMMMd/mMMMMMmhsosy`        NMMm  .omMMmo.  mMMN odmMMMmdddd.`omMNdsoshNMNy`
#   .+dMMMMy/mMMMMMMMMMMm-         NMMm-yNMMh/`    mMMN   /MMM+     sMMN:`   `:NMMy
#     `-ymo/NMMMMMMMMMMMd          NMMMNMMN/       mMMN   :MMM+    .MMMNdddddddNMMN
#        ``hMMMMMMMMMMMM:          NMMm+mMMNs.     mMMN   :MMM+    `MMMh//////////:
#          `:yNMMMMMMMMh           NMMm `/dMMNy-   mMMN   :MMM+  `. sMMNo`    `-:
#             .+mMMMMMM-           NMMm   `/dMMNy- mMMN   .MMMNddNN/ +NMMNdhydNNMs
#               `:yMMMy            yhhs     `/hhhh:shhs    :ymmmdho:  `/sdmmmmhs/`
#                  `om.

""" Kite is your programming co-pilot. This playground will teach you
    all of its superpowers.

    We'll be building a simple program to fetch some data from the web
    and display it. To make our lives easier, Kite will be assisting us
    along the way.


    //////////////////
    //              //
    //  DISCLAIMER  //
    //              //
    //////////////////

    This requires a stable internet connection to work properly -
    we're fetching remote data, after all! You may experience varied
    results if your network connection is slow.

    If you get stuck at any point, please email feedback@kite.com and
    we'll help you debug.

NOTE: This is a Python 2 program, but Kite works with Python 3 too. """

# Python version compatibility check
import sys
if sys.version_info[0] > 2:
  print("We're sorry - we can only run this tutorial in Python 2, and you're running it in Python 3")
  sys.exit(1)

# 1. HELP ON HOVER

# Let's start by importing the urlib2 package.

# Hover your mouse cursor over "urllib2" and click "Docs" to see Kite help
# you with a rich, interactive explanation in our Copilot application.

import urllib2


def fetch_and_print(url):
    """ Fetches data from a URL and then prints it.
    """

    # 2. SMART AUTOCOMPLETE

    # Let's define our function to fetch data from a URL.

    # Try typing "." after urllib2 to see how Kite can help you with a
    # smarter autocomplete.

    # Hint: The function we want to use is "urlopen".

    fetch = urllib2



    # 3. AUTOCOMPLETE THAT GOES BEYOND

    # Add "(" below to see that Kite doesn't stop with autocomplete - it
    # helps you with the function signature, and shows you how other
    # programmers use the function.

    # Hint: Pass in the "url" variable to our fetch function.

    response = fetch


    # 4. WRAP IT UP

    # Almost there! Now read the data from response and then print it.

    # Hint: Use response's "read" method to get the data and the builtin
    # "print" function to display it.

    # Read the data and print it



# 5. YOUR CODE IS AS IMPORTANT AS EVERYONE ELSE'S

# Call your fetch_and_print function on kite_url. Kite will jump in to
# help, even on code it's seeing for the very first time.

kite_url = 'https://kite.com/onboarding/'

# Call your function here



# 6. RUN IT

# Try running this program from your terminal to see what Kite has in
# store for you :)

# Don't forget to save the file before running it!

# HINT: This file should be located in your home directory



""" That's it! Enjoy using Kite.

Click on the Kite icon at the bottom of your editor to learn more and to
configure it.

P.S. You can access this onboarding anytime by running "Kite: Tutorial"
from the command palette """
