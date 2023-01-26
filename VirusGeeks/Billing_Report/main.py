
import os
import sys
import math
import datetime as dt
import re
import redshift_connector
from dotenv import load_dotenv
import pandas as pd
import numpy as np
from pymongo import MongoClient
import pytz
# from cryptography.fernet import Fernet
# import pyAesCrypt
# import hasblib
# import random
# import string


# Load env file
load_dotenv()



# Create connection to Mongo
MONGO_HOST = os.getenv('MONGO_HOST')
client = MongoClient(MONGO_HOST)

def get_start_end_dates():
    year_code, month_code, day_code = 0, 1, 2

    today = dt.date.today()
    today_str = str(today).split('-')
    
    current_month = int(today_str[month_code])
    current_day = int(today_str[day_code])
    current_year = int(today_str[year_code])

    if current_day > 5:
        s_date, e_date = f"{current_year}-{current_month}-01", f"{current_year}-{current_month}-01"
    elif current_day <= 5:
        previous_month = str(dt.date.today() - dt.timedelta(14)).split('-')[month_code]
        previous_year = str(dt.date.today() - dt.timedelta(14)).split('-')[year_code]
        s_date, e_date = f"{current_year}-{previous_month}-01", f"{current_year}-{current_month}-01"

    return s_date, e_date

# date to start the mongo query from so we don't query all of testkits unneccessarily
start_date, end_date = get_start_end_dates()

start_date = '2021-07-01'
end_date = '2022-10-01'
#reformat start_date to be mongo readable datetime

start_date = start_date.split("-")
start_date = f"{start_date[0]}{start_date[1]}{start_date[2]}"

# Create pandas dataframe of testkits joined with customers and testresults and labs
result = client['VGP']['testkits'].aggregate([
    {
        '$match': {
            'collectionDate': {
                '$gte': f'{start_date}'
            }
        }
    }, {
        '$lookup': {
            'from': 'customers', 
            'localField': 'customer_id', 
            'foreignField': '_id', 
            'as': 'customer'
        }
    }, {
        '$unwind': {
            'path': '$customer'
        }
    }, {
        '$unwind': {
            'path': '$testResults'
        }
    }, {
        '$lookup': {
            'from': 'testresults', 
            'localField': 'testResults', 
            'foreignField': '_id', 
            'as': 'testresults'
        }
    }, {
        '$unwind': {
            'path': '$testresults'
        }
    }, {
        '$lookup': {
            'from': 'labs', 
            'localField': 'labID', 
            'foreignField': 'labID', 
            'as': 'labs'
        }
    }, {
        '$unwind': {
            'path': '$labs'
        }
    }, {
        '$project': {
            'status': '$status', 
            'customer_name': '$customer.company_name', 
            'customer_send_to_billing': '$customer.billing.sendToBilling', 
            'customer_bill_groups': '$customer.billing.billGroups', 
            'paymentCode': 1, 
            'sample_status': '$testKitStatus', 
            'resultReceivedDate': '$testresults.resultReceivedDate', 
            'resultReceivedDate': 1,
            'labReceivedSampleDate': 1, 
            'CLIA': '$labs.cliaNumber', 
            'lab_name': '$labs.shortName', 
            'collectionDate': 1, 
            'test_result': '$testresults.final_result'
        }
    }
])
result_list = list(result)
df_mongo = pd.DataFrame(data = result_list)
df_mongo['tk_id'] = df_mongo["_id"].apply(lambda x: str(x))
# print(df_mongo.head()); print(df_mongo.info())
# print(df_mongo['tk_id'].value_counts().reset_index())

vals = df_mongo['tk_id'].value_counts().reset_index()
vals = vals[vals['tk_id'] == 4]
# print(vals)
# for row in vals.rows:
#     print(row)

# A test with multiple results
# print(df_mongo[df_mongo['tk_id'] == '6372afee12e21f00232566c3'])

# Set env variables to globals
AWS_RS_HOST = os.getenv("AWS_RS_HOST")
AWS_DEFAULT_REGION = os.getenv("AWS_DEFAULT_REGION")
AWS_RS_DB = os.getenv("AWS_RS_DB")
AWS_RS_USER = os.getenv("AWS_RS_USER")
AWS_RS_PW = os.getenv("AWS_RS_PW")


