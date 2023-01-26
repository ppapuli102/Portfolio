"""
    Daily Report for billing team which details AMD Billing data such as vg_balance, mx_balance, null_tests, etc.

    Data is taken from VGP.participants_full unwinded on TestKits
    The Collection date range starts from 20210311 and ends in the most recent collection day

    Output includes two dataframes:
    1) df_tests_by_week: The number of testkits grouped across the week start
    2) df_tests_average_weekly: The aggregate of average tests across all weeks

    Data is output into a .xlsx file and uploaded into VGP's sharepoint Documents > Data Analytics > adhoc > Customer Reports > xxxxxxxxxxxxxx
    

    Author: Peter Papuli
    Last Modified Date: 7/11/2022
    Last Modified By: Peter Papuli
"""

from dotenv import load_dotenv
import os
from datetime import date, timedelta, datetime
import numpy as np
import pandas as pd
import redshift_connector 
from pymongo import MongoClient


# Load our .env file which holds all credentials
load_dotenv()

# Create connection to Mongo
MONGO_HOST = os.getenv('MONGO_HOST')
client = MongoClient(MONGO_HOST)

# Create pandas dataframe of customers
testkits = client['VGP']['testkits'].aggregate([
    {
        '$match': {
            'paymentCode': {
                '$exists': True
            }
        }
    }, {
        '$project': {
            'paymentCode': 1,
        }
    }
])
testkits_list = list(testkits)
df_mongo = pd.DataFrame(data = testkits_list)
df_mongo['testkit_key'] = df_mongo['_id'].apply(lambda x: str(x))

# print(df_mongo[df_mongo["paymentCode"] != ''])

# Redshift connection is detailed in a .env file for security
REDSHIFT_HOST = os.getenv('REDSHIFT_HOST')
REDSHIFT_DATABASE = os.getenv('REDSHIFT_DATABASE')
REDSHIFT_USER = os.getenv('REDSHIFT_USER')
REDSHIFT_PASSWORD = os.getenv('REDSHIFT_PASSWORD')

# Create connection to RedShift
conn = redshift_connector.connect(
     host=REDSHIFT_HOST,
     database=REDSHIFT_DATABASE,
     user=REDSHIFT_USER,
     password=REDSHIFT_PASSWORD
  )
    
cursor = conn.cursor()

# Query from RedShift
cursor.execute(
    """ SELECT *
        FROM vgp.v_daily_summary """
)
result = cursor.fetch_dataframe()
df_redshift = pd.DataFrame(data=result)

# Query the null tests from redshift
cursor = conn.cursor()
cursor.execute(
    """ SELECT tk.testkit_key, tk.result_key, tr.test_result, 
        case when tr.test_result not in ('NEGATIVE', 'NOT DETECTED', 'DETECTED', 'POSITIVE', 'INCONCLUSIVE') then 1 else 0 end as null_tests
        from vgp.testkits tk
        left join vgp.testresults tr on tk.result_key = tr.result_key """
)
result = cursor.fetch_dataframe()
df_non_null_tests = pd.DataFrame(data=result)
# print(df_non_null_tests)

# Query the null tests from redshift
cursor = conn.cursor()
cursor.execute(
    """ SELECT p.first_name, 
	   p.last_name, 
	   p.email, 
	   p.dob,
	   tk.testkit_key, 
	   tk.collection_date, 
	   tk.insurance_company, 
	   tab.wasbilled,
	   tab.wasbilledlaboratory,
	   tr.status,
	   case 
		    when datediff(year, p.dob, tk.collection_date) <= 0 then 'other'
		   	when ABS(datediff(year, p.dob, tk.collection_date)) < 18 then '<18'
	   		when (ABS(datediff(year, p.dob, tk.collection_date)) <= 20) then '18-20'
	   		when (ABS(datediff(year, p.dob, tk.collection_date)) <= 30) then '20-30'
	   		when (ABS(datediff(year, p.dob, tk.collection_date)) <= 40) then '30-40'
	   		when (ABS(datediff(year, p.dob, tk.collection_date)) <= 50) then '40-50'
	   		when (ABS(datediff(year, p.dob, tk.collection_date)) <= 60) then '50-60'
	   		when (ABS(datediff(year, p.dob, tk.collection_date)) <= 70) then '70-80'
	   		when (ABS(datediff(year, p.dob, tk.collection_date)) <= 80) then '70-80'
	   		when (ABS(datediff(year, p.dob, tk.collection_date)) <= 90) then '80-90'
	   		when ABS(datediff(year, p.dob, tk.collection_date)) > 90 then '>90'
	   		else 'other' end as age_grouprest
        from vgp.testkits tk
        left join vgp.testresults tr on tk.result_key = tr.result_key 
        left join vgp.people p on tk.person_key = p.person_key 
        left join vgp.testkit_amd_billing tab on tk.testkit_key = tab.testkit_key 
        where tr.test_result not in ('NEGATIVE', 'NOT DETECTED', 'DETECTED', 'POSITIVE', 'INCONCLUSIVE')
        order by tk.collection_date desc """
)
result = cursor.fetch_dataframe()
df_null_tests = pd.DataFrame(data=result)
print(df_null_tests.info())
df_null_tests = df_null_tests.set_index('testkit_key').join(df_mongo.set_index('testkit_key'), how='left').drop(['_id'], axis=1)
# print(df_null_tests)

