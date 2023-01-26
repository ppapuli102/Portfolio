# Import required libraries
import pickle
import copy
import pathlib
import dash
import math
import datetime as dt
import pandas as pd
from dash.dependencies import Input, Output, State, ClientsideFunction
import dash_core_components as dcc
import dash_html_components as html
import plotly.graph_objects as go
from google.cloud import bigquery
from google.oauth2 import service_account
import pandas_gbq
import pygeoj
from geopy.geocoders import Nominatim
import json
import numpy as np
import plotly.express as px
import dash_table


# Load data from Google Big Query
credentials = service_account.Credentials.from_service_account_file('E:/New York Strategy/PoliPod/Dash/dash-oil-and-gas/GCP_M24_ServiceAccount.json')
project_id = "mfor24"
query = """
SELECT * FROM `mfor24.nys_foil.nys_foil_original` LIMIT 1000
"""
sm_df = pandas_gbq.read_gbq(query, project_id= project_id, credentials= credentials)

df = pd.read_csv("C:/Users/Peter/Desktop/PoliPod/Dash/dash-oil-and-gas/data/Mayoral Contributions.csv")

# GEO STUFF
json_df = pd.read_json("C:/Users/Peter/Desktop/PoliPod/Dash/dash-oil-and-gas/nyc_zip_code_tabulation_areas_polygons.geojson")
geodata = pygeoj.load("C:/Users/Peter/Desktop/PoliPod/Dash/dash-oil-and-gas/nyc_zip_code_tabulation_areas_polygons.geojson")

# Getting list of all NYC Zip Codes from the Geo Data
zips_list = []
for i in range(len(json_df['features'])):
    zips_list.append(json_df['features'][i]['properties']['postalcode'])

# Clean data up
def GetElectionYear(data_frame):
    election_year = []
    for election in data_frame['ELECTION']:
        election_year.append(int(election[:4]))
    data_frame['ELECTION_INT'] = election_year
    return data_frame

#df = GetElectionYear(df)

for i in range(len(df['Election District'])):
    if df['Election District'][i].isdigit():
        CCDistrict = df['Election District'][i]
        df["Election District"][i] = "City Council District " + str(CCDistrict)
    else:
        pass

# create list and dicitonary including all years
year_options = []
year_dict = {}
for year in df['ELECTION_INT'].sort_values().unique():
    year_options.append({'label': str(year), 'value': year})
    year_dict.update({str(year): str(year)})

# Clean Zip Code Column to be used with GeoJson
clean_zips = []
for i in df['ZIP']:
    i = str(i)
    if len(i) >= 5:
        if i[:5].isdigit():
            clean_zips.append(str(i[:5]))
        else:
            clean_zips.append('')
    else:
        clean_zips.append('')
df['CLEAN_ZIP'] = clean_zips

# dataframe including contributions only within zip codes we have geoJsons for
nyc_df = df[df['CLEAN_ZIP'].isin(zips_list)]

# Create global chart template
mapbox_access_token = "pk.eyJ1IjoicGxvdGx5bWFwYm94IiwiYSI6ImNrOWJqb2F4djBnMjEzbG50amg0dnJieG4ifQ.Zme1-Uzoi75IaFbieBDl3A"

layout = dict(
    autosize=True,
    automargin=True,
    margin=dict(l=30, r=30, b=20, t=40),
    hovermode="closest",
    plot_bgcolor="#F9F9F9",
    paper_bgcolor="#F9F9F9",
    legend=dict(font=dict(size=10), orientation="h"),
    title="Satellite Overview",
    mapbox=dict(
        accesstoken=mapbox_access_token,
        style="light",
        center=dict(lon=-78.05, lat=42.54),
        zoom=7,
    ),
)