def redshift_connection():
    conn = redshift_connector.connect(
        host=AWS_RS_HOST,
        port=5439,
        database=AWS_RS_DB,
        user=AWS_RS_USER,
        password=AWS_RS_PW
    )

    cursor = conn.cursor()
    return cursor


cursor = redshift_connection()

query1 = f""" SELECT distinct p2.first_name,
	                p2.last_name,
	                p2.dob,
                    p2.sex,
                    p2.phone,
                    p2.email,
                    p2.address1,
                    p2.address2,
                    p2.city,
                    p2.state,
                    p2.zipcode,
                    c.company_name,
                    ts.testsite_shortname,
                    p3.short_name product,
                    p3.diagnostic_type,
                    tks.insurance_company,
                    tks.insurance_member_id,
                    tks.insurance_policy_holder,
                    case 	when tks.insurance_company like '%HRSA%'
                                or tks.insurance_company is null
                                or tks.insurance_company = 'N/A' 
                                or tks.insurance_company = 'n/a' 
                                or tks.insurance_company = 'NONE'
                            then 'uninsured' 
                            else 'insured' end insurance_status,
                    cast(tks.collection_date as date) collection_date,
                    tks.testkit_number,
                    tab.shouldBill,
                    tab.wasBilled,
                    tab.wasBilledLaboratory,
            --		 case 	when tr.test_result in ('NEGATIVE', 'NOT DETECTED', 'DETECTED', 'POSITIVE', 'INCONCLUSIVE')
            --		 		then 1
            --				else 0 end matrix_billable,
                    case 	when cc.invoicing = true
                            and cast(tks.collection_date as date) > cc.contract_date
                            then true
                            else false end invoicing,
                    case 	when cc.exception_sites = true
                            and cast(tks.collection_date as date) > cc.contract_date
                            then true
                            else false end exception_site,
                    isnull(cc.agreed_amount, 0) invoice_agreed_amount,
            --		 tr.sample_status,
                    tab.vg_billing_key,
                    tab.matrix_billing_key,
                    tks.testkit_key,
            --		 vg.vg_charges, vg.vg_balance, vg.vg_payment_received, case when isnull(vg.vg_charges,0) > 0 then 1 else 0 end vg_active, vg.firstname vg_first_name, vg.lastname vg_lastname, vg.dob vg_dob, vg.dos vg_dos, 
            --		 mx.mx_charges, mx.mx_balance, mx.mx_payment_received, case when isnull(mx.mx_charges,0) > 0 then 1 else 0 end mx_active, mx.firstname mx_first_name, mx.lastname mx_lastname, mx.dob mx_dob, mx.dos mx_dos,
                    tks.status testkit_status,
                    dd.month_start,
                    case 
                        when tab.shouldbill = false or tks.status = 1 then 'v_unbillable_tests'
                        when datediff(day, cast(tks.collection_date as date), getdate()) <= 4 then 'v_developing_tests'
                        when
                        (cc.invoicing = true and cast(tks.collection_date as date) > cc.contract_date)
                            and
                            (tks.insurance_company like '%HRSA%'
                                or tks.insurance_company is null
                                or tks.insurance_company = 'N/A' 
                                or tks.insurance_company = 'n/a' 
                                or tks.insurance_company = 'NONE') then 'v_invoiced_tests'
                        else 'v_billable_tests' end as destination,
                    tks.customer_key,
                    tks.testsite_key,
                    tks.product_key,
                    tks.participant_key
		 
from 		vgp.testkits			tks
join 		vgp.dim_date 			dd  on dd.full_date = cast(tks.collection_date as date)
join 		vgp.participant_groups	pg 	on tks.participant_key = pg.participant_key
join 		vgp.people 				p2 	on tks.participant_key = p2.participant_key
join 		vgp.customers 			c 	on tks.customer_key = c.customer_key
join 		vgp.testsites 			ts	on tks.testsite_key = ts.testsite_key  
join 		vgp.products 			p3 	on tks.product_key = p3.product_key
--join 		vgp.testresults			tr 	on tks.result_key = tr.result_key
left join 	vgp.customer_contracts 	cc	on tks.customer_key = cc.customer_key
left join 	vgp.testkit_amd_billing	tab on tks.testkit_key = tab.testkit_key
--left join 	cte_vg 					vg 	on tab.vg_billing_key = vg.vg_appointment_uid
--left join 	cte_matrix 				mx 	on tab.matrix_billing_key  = mx.mx_appointment_uid
where month_start = '{start_date}' """

