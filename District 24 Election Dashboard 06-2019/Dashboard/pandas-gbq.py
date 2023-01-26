from dash.dependencies import Input, Output
from google.cloud import bigquery
from google.oauth2 import service_account
import pandas_gbq
import pandas as pd
import numpy as np
import plotly.express as px
import plotly.graph_objects as go
import dash
import dash_core_components as dcc
import dash_html_components as html

# Read the credentials using service account which reads a JSON file
credentials = service_account.Credentials.from_service_account_file('C:/Users/Peter/Desktop/PoliPod/Dash/dash-oil-and-gas/GCP_M24_ServiceAccount.json')
# Then query just as you would from Google Big Query
query = """SELECT * FROM `mfor24.campaign_finance.nyc_contributions`"""
# Include the query in a DataFrame (you will need the project ID from GBQ)
df = pandas_gbq.read_gbq(query, project_id="mfor24", credentials = credentials)

print(df.head())
