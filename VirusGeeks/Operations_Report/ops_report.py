'''
    Version 1.0

    Daily Report for operations which details the previous test results of VG staff

    Data is taken from VGP.participants joined with testkits and testresults

    Output includes one dataframe:
    1) df: The previous tests, collection date, names of vg staff who were tested

    Data is output into a .xlsx file and uploaded into VGP's sharepoint Documents > VG Staff Covid-19 Tests
    https://vg100.sharepoint.com/sites/VGOps/Shared%20Documents/Forms/AllItems.aspx?id=%2Fsites%2FVGOps%2FShared%20Documents%2FVG%20Staff%20Covid%2D19%20Tests&p=true&ga=1

    Author: Peter Papuli
    Last Modified Date: 5/26/2022
    Last Modified By: Peter Papuli 
'''

from datetime import datetime, tzinfo, timezone
from pymongo import MongoClient
import pandas as pd
import numpy as np
import os
from dotenv import load_dotenv
import unicodedata
from datetime import datetime, timedelta
# from office365.sharepoint.client_context import ClientContext


load_dotenv()


# SHAREPOINT_URL=os.getenv('OPS_REPORT_URL')
# SHAREPOINT_USERNAME=os.getenv('SHAREPOINT_USERNAME')
# SHAREPOINT_PASSWORD=os.getenv('SHAREPOINT_PASSWORD')

# ctx = ClientContext(SHAREPOINT_URL)
# ctx.with_user_credentials(SHAREPOINT_USERNAME, SHAREPOINT_PASSWORD)

# web = ctx.web.get().execute_query()
# print(web.url)


# Datetime variables
now = datetime.now() # current date and time
year = now.strftime("%Y")
month = now.strftime("%m")
day = now.strftime("%d")
time = now.strftime("%H:%M:%S").split(":")
hour, minute, second = time[0], time[1], time[2]
milliseconds_in_a_day = 86400000


# Load data from Mongo
MONGO_HOST = os.getenv('MONGO_HOST')
client = MongoClient(MONGO_HOST)

result = client['VGP']['participants'].aggregate([
    {
        '$match': {
            'status': 0, 
            'groups': {
                '$in': [
                    '$groups', 'VG Staff-Staff'
                ]
            }
        }
    }, {
        '$lookup': {
            'from': 'testkits', 
            'localField': 'testKit_ids', 
            'foreignField': '_id', 
            'as': 'testkits'
        }
    }, {
        '$lookup': {
            'from': 'testresults', 
            'localField': 'testkits.result_id', 
            'foreignField': '_id', 
            'as': 'testresults'
        }
    }, {
        '$addFields': {
            'lastTest': {
                '$last': '$testkits'
            }, 
            'previousTest': {
                '$first': {
                    '$slice': [
                        '$testkits', -2, 1
                    ]
                }
            }, 
            'lastResult': {
                '$last': '$testresults'
            }, 
            'previousResult': {
                '$first': {
                    '$slice': [
                        '$testresults', -2, 1
                    ]
                }
            }, 
            'keep': {
                '$cmp': [
                    {
                        '$last': '$testkits.collectionDate'
                    }, {
                        '$dateToString': {
                            'date': datetime.today() - timedelta(14), 
                            'format': '%Y%m%d'
                        }
                    }
                ]
            }
        }
    }, {
        '$match': {
            'keep': {
                '$gt': -1
            }
        }
    }, {
        '$lookup': {
            'from': 'people', 
            'localField': 'person_id', 
            'foreignField': '_id', 
            'as': 'person'
        }
    }, {
        '$lookup': {
            'from': 'users', 
            'localField': 'person_id', 
            'foreignField': 'person_id', 
            'as': 'users'
        }
    }, {
        '$unwind': {
            'path': '$users', 
            'preserveNullAndEmptyArrays': True
        }
    }, {
        '$project': {
            'firstName': '$person.firstName', 
            'lastName': '$person.lastName', 
            'phone': '$person.phone', 
            'email': '$person.email', 
            'userName': {
                '$ifNull': [
                    '$users.username', ''
                ]
            }, 
            'activeEmployee': {
                '$ifNull': [
                    '$users.activated', ''
                ]
            }, 
            'lastTestKitNumber': '$lastTest.testKitNumber', 
            'lastCollectionDate': {
                '$toDate': '$lastTest.collectionDate'
            }, 
            'lastReportedDate': '$lastResult.reportedDate', 
            'lastTestResult': '$lastResult.final_result', 
            'previousTestKitNumber': '$previousTest.testKitNumber', 
            'previousCollectionDate': {
                '$toDate': '$previousTest.collectionDate'
            }, 
            'previousReportedDate': '$previousTest.reportedDate', 
            'previousTestResult': '$previousResult.final_result'
        }
    }, {
        '$addFields': {
            'daysBetweenTests': {
                '$divide': [
                    {
                        '$subtract': [
                            '$lastCollectionDate', '$previousCollectionDate'
                        ]
                    }, milliseconds_in_a_day
                ]
            }, 
            'daysSinceLastTest': {
                '$floor': {
                    '$divide': [
                        {
                            '$subtract': [
                                datetime.today(), '$lastCollectionDate'
                            ]
                        }, milliseconds_in_a_day
                    ]
                }
            }
        }
    }, {
        '$project': {
            '_id': 0
        }
    }, {
        '$sort': {
            'lastReportedDate': -1, 
            'lastTestResult': 1, 
            'email': 1
        }
    }
])
# Convert mongo data into pandas df
result_list = list(result)
df = pd.DataFrame(data = result_list)

# Column-cleaning functions
def string_uppercase(s):
    ''' Return the upper case of a string '''
    return str(s).upper()

def latin_ascii(s):
    ''' Return the unicode version of a string '''
    return unicodedata.normalize('NFKD', str(s))

def text_clean(s):
    ''' Replace certain special characters with a blank '''
    return str(s).replace('[', '').replace("'", "").replace(']', '')

# Apply cleaning to first name, last name, email, phone
df["firstName"] = df["firstName"].apply(string_uppercase).apply(latin_ascii).apply(text_clean)
df["lastName"] = df["lastName"].apply(string_uppercase).apply(latin_ascii).apply(text_clean)
df["email"] = df["email"].apply(string_uppercase).apply(latin_ascii).apply(text_clean)
df["phone"] = df["phone"].apply(text_clean)


# Filter out non-current employees
df = df[df['activeEmployee'] != 'false']
print(df.info())
print(df)


# Output all dataframes to a csv file
# Create a Pandas Excel writer using XlsxWriter as the engine
writer = pd.ExcelWriter('../Data/VG Staff Covid-19 Tests - {}-{}-{} - {}AM.xlsx'.format(year, month, day, hour), engine='xlsxwriter')

# Write each dataframe to its own sheet
df.to_excel(writer, sheet_name='TestKits')

#close the Pandas Excel writer and output the Excel file
writer.save()


print(df)