cursor.execute(query1)
testkit_billing_summary: pd.DataFrame = cursor.fetch_dataframe()
# print(testkit_billing_summary[testkit_billing_summary['testkit_key'] == '636bcec85419180023f2b755'])

def standardize_result_date(row):
    """
        Convert resultReceivedDate from a string to a PST mongo readable datetime
    """
    # resultRecievedDate
    if type(row) == float:
        return np.nan
    else:
        date = row.split(" ")
        month = date[1]; day = date[2]; year = date[3]; time = date[4].split(":"); hour = time[0]; minute = time[1]; second = time[2];
        # print(month); print(day); print(year); print(hour); print(minute); print(second)
        return dt.datetime.strptime(f'{year}-{month}-{day} {hour}:{minute}:{second}', '%Y-%b-%d %H:%M:%S')


def standardize_received_date(row):
    """
        Convert labReceivedSampleDate from a string to a PST mongo readable datetime
    """
    # labReceivedSampleDate
    # print(row.labReceivedSampleDate)
    if type(row) == float:
        return np.nan
    else:
        year = row[0:4]; month = row[4:6]; day = row[6:8]; hour = row[8:10]; minute = row[10:]; second = "00"
        # print(year); print(month); print(day); print(hour); print(minute); print(second)
        return dt.datetime.strptime(f'{year}-{month}-{day} {hour}:{minute}:{second}', '%Y-%m-%d %H:%M:%S')


def convert_UTC_to_PST(row):
    """
        Converts a datetime field from UTC timezone to PST for billing claims purposes
    """
    row = row.tz_localize(tz='UTC')
    row = row.tz_convert('US/Pacific')
    return row.tz_localize(tz=None)
    

df_mongo['resultReceivedDate'] = df_mongo['resultReceivedDate'].apply(lambda row: standardize_result_date(row))
df_mongo['resultReceivedDate_PST'] = df_mongo['resultReceivedDate'].apply(lambda row: convert_UTC_to_PST(row))

df_mongo['labReceivedSampleDate'] = df_mongo['labReceivedSampleDate'].apply(lambda row: standardize_received_date(row))
df_mongo['labReceivedSampleDate_PST'] = df_mongo['labReceivedSampleDate'].apply(lambda row: convert_UTC_to_PST(row))


def check_valid_test(row):
    '''
        If customer_send_to_billing = 'no' then isValidTest = false
        If customer_send_to_billing = 'yes' and there is a paymentCode then shouldbill=false
        Unless there is a 'Refunded' in paymentCode which means we need to bill so isValidTest = true
    '''
    # Convert payment code nans to a string
    row.paymentCode = str(row.paymentCode)

    if row.shouldbill == True:
        if row.customer_send_to_billing == 'no':
            return False
        if row.customer_send_to_billing == 'yes': 
            if row.paymentCode is None or row.paymentCode == '' or row.paymentCode == 'nan':
                return True
            # the length of the payment code is greater than 0 even though it doesn't appear to have anything
            elif len(row.paymentCode) > 0 and row.paymentCode != 'nan':
                # print(type(row.paymentCode)); print(row.paymentCode); print(len(row.paymentCode))
                if row.paymentCode == 'Refunded':
                    return True
                else:
                    return False
    elif row.shouldbill == False:
        return False
    else:
        return None

def find_reason_for_unbilled(row):
    '''
        Creates a new column giving an explanation on the shouldbill=false status

        check for payment code which isn't refunded
        check billing groups to see if it is in or out of valid billing group
        check teskit_status = 0

        anything outside of this needs to be investigated
    '''
    row.paymentCode = str(row.paymentCode)
    billing_groups = get_billing_groups();

    if len(row.paymentCode) > 0 and row.paymentCode != 'nan' and row.paymentCode != 'Refunded':
        # Then there is a valid payment code and we should not bill
        return 'Valid Payment Code'
    elif row.testkit_status == 1:
        # Then it may be a valid test
        return 'Status = 1'
    elif row.customer_send_to_billing == 'by-groups':
        # Then check whether this test's participant group is in the customer's billing group
        if row.customer in billing_groups['company_name']:
            if row.participant_group not in billing_groups['groups']:
                return 'Invalid Billing Group'
    elif row.customer_send_to_billing == 'no':
        # Then it is not meant to be billed
        return 'Not Billable'
    elif row.sample_status in ['Damaged', 'Misregistered', 'Unregistered', 'Canceled', 'Lost']:
        return row.sample_status
    else:
        return 'Investigate'
    
