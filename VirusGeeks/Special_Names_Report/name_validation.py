"""
    Create an output of names tested on the previous day with special characters.
    A special character is defined as not part of the alphabet (lowercase or uppercase) and without a hyphen
"""
import re
from pymongo import MongoClient
import pandas as pd
import numpy as np
from dotenv import load_dotenv
import os
import datetime
# import VirusGeeks as vg


# Load our .env file which holds all credentials
load_dotenv()

# Create connection to Mongo
MONGO_HOST = os.getenv('MONGO_HOST')
client = MongoClient(MONGO_HOST)

# Create pandas dataframe of participants with a special case regex
participants = client['VGP']['participants_full'].aggregate([
    {
        '$unwind': {
            'path': '$person'
        }
    }, {
        '$unwind': {
            'path': '$testKits'
        }
    }, {
        '$match': {
            '$or': [
                {
                    'person.firstName': re.compile(r"[^-a-zA-Z[:blank:]]")
                }, {
                    'person.lastName': re.compile(r"[^-a-zA-Z[:blank:]]")
                }
            ]
        }
    }, {
        '$match': {
            'status': 0
        }
    }, {
        '$project': {
            'person_key': '$person._id', 
            'testkit_key': '$testKits._id',
            'first_name': '$person.firstName', 
            'last_name': '$person.lastName', 
            'dob': '$person.dob', 
            'email': '$person.email', 
            'testsite_name': '$testKits.testSite.shortName', 
            'collection_date': '$testKits.collectionDate'
        }
    }
])
participants_list = list(participants)
df = pd.DataFrame(data = participants_list)
print(df)

def stringDate(dt):
    """
        Takes a datetime "dt" 
        returns it as a string formatted: %Y%m%d
    """
    date = str(dt).split(" ")[0].split("-")
    date = "{}{}{}".format(date[0], date[1], date[2])

    return str(date)


print("How many previous days to run report for: \n")
days_to_run = int(input())

def runReport(n):
    '''
        Creates a df for the last "n" days with special characters
        e.g. n = 2 will give yesterdays and the day before's special character names
    '''
    df_func = pd.DataFrame()

    for i in range(1, n+1):
        collection_date = pd.to_datetime('today') - pd.Timedelta(i, unit='D')
        collection_date = stringDate(collection_date)

        df_func = df_func.append(df[df['collection_date'] == collection_date])

    return df_func



df_report = runReport(days_to_run)

print(df_report)


# # Output all dataframes to a csv file
# Create a Pandas Excel writer using XlsxWriter as the engine
writer = pd.ExcelWriter('../Data/special_names_report.xlsx', engine='xlsxwriter')

# Write each dataframe to its own sheet
df_report.to_excel(writer, sheet_name='today'.format())

#close the Pandas Excel writer and output the Excel file
writer.save()