# Query the daily summary from redshift
cursor = conn.cursor()
cursor.execute(
    """ SELECT collection_date,
              count(*) tests,
              sum(case when shouldbill then 1 else 0 end ) should_bill,
              sum(case when wasbilled then 1 else 0 end ) was_billed_vg,
              sum(vg_active) vg_active,
              sum(isnull(vg_balance,0)) vg_balance,
              sum(isnull(vg_payment_received,0)) vg_payment_received,
              sum(case when wasbilledlaboratory then 1 else 0 end ) was_billed_matrix,
              sum(mx_active) mx_active,
              sum(isnull(mx_balance,0)) mx_balance,
              sum(isnull(mx_payment_received,0)) mx_payment_received
          
        FROM vgp.v_daily_summary

        WHERE collection_date <= getdate()
        GROUP BY collection_date
        ORDER BY 1 DESC """
)
result = cursor.fetch_dataframe()
df_summary = pd.DataFrame(data=result)

# Query the damaged tests from redshift
cursor = conn.cursor()
cursor.execute(
    """ SELECT first_name
            , last_name
            , dob
            , sex
            , phone
            , email
            , address1
            , address2
            , city
            , state
            , zipcode
            , company_name
            , testsite_shortname
            , product
            , diagnostic_type
            , insurance_company
            , insurance_member_id
            , insurance_policy_holder
            , collection_date
            , shouldbill
            , wasbilled
            , wasbilledlaboratory
            , matrix_billable
            , sample_status
            , month_start

    from vgp.v_testkit_billing_summary t 

    where t.sample_status in ('Lost', 'Canceled', 'Mistake', 'Misregistered', 'Damaged', 'Duplicate')

    order by t.collection_date desc """
)
result = cursor.fetch_dataframe()
df_damaged = pd.DataFrame(data=result)


cte_final = df_redshift.set_index('testkit_key').join(df_mongo.set_index('testkit_key'), how='left')
cte_final = cte_final.join(df_non_null_tests.set_index('testkit_key'), how='left')

# count the number of null values along with the amount of "Refund"s in payment code
cte_final["self_pay"] = np.where(cte_final['paymentCode'].isna(), 0,
                        np.where(cte_final['paymentCode'].str.contains(r"Refund"), 0,
                        np.where(cte_final['paymentCode'] == '', 0,
                        1))) # if none of the other statements are true then 0

# print("\n\nThe Amount of Null Values in cte_final is: \n")
# print(cte_final['paymentCode'].isna().count(), "\n")
# print("\n\nThe Amount of NOT NULL Values in cte_final is: \n")
# print(cte_final['paymentCode'].notna().count(), "\n")
# print(cte_final.agg({"self_pay" : ["sum"]}))

# count the number of null tests by day
cte_final_merge = cte_final.groupby("collection_date").agg({"null_tests" : ["sum"], "self_pay" : ["sum"]}).reset_index()

df_summary = df_summary.join(cte_final_merge.set_index('collection_date'), on='collection_date', how='left')

# Change column names to readable
df_summary.rename({ "collection_date": "Collection Date", 
                    "tests": "Number of Tests", 
                    "should_bill": "Should Bill", 
                    "was_billed_vg": "Was Billed VG", 
                    "vg_active": "VG Active", 
                    "vg_balance": "VG Balance", 
                    "vg_payment_received": "VG Payment Received", 
                    "was_billed_matrix": "Was Billed Matrix", 
                    "mx_active": "Matrix Active", 
                    "mx_balance": "Matrix Balance", 
                    "mx_payment_received": "Matrix Payment Received", 
                    ("null_tests", "sum"): "Number of Damaged/Lost Tests",
                    ("self_pay", "sum"): "Number of Self-Paid Tests"},
                    axis=1,
                    inplace=True
)

# Remove payment information
df_summary = df_summary.drop(
    labels= [
        'VG Balance',
        'VG Payment Received',
        'Matrix Balance',
        'Matrix Payment Received',
    ],
    axis=1,
)

print(df_summary)

# Output all dataframes to a csv file
# Create a Pandas Excel writer using XlsxWriter as the engine
writer = pd.ExcelWriter('../Data/billing_summary.xlsx', engine='xlsxwriter')

# Write each dataframe to its own sheet
df_summary.to_excel(writer, sheet_name='Billing_Summary')
df_null_tests.to_excel(writer, sheet_name='Null_Tests')
df_damaged.to_excel(writer, sheet_name='Damaged_Tests')

#close the Pandas Excel writer and output the Excel file
writer.save()