def get_billable_tests(start_date):
    '''
        Query the billable tests from staging.v_billable_tests in redshift and return in pandas df
    '''
    # cursor = redshift_connection()

    # query1 = f"select * from staging.v_billable_tests vbt \
    #         where vbt.month_start = '{start_date}' "
    # cursor.execute(query1)
    # billable_tests: pd.DataFrame = cursor.fetch_dataframe()
    billable_tests = testkit_billing_summary[testkit_billing_summary['destination'] == 'v_billable_tests']

    col_names = {'first_name': 'firstName',
                 'last_name': 'lastName',
                 'dob': 'dob',
                 'sex': 'gender',
                 'phone': 'phone',
                 'email': 'email',
                 'address1': 'address1',
                 'address2': 'address2',
                 'city': 'city',
                 'state': 'state',
                 'zipcode': 'ZipCode',
                 'company_name': 'customer',
                 'testsite_shortname': 'testsite',
                 'product': 'product',
                 'diagnostic_type': 'diagnosticType',
                 'insurance_company': 'insuranceName',
                 'insurance_member_id': 'memberID',
                 'insurance_policy_holder': 'policyHolder',
                 'collection_date': 'collection_date',
                 'shouldbill': 'shouldbill',
                 'wasbilled': 'wasbilled',
                 'wasbilledlaboratory': 'wasbilledlaboratory',
                #  'matrix_billable': 'matrix_billable',
                 'sample_status': 'sample_status',
                 'vg_billing_key': 'vg_billing_key',
                 'matrix_billing_key': 'matrix_billing_key',
                 'testkit_key': 'testkit_id',
                #  'vg_charges': 'vg_charges',
                #  'vg_balance': 'vg_balance',
                #  'vg_payment_received': 'vg_payment_received',
                #  'vg_active': 'vg_active',
                #  'vg_first_name': 'vg_first_name',
                #  'vg_lastname': 'vg_lastname',
                #  'vg_dob': 'vg_dob',
                #  'vg_dos': 'vg_dos',
                #  'vg_last_update': 'vg_last_update',
                #  'mx_charges': 'mx_charges',
                #  'mx_balance': 'mx_balance',
                #  'mx_payment_received': 'mx_payment_received',
                #  'mx_active': 'mx_active',
                #  'mx_first_name': 'mx_first_name',
                #  'mx_lastname': 'mx_lastname',
                #  'mx_dob': 'mx_dob',
                #  'mx_dos': 'mx_dos',
                #  'mx_last_update': 'mx_last_update',
                 'month_start': 'month_start', }

    # Rename columns
    billable_tests.rename(columns=col_names, inplace=True)

    # Join redshift data with mongo data so that we can add a valid test boolean field
    billable_tests = billable_tests.merge(df_mongo, how='left', left_on='testkit_id', right_on='tk_id')

    # print(billable_tests.info()); print(billable_tests)
    # print(billable_tests['tk_id'].value_counts())
    

    # Create a new column which adds extra parameters to shouldbill to check if we have a valid test
    billable_tests['isValidTest'] = billable_tests.apply(lambda row: check_valid_test(row), axis=1)

    billable_tests = billable_tests.drop(
            labels=[
                # 'vg_payment_received',
                # 'mx_payment_received',
                # 'mx_balance',
                # 'vg_balance',
                # 'matrix_billable',
                # 'sample_status',
                'destination'
            ],
            axis=1,
        )

    # Create scrubbed version of the data for sharepoint upload
    billable_tests_scrubbed = billable_tests.drop(
        labels=[
            'firstName',
            'lastName',
            'dob',
            'gender',
            'phone',
            'email',
            'address1',
            'address2',
            'city', 
            'state',
            'ZipCode',
            'memberID',
            'policyHolder',
            'vg_billing_key',
            'matrix_billing_key',
            # 'vg_charges',
            # 'vg_active',
            # 'vg_first_name',
            # 'vg_lastname',
            # 'vg_dob',
            # 'vg_dos',
            # 'vg_last_update',
            # 'mx_charges',
            # 'mx_active',
            # 'mx_first_name',
            # 'mx_lastname',
            # 'mx_dob',
            # 'mx_dos',
            # 'mx_last_update',
        ],
        axis=1,
    )

    return billable_tests, billable_tests_scrubbed