# Create app layout
app.layout = html.Div(
    [
        dcc.Store(id="aggregate_data"),
        # empty Div to trigger javascript file for graph resizing
        html.Div(id="output-clientside"),
        html.Div(
            [
                html.Div(
                    [
                        html.Img(
                            src=app.get_asset_url("NYS logo.png"),
                            id="plotly-image",
                            style={
                                "height": "60px",
                                "width": "auto",
                                "margin-bottom": "25px",
                            },
                        )
                    ],
                    className="one-third column",
                ),
                html.Div(
                    [
                        html.Div(
                            [
                                html.H3(
                                    "Campaign Finance and Voter Turnout",
                                    style={"margin-bottom": "0px"},
                                ),
                                html.H5(
                                    "A Dashboard by New York Strategy", style={"margin-top": "0px"}
                                ),
                            ]
                        )
                    ],
                    className="one-half column",
                    id="title",
                ),
            ],
            id="header",
            className="row flex-display",
            style={"margin-bottom": "25px"},
        ),
        html.Div(
            [
                html.Div(
                    [
                        html.P(
                            "Filter by Election Year:",
                            className="control_label",
                        ),
                        dcc.RangeSlider(
                            id='year-picker',
                            min = min(df['ELECTION_INT'].unique()),
                            max = max(df['ELECTION_INT'].unique()),
                            value= [2013, 2017],
                            marks = year_dict

                        ),
                        html.P("Filter by Candidate Name:", className="control_label"),
                        dcc.Dropdown(
                            id="cand_picker",
                            multi=True,
                            value='',
                            className="dcc_control",
                        ),
                        html.P("Filter by Race Type:", className="control_label"),
                        dcc.Dropdown(
                            id="election_type",
                            className="dcc_control",
                            value=[],
                            multi = True # take Out?
                        ),
                    ],
                    className="pretty_container four columns",
                    id="cross-filter-options",
                ),
                html.Div(
                    [
                        html.Div(
                            [
                                html.Div(
                                    [html.H6(id="candidate_text"), html.P("Running Candidates")],
                                    id="wells",
                                    className="mini_container",
                                ),
                                html.Div(
                                    [html.H6(id="tConText"), html.P("In Total Contributions")],
                                    id="gas",
                                    className="mini_container",
                                ),
                                html.Div(
                                    [html.H6(id="conText"), html.P("Average Contributions")],
                                    id="oil",
                                    className="mini_container",
                                ),
                            ],
                            id="info-container",
                            className="row container-display",
                        ),
                        html.Div(
                            [dcc.Graph(id="graph")],
                            id="countGraphContainer",
                            className="pretty_container",
                        ),
                    ],
                    id="right-column",
                    className="eight columns",
                ),
            ],
            className="row flex-display",
        ),
        html.Div(
            [
                html.Div(
                    [dcc.Graph(id="nyc_map")],
                    className="pretty_container seven columns",
                ),
                html.Div(
                    [dcc.Graph(id="individual_graph")],
                    className="pretty_container five columns",
                ),
            ],
            className="row flex-display",
        ),
        html.Div(
            [
                html.Div(
                    #[dash_table.DataTable(
                    #    id='table',
                    #    columns=[{'name': i, "id": i} for i in df.columns],
                    #    data = df.to_dict(orient='records')
                    #)
                #   ],
                    className="pretty_container seven columns",
                ),
                html.Div(
                    [dcc.Graph(id="aggregate_graph")],
                    className="pretty_container five columns",
                ),
            ],
            className="row flex-display",
        ),
    ],
    id="mainContainer",
    style={"display": "flex", "flex-direction": "column"},
)


# Populates Candidate List
@app.callback([Output(component_id='cand_picker', component_property='options'),
            Output(component_id ='cand_picker', component_property='value')],
            [Input(component_id='year-picker', component_property='value')])
def update_Cand_Picker(selected_year):
    year_filler(selected_year)

    filtered_df =df[df['ELECTION_INT'].isin(selected_year)]
    default = filtered_df['RECIPNAME'].unique()
    candidates =[]
    for every_cand in filtered_df['RECIPNAME'].unique():
        candidates.append({'label':every_cand, 'value':every_cand})
    return [candidates, default]

# Updates Candidate Contributions Bar Graph
@app.callback(Output(component_id = 'graph', component_property = 'figure'),
              [Input(component_id = 'year-picker', component_property = 'value'),
               Input(component_id = 'cand_picker', component_property = 'value'),
               Input(component_id = 'election_type', component_property = 'value')
               ]
              )
def update_figure(selected_year, candidate_list, election_type):
    year_filler(selected_year)

    filtered_df = df[df['ELECTION_INT'].isin(selected_year)]
    filtered_df2 = filtered_df[filtered_df['Election District']== election_type]
    #candidate_df = filtered_df2
    specific_candidates = filtered_df2[filtered_df2['RECIPNAME'].isin(list(candidate_list))].groupby('RECIPNAME', as_index = False).agg({'AMNT':'sum'}).sort_values("AMNT", ascending = False)

    fig_data = []
    fig_data.append(go.Bar(x = specific_candidates['RECIPNAME'],
                           y = specific_candidates['AMNT'],
                           ))

    fig = {
        'data':fig_data,
        'layout': go.Layout(
            title = 'Candidate Contributions',
            annotations = [   # Set the x axis title using a custom annotation
                dict(
                    x=-0.04,
                    y=-0.20,
                    showarrow=False,
                    text="Candidate",
                    textangle= -90,
                    xref="paper",
                    yref="paper",
                    font= {
                        'size': 14,
                        'color': 'black'
                    }
                )
            ],
            yaxis = {
                'title': {
                    'text': 'Total Contributions',
                    'font': {
                        'size': 14,
                        'color': 'black'
                    }
                }

            },
           #plot_bgcolor = '#79667a',   # dark purple
           #paper_bgcolor = '#79667a',  # dark purple
           font = {'color': 'black',
                    'size': 10,
            },
            height = 540
        )
    }
    return fig

