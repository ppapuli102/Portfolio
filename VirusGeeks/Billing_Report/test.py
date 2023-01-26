import datetime as dt
from dotenv import load_dotenv
from pymongo import MongoClient
import pandas as pd
import os
import re

load_dotenv()

# Create connection to Mongo
MONGO_HOST = os.getenv('MONGO_HOST')
client = MongoClient(MONGO_HOST)

# Create pandas dataframe of testkits
result = client['VGP']['testkits'].aggregate([
    {
        '$match': {
            'collectionDate': {
                '$gte': '20221110',
                '$lt': '20221114'
            }
        }
    }, {
        '$project': {
            'collectionDate': 1, 
            'testKitNumber': 1, 
            'shouldBill': 1, 
            'billing_status': '$billing.primaryStatus', 
        }
    }
])
result_list = list(result)
df = pd.DataFrame(data = result_list)

print(len(df)); print(len(df[df['shouldBill'] == 'TRUE']))

# # Create pandas dataframe of testkits
# result = client['VGP']['testkits'].aggregate([
#     {
#         '$match': {
#             'collectionDate': {
#                 '$gte': '20220801'
#             }
#         }
#     }, {
#         '$lookup': {
#             'from': 'testresults', 
#             'localField': 'result_id', 
#             'foreignField': '_id', 
#             'as': 'testresults'
#         }
#     }, {
#         '$unwind': {
#             'path': '$testresults'
#         }
#     }, {
#         '$lookup': {
#             'from': 'testsites', 
#             'localField': 'testsite_id', 
#             'foreignField': '_id', 
#             'as': 'testsite'
#         }
#     }, {
#         '$unwind': {
#             'path': '$testsite'
#         }
#     }, {
#         '$project': {
#             'collectionDate': 1, 
#             'testKitNumber': 1, 
#             'sampleType': 1, 
#             'status': 1, 
#             'testsite_id': 1, 
#             'insurance_name': '$insurance.insuranceName', 
#             'shouldBill': 1, 
#             'wasBilled': 1, 
#             'wasBilledLaboratory': 1, 
#             'billing_status': '$billing.primaryStatus', 
#             'result_status': '$testresults.tr_status', 
#             'testsite_name': '$testsite.name'
#         }
#     }
# ])
# result_list = list(result)
# df = pd.DataFrame(data = result_list)
# # print("mongo: \n"); print(df.head()); print(df.info())

# regex_pattern = re.compile(r"canceled", flags=re.I)
# canceled_tests = []

# def check_for_canceled(row):
#     canceled = regex_pattern.findall(row)
#     if len(canceled) > 0:
#         canceled_tests.append(row)

#     return

# df["testKitNumber"].apply(lambda row: check_for_canceled(row))

# df_canceled_tests = df[df['testKitNumber'].isin(canceled_tests)]
# print(df_canceled_tests)

# # print(df[df['testKitNumber'] == 'VG1747678'])

# # Read removed testkit data from excel
# df_removedtks = pd.read_excel("C:/Users/VirusGeeks/Documents/GitHub/Data-Analytics/Reports/Billing_Report/removed_testkits.xlsx", sheet_name="Rec 0826, Removed Now")
# # print(df_removedtks.head()); print(df_removedtks.info())

# # Merge removed testkits into mongo testkits
# df_merged = df.merge(df_removedtks, how='inner', left_on='testKitNumber', right_on='testkit_number')
# # print(df_merged.head()); print(df_merged.info())

# print(df_merged.groupby('testsite_name').agg({'collectionDate' : ['count']}))


# today = dt.date.today()
# date_check = today - dt.timedelta(days=5)
# date_check_mdb = str(date_check).replace("-","")
# print(date_check_mdb)

# year_code, month_code, day_code = 0, 1, 2
# today = dt.date.today()
# today_str = str(today).split('-')
# current_month = int(today_str[month_code])
# current_day_int = int(today_str[day_code])

# if current_day_int >= 7:
#         s_date, e_date = str(today).replace('-', ''), str(today).replace('-', '')



# print(s_date, e_date)