def get_unbillable_tests(start_date):
    '''
        Query the unbillable tests from staging.v_unbillable_tests in redshift and return in pandas df
    '''
    # cursor = redshift_connection()

    # query2 = f" select  vut.*, \
    #                     tk.status, \
    #                     pg.participant_group, \
    #                     c.send_to_billing \
    #             from staging.v_unbillable_tests vut \
    #             join vgp.testkits tk on tk.testkit_key = vut.testkit_key \
    #             join vgp.participant_groups pg on tk.participant_key = pg.participant_key \
    #             join vgp.customers c on c.customer_key = pg.customer_key \
    #             where vut.month_start = '{start_date}' "
    # cursor.execute(query2)
    # unbillable_tests: pd.DataFrame = cursor.fetch_dataframe()
    unbillable_tests = testkit_billing_summary[testkit_billing_summary['destination'] == 'v_unbillable_tests']

    # rename columns
    col_names = {'first_name': 'firstName',
                 'last_name': 'lastName',
                 'dob': 'dob',
                 'sex': 'gender',
                 'phone': 'phone',
                 'email': 'email',
                 'address1': 'address1',
                 'address2': 'address2',
                 'city': 'city',
                 'state': 'state',
                 'zipcode': 'ZipCode',
                 'company_name': 'customer',
                 'testsite_shortname': 'testsite',
                 'product': 'product',
                 'diagnostic_type': 'diagnosticType',
                 'insurance_company': 'insuranceName',
                 'insurance_member_id': 'memberID',
                 'insurance_policy_holder': 'policyHolder',
                 'collection_date': 'collection_date',
                 'shouldbill': 'shouldbill',
                 'wasbilled': 'wasbilled',
                 'wasbilledlaboratory': 'wasbilledlaboratory',
                #  'matrix_billable': 'matrix_billable',
                 'vg_billing_key': 'vg_billing_key',
                 'matrix_billing_key': 'matrix_billing_key',
                 'testkit_key': 'testkit_id',
                #  'vg_charges': 'vg_charges',
                #  'vg_balance': 'vg_balance',
                #  'vg_payment_received': 'vg_payment_received',
                #  'vg_active': 'vg_active',
                #  'vg_first_name': 'vg_first_name',
                #  'vg_lastname': 'vg_lastname',
                #  'vg_dob': 'vg_dob',
                #  'vg_dos': 'vg_dos',
                #  'vg_last_update': 'vg_last_update',
                #  'mx_charges': 'mx_charges',
                #  'mx_balance': 'mx_balance',
                #  'mx_payment_received': 'mx_payment_received',
                #  'mx_active': 'mx_active',
                #  'mx_first_name': 'mx_first_name',
                #  'mx_lastname': 'mx_lastname',
                #  'mx_dob': 'mx_dob',
                #  'mx_dos': 'mx_dos',
                #  'mx_last_update': 'mx_last_update',
                 'month_start': 'month_start',
    }
    unbillable_tests.rename(columns=col_names, inplace=True)

    # Join redshift data with mongo data so that we can add a valid test boolean field
    unbillable_tests = unbillable_tests.merge(df_mongo, how='left', left_on='testkit_id', right_on='tk_id')

    unbillable_tests = unbillable_tests.drop(
        labels=[
            # 'vg_payment_received',
            # 'mx_payment_received',
            # 'mx_balance',
            # 'vg_balance',
            # 'matrix_billable',
            # 'sample_status',
            'destination',
        ],
        axis=1,
    )

    # Add field for unbilled reason
    unbillable_tests['reason'] = unbillable_tests.apply(lambda row: find_reason_for_unbilled(row), axis=1)

    # Create scrubbed version of the data for sharepoint upload
    unbillable_tests_scrubbed = unbillable_tests.drop(
        labels=[
            'firstName',
            'lastName',
            'dob',
            'gender',
            'phone',
            'email',
            'address1',
            'address2',
            'city', 
            'state',
            'ZipCode',
            'memberID',
            'policyHolder',
            'vg_billing_key',
            'matrix_billing_key',
            # 'vg_charges',
            # 'vg_active',
            # 'vg_first_name',
            # 'vg_lastname',
            # 'vg_dob',
            # 'vg_dos',
            # 'vg_last_update',
            # 'mx_charges',
            # 'mx_active',
            # 'mx_first_name',
            # 'mx_lastname',
            # 'mx_dob',
            # 'mx_dos',
            # 'mx_last_update',
        ],
        axis=1,
    )
    regex_pattern = re.compile(r"canceled", flags=re.I)
    list_canceled_tests = []

    def check_for_canceled(row):
        canceled = regex_pattern.findall(row)
        if len(canceled) > 0:
            list_canceled_tests.append(row)

    unbillable_tests["testkit_number"].apply(lambda row: check_for_canceled(row))

    canceled_tests = unbillable_tests[unbillable_tests['testkit_number'].isin(list_canceled_tests)]

    # canceled_tests = unbillable_tests_scrubbed[unbillable_tests['testkit_number'] == 'Canceled']

    return unbillable_tests, unbillable_tests_scrubbed, canceled_tests