# Updates Contributions Choropleth by Zip
@app.callback(Output(component_id = 'nyc_map', component_property = 'figure'),
              [Input(component_id = 'year-picker', component_property = 'value'),
               Input(component_id = 'cand_picker', component_property = 'value'),
               Input(component_id = 'election_type', component_property = 'value')
               ]
              )
def update_figure(selected_year, candidate_list, election_type):
    if not selected_year:
        return {'data':[], 'layout':[]}

    year_filler(selected_year)

    filtered_df = nyc_df[nyc_df['ELECTION_INT'].isin(selected_year)]
    filtered_df2 = filtered_df[filtered_df['Election District']== election_type]
    filtered_df3 = filtered_df2[filtered_df2['RECIPNAME'].isin(candidate_list)]
    grouped_df = nyc_df.groupby("CLEAN_ZIP", as_index=False).agg({"AMNT":"sum"})
    grouped_df["LOG_AMNT"] = np.log(grouped_df["AMNT"])

    text_list = []
    for i in range(len(grouped_df)):
        text_list.append('Zip Code: ' + str(grouped_df['CLEAN_ZIP'][i]) + '<br>' + \
                        'Total Contributions: ' + str(grouped_df['AMNT'][i]) + '<br>' + 'Total Number of Contributions: ' + \
                        str(len(grouped_df['AMNT'])))
    grouped_df['text'] = text_list

    fig_data = []
    fig_data = px.choropleth(
        grouped_df,
        geojson=geodata,
        locations='CLEAN_ZIP',
        featureidkey = 'properties.postalcode',
        scope = 'usa',
        color='LOG_AMNT',
        labels={'LOG_AMNT':'Log Contributions', 'CLEAN_ZIP':'Zip Code', 'AMNT':'Total Contribution'},
        color_continuous_scale = 'haline', hover_data = ['AMNT'])

    fig_data.update_geos(fitbounds = 'locations', visible = False)

    fig = {
        'data':fig_data,
        'layout': go.Layout(
            title = 'Total Contributions by Zip Code',
        )
    }

    return fig

# Create callbacks
app.clientside_callback(
    ClientsideFunction(namespace="clientside", function_name="resize"),
    Output("output-clientside", "children"),
    [Input("count_graph", "figure")],
)

# Updates the top left card 'running candidates' above the bar graph
@app.callback(
    Output("candidate_text", "children"),
    [
        Input("year-picker", "value"),
    ],
)
def update_candidate_text(selected_year):
    year_filler(selected_year)

    filtered_df = df[df['ELECTION_INT'].isin(selected_year)]
    candidates = filtered_df['RECIPNAME'].unique()
    return len(candidates)

# Updates the top middle card 'total contribution' above the bar graph
@app.callback(
        Output("tConText", "children"),
    [Input("year-picker", "value")],
)
def update_total_contribution_text(selected_year):
    year_filler(selected_year)

    filtered_df = df[df['ELECTION_INT'].isin(selected_year)]
    commas = "{:,.0f}".format(sum(filtered_df['AMNT']))
    return "$" + str(commas)

# Updates the top right card 'average contribution' above the bar graph
@app.callback(
        Output("conText", "children"),
    [Input("year-picker", "value")],
)
def update_average_contribution_text(selected_year):
    year_filler(selected_year)

    filtered_df = df[df['ELECTION_INT'].isin(selected_year)]
    commas = "{:,.0f}".format(sum(filtered_df['AMNT']) / len(filtered_df['AMNT']))
    return "$" + str(commas)


# Updates the race types filter
@app.callback([Output(component_id='election_type', component_property='options'),
            Output(component_id ='election_type', component_property='value')],
            [Input(component_id='year-picker', component_property='value')])
def update_election_type(selected_year):
    year_filler(selected_year)

    filtered_df =df[df['ELECTION_INT'].isin(selected_year)]
    default = filtered_df['Election District'].unique()
    candidates =[]
    for every_cand in filtered_df['Election District'].unique():
        candidates.append({'label':every_cand, 'value':every_cand})
    return [candidates, default[0]]


# Main
if __name__ == "__main__":
    app.run_server(debug=True)