def get_invoiced_tests(start_date):
    """
        Query the invoiced tests from staging.v_invoiced_tests in redshift and return in pandas df
    """
    # cursor = redshift_connection()

    # query3 = f" select  * \
    #             from staging.v_invoiced_tests vit \
    #             where vit.month_start = '{start_date}' "
    # cursor.execute(query3)
    # invoiced_tests: pd.DataFrame = cursor.fetch_dataframe()
    invoiced_tests = testkit_billing_summary[testkit_billing_summary['destination'] == 'v_invoiced_tests']

    # rename columns
    col_names = {'first_name': 'firstName',
                 'last_name': 'lastName',
                 'dob': 'dob',
                 'sex': 'gender',
                 'phone': 'phone',
                 'email': 'email',
                 'address1': 'address1',
                 'address2': 'address2',
                 'city': 'city',
                 'state': 'state',
                 'zipcode': 'ZipCode',
                 'company_name': 'customer',
                 'testsite_shortname': 'testsite',
                 'product': 'product',
                 'diagnostic_type': 'diagnosticType',
                 'insurance_company': 'insuranceName',
                 'insurance_member_id': 'memberID',
                 'insurance_policy_holder': 'policyHolder',
                 'insurance_status': 'insuranceStatus',
                 'collection_date': 'collection_date',
                 'testkit_number': 'testkitNumber',
                 'shouldbill': 'shouldbill',
                 'wasbilled': 'wasbilled',
                 'wasbilledlaboratory': 'wasbilledlaboratory',
                #  'matrix_billable': 'matrix_billable',
                 'invoicing': 'invoicing',
                 'invoice_agreed_amount': 'invoiceAgreedAmount',
                 'vg_billing_key': 'vg_billing_key',
                 'matrix_billing_key': 'matrix_billing_key',
                 'testkit_key': 'testkit_id',
                #  'vg_charges': 'vg_charges',
                #  'vg_balance': 'vg_balance',
                #  'vg_payment_received': 'vg_payment_received',
                #  'vg_active': 'vg_active',
                #  'vg_first_name': 'vg_first_name',
                #  'vg_lastname': 'vg_lastname',
                #  'vg_dob': 'vg_dob',
                #  'vg_dos': 'vg_dos',
                #  'mx_charges': 'mx_charges',
                #  'mx_balance': 'mx_balance',
                #  'mx_payment_received': 'mx_payment_received',
                #  'mx_active': 'mx_active',
                #  'mx_first_name': 'mx_first_name',
                #  'mx_lastname': 'mx_lastname',
                #  'mx_dob': 'mx_dob',
                #  'mx_dos': 'mx_dos',
                 'month_start': 'month_start',
    }
    invoiced_tests.rename(columns=col_names, inplace=True)

    invoiced_tests = invoiced_tests.drop(
        labels=[
            # 'vg_payment_received',
            # 'mx_payment_received',
            # 'mx_balance',
            # 'vg_balance',
            # 'matrix_billable',
            'destination',
        ],
        axis=1,
    )

    # Create scrubbed version of the data for sharepoint upload
    invoiced_tests_scrubbed = invoiced_tests.drop(
        labels=[
            'firstName',
            'lastName',
            'dob',
            'gender',
            'phone',
            'email',
            'address1',
            'address2',
            'city', 
            'state',
            'ZipCode',
            'memberID',
            'policyHolder',
            'vg_billing_key',
            'matrix_billing_key',
            # 'vg_charges',
            # 'vg_active',
            # 'vg_first_name',
            # 'vg_lastname',
            # 'vg_dob',
            # 'vg_dos',
            # 'vg_last_update',
            # 'mx_charges',
            # 'mx_active',
            # 'mx_first_name',
            # 'mx_lastname',
            # 'mx_dob',
            # 'mx_dos',
            # 'mx_last_update',
        ],
        axis=1,
    )

    return invoiced_tests, invoiced_tests_scrubbed

def get_billing_groups():
    # Create pandas dataframe of customers for the billing groups
    result = client['VGP']['customers'].aggregate([
        {
            '$unwind': {
                'path': '$groups'
            }
        }, {
            '$project': {
                'company_name': 1, 
                'sendToBilling': '$billing.sendToBilling', 
                'invoice': '$billing.invoice', 
                'groups': 1, 
                'active': 1
            }
        }
    ])
    result_list = list(result)
    df_billing_groups = pd.DataFrame(data = result_list)

    return df_billing_groups

def get_redshift(start_date):
    '''
        Query from both billable tests and unbillable tests in the month start_date
        return both pandas dataframes as a tuple
    '''
    billable_tests, billable_tests_scrubbed = get_billable_tests(start_date)
    unbillable_tests, unbillable_tests_scrubbed, canceled_tests = get_unbillable_tests(start_date)
    invoiced_tests, invoiced_tests_scrubbed = get_invoiced_tests(start_date)

    all_tests = billable_tests, billable_tests_scrubbed, unbillable_tests, unbillable_tests_scrubbed, canceled_tests, invoiced_tests, invoiced_tests_scrubbed

    return all_tests


if __name__ == '__main__':
    # Get date range to run report between
    start_date, end_date = get_start_end_dates()
    # start_date = '2021-07-01'
    # end_date = '2022-10-01'
    # Get a list of months
    cursor = redshift_connection()
    month_qry = f"select distinct month_start \
                from staging.v_billable_tests vbt  \
                where vbt.month_start between '{start_date}' and '{end_date}'  \
                order by 1 desc"
    cursor.execute(month_qry)
    months = cursor.fetchall()

    # Iterate through each month
    for month in months:
        print("\nMONTH"); print(month)
        current_month = month[0].isoformat()
        current_month_formatted = current_month.replace('-', '')
        
        billable_tests, billable_tests_scrubbed, unbillable_tests, unbillable_tests_scrubbed, canceled_tests, invoiced_tests, invoiced_tests_scrubbed = get_redshift(current_month)
        
        # Output all dataframes to a csv file
        # Create a Pandas Excel writer using XlsxWriter as the engine
        writer = pd.ExcelWriter(f'output/billingData_{current_month_formatted}.xlsx', engine='xlsxwriter')

        # Write each dataframe to its own sheet
        billable_tests.to_excel(writer, sheet_name='Billable')
        unbillable_tests.to_excel(writer, sheet_name='UnBillable')
        canceled_tests.to_excel(writer, sheet_name='Canceled')
        invoiced_tests.to_excel(writer, sheet_name='Invoiced')

        #close the Pandas Excel writer and output the Excel file
        writer.save()

        # Output all dataframes to a csv file
        # Create a Pandas Excel writer using XlsxWriter as the engine
        writer = pd.ExcelWriter(f'output/billingData_scrubbed_{current_month_formatted}.xlsx', engine='xlsxwriter')

        # Write each dataframe to its own sheet
        billable_tests_scrubbed.to_excel(writer, sheet_name='Billable Scrubbed')
        unbillable_tests_scrubbed.to_excel(writer, sheet_name='Unbillable Scrubbed')
        canceled_tests.to_excel(writer, sheet_name='Canceled')
        invoiced_tests_scrubbed.to_excel(writer, sheet_name='Invoiced Scrubbed')

        #close the Pandas Excel writer and output the Excel file
        writer.save()

        print("There are {} billable tests in {}".format(len(billable_tests), month))
        print("There are {} unbillable tests in {}".format(len(unbillable_tests), month))
        print("There are {} invoiced tests in {}".format(len(invoiced_tests), month))
        print("There are {} canceled tests in {}".format(len(canceled_tests), month))
        print("The most recent collection date was {}".format(billable_tests['collection_date'].max()))

        # print("On {} there were {} billable tests, {} unbillable tests, {} invoiced tests for a total of {} tests".format( billable_tests ))

    print("\nEnd Process")