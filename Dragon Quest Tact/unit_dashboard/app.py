import pickle
import os.path
from google_auth_oauthlib.flow import InstalledAppFlow
from google.auth.transport.requests import Request
from googleapiclient.discovery import build
import pandas as pd
import numpy as np
import plotly as plt
import plotly.express as px
import plotly.graph_objects as go
import dash
import dash_core_components as dcc
import dash_html_components as html
from dash.dependencies import Input, Output, State, ClientsideFunction
import dash_table
import base64
import os
import clean_database

# Set some custom pandas options
pd.set_option('max_columns', None)
pd.set_option('max_colwidth', 100)
pd.options.display.width = None

def gsheet_api_check(SCOPES):
    creds = None
    if os.path.exists('token.pickle'):
        with open('token.pickle', 'rb') as token:
            creds = pickle.load(token)
    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            flow = InstalledAppFlow.from_client_secrets_file(
                'credentials.json', SCOPES)
            creds = flow.run_local_server(port=0)
        with open('token.pickle', 'wb') as token:
            pickle.dump(creds, token)
    return creds

def pull_sheet_data(SCOPES,SPREADSHEET_ID,DATA_TO_PULL):
    creds = gsheet_api_check(SCOPES)
    service = build('sheets', 'v4', credentials=creds)
    sheet = service.spreadsheets()
    result = sheet.values().get(
        spreadsheetId=SPREADSHEET_ID,
        range=DATA_TO_PULL).execute()
    values = result.get('values', [])

    if not values:
        print('No data found.')
    else:
        rows = sheet.values().get(spreadsheetId=SPREADSHEET_ID,
                                  range=DATA_TO_PULL).execute()
        data = rows.get('values')
        print("COMPLETE: Data copied")
        return data

SCOPES = ['https://www.googleapis.com/auth/spreadsheets']
SPREADSHEET_ID = '1L29jVsLXAvTYotKVdCOBD3-rIvmBIy4XVi8shubo4HY'
DATA_TO_PULL = 'All Rarity'
data = pull_sheet_data(SCOPES,SPREADSHEET_ID,DATA_TO_PULL)

#Get dataframe
dqt = pd.DataFrame(data[1:], columns=data[1])
dqt = clean_database.clean_database(dqt)
# Set our file path
directory = "C:/Users/Peter/Desktop/DQT_dashboard/assets/images"

# Encode our Image Assets
monster_images = []
encoded_images = []
for filename in os.listdir(directory):
    monster_images.append(os.path.join(directory, filename))
for image_filename in monster_images:
    encoded_images.append(base64.b64encode(open(image_filename, 'rb').read()))
HTML_IMG_SRC_PARAMETERS = 'data:image/png;base64, '

# Clean the "S" rarity column
def rarity_S_fixer(s):
    if '.' in s:
        return s.split('.')[1]
    else:
        return s
dqt['Rarity'] = dqt['Rarity'].apply(rarity_S_fixer)

# Drop unneccessary rows
dqt = dqt.replace('',np.NaN).drop(0)
dqt = dqt.dropna(subset=['Family'])     # empty values within the Family Column will be dropped

# Insert encoded images into dataframe
#dqt['Image_Encoded'] = encoded_images

def filterDF(df, rarity, family, res_array, condition_array, atk):
    filtered_df = df[df['Rarity'].isin(rarity)]                                 # filter by rarity
    filtered_df = filtered_df[filtered_df['Family'].isin(family)]               # filter by family
    types = ['Frizz', 'Sizz', 'Crack', 'Woosh', 'Bang', 'Zap', 'Zam']
    conditions = ['Sleep', 'Poison', 'Physical Lock', 'Spell Lock', 'Martial Lock', 'Breath Lock', 'Hobble', 'Stun', 'Blind', 'Curse', 'Paralysis', 'Confusion', 'Charmed']
    # filter by type resistance
    for index, element in enumerate(res_array):
        if element == 0:
            filtered_df = filtered_df[filtered_df[types[index]] == 'Weak']
        if element == 2:
            filtered_df = filtered_df[filtered_df[types[index]] == 'Heavy Res']
    # filter by condition resistance
    for index, element in enumerate(condition_array):
        if element == 0:
            filtered_df = filtered_df[filtered_df[conditions[index]] == 'Weak']
        if element == 2:
            filtered_df = filtered_df[filtered_df[conditions[index]] == 'Half Res']
        if element == 3:
            filtered_df = filtered_df[filtered_df[conditions[index]] == 'Immune']
    # filter by attack type
    if len(atk) > 0:
        for index, element in enumerate(atk):
            filtered_df = filtered_df[(filtered_df['Skill 1 - Damage Element'] == element) | (filtered_df['Skill 2 - Damage Element'] == element) | (filtered_df['Skill 3 - Damage Element'] == element)]

    return filtered_df

### Visualize
app = dash.Dash(__name__)
server = app.server
app.layout = html.Div(
    children=[
        html.Div(
            [
                html.Div(
                    [
                        html.P("Filter by Rarity", className="control_label"),
                        html.Div(
                            # Rarity Picker
                            dcc.Checklist(
                                id="rarity_picker",
                                className="dcc_control",
                                value=[],
                                options=[
                                    {'label': 'S', 'value': 'S'},
                                    {'label': 'A', 'value': 'A'},
                                    {'label': 'B', 'value': 'B'},
                                    {'label': 'C', 'value': 'C'},
                                    {'label': 'D', 'value': 'D'},
                                    {'label': 'E', 'value': 'E'},
                                    {'label': 'F', 'value': 'F'},
                                ],
                                persistence_type='session',
                                persistence=True,
                                style={'max-height': '350px', 'overflow': 'auto'}
                            ),
                            className="row flex-display",
                        ),
                    ],
                    className="pretty_container two columns",
                ),
                html.Div(
                    [
                        html.P("Filter by Family", className="control_label"),
                        html.Div(
                            # Family Picker
                            dcc.Checklist(
                                id="family_picker",
                                className="dcc_control",
                                value=[],
                                options=[
                                    {'label': 'Slime', 'value': 'Slime'},
                                    {'label': 'Demon', 'value': 'Demon'},
                                    {'label': 'Beast', 'value': 'Beast'},
                                    {'label': 'Inorganic', 'value': 'Inorganic'},
                                    {'label': 'Undead', 'value': 'Undead'},
                                    {'label': 'Nature', 'value': 'Nature'},
                                    {'label': 'Dragon', 'value': 'Dragon'},
                                    {'label': '???', 'value': '???'},
                                    {'label': 'Hero', 'value': 'Hero'},
                                ],
                                persistence_type='session',
                                persistence=True,
                                style={'max-height': '350px', 'overflow': 'auto', 'margin':'auto'}
                            ),
                            className="row flex-display",
                        ),
                    ],
                    className="pretty_container two columns"
                ),
                html.Div(
                    [
                    html.P("Elemental Defense", className="control_label"),
                        html.Div([
                            # Resistance Picker
                            html.P("Frizz", className="control_label", style={'text-align':'center'}),
                            # frizz resistance
                            dcc.Slider(
                                id="frizz_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=2,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Sizz", className="control_label", style={'text-align':'center'}),
                            # sizz resistance
                            dcc.Slider(
                                id="sizz_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=2,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Crack", className="control_label", style={'text-align':'center'}),
                            # crack resistance
                            dcc.Slider(
                                id="crack_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=2,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Woosh", className="control_label", style={'text-align':'center'}),
                            # woosh resistance
                            dcc.Slider(
                                id="woosh_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=2,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Bang", className="control_label", style={'text-align':'center'}),
                            # bang resistance
                            dcc.Slider(
                                id="bang_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=2,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Zap", className="control_label", style={'text-align':'center'}),
                            # zap resistance
                            dcc.Slider(
                                id="zap_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=2,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Zam", className="control_label", style={'text-align':'center'}),
                            # zam resistance
                            dcc.Slider(
                                id="zam_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=2,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            #className="row flex-display",
                            ]),
                    ],
                    className="pretty_container five columns",
                ),
                html.Div(
                    [
                    html.P("Status Defense", className="control_label"),
                        html.Div([
                            ## Resistance Picker
                            html.P("Sleep", className="control_label", style={'text-align':'center'}),
                            # sleep resistance
                            dcc.Slider(
                                id="sleep_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Poison", className="control_label", style={'text-align':'center'}),
                            # poison resistance
                            dcc.Slider(
                                id="poison_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Physical Lock", className="control_label", style={'text-align':'center'}),
                            # physical lock resistance
                            dcc.Slider(
                                id="physical_lock_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Spell Lock", className="control_label", style={'text-align':'center'}),
                            # spell lock resistance
                            dcc.Slider(
                                id="spell_lock_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Martial Lock", className="control_label", style={'text-align':'center'}),
                            # martial lock resistance
                            dcc.Slider(
                                id="martial_lock_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Breath Lock", className="control_label", style={'text-align':'center'}),
                            # breath lock resistance
                            dcc.Slider(
                                id="breath_lock_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Hobble", className="control_label", style={'text-align':'center'}),
                            # hobble resistance
                            dcc.Slider(
                                id="hobble_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Stun", className="control_label", style={'text-align':'center'}),
                            # stun resistance
                            dcc.Slider(
                                id="stun_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Blind", className="control_label", style={'text-align':'center'}),
                            # blind resistance
                            dcc.Slider(
                                id="blind_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Curse", className="control_label", style={'text-align':'center'}),
                            # curse resistance
                            dcc.Slider(
                                id="curse_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Paralysis", className="control_label", style={'text-align':'center'}),
                            # paralysis resistance
                            dcc.Slider(
                                id="paralysis_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Confusion", className="control_label", style={'text-align':'center'}),
                            # confusion resistance
                            dcc.Slider(
                                id="confusion_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            html.P("Charmed", className="control_label", style={'text-align':'center'}),
                            # charmed resistance
                            dcc.Slider(
                                id="charmed_res",
                                #className="dcc_control",
                                value=1,
                                step=None,
                                min=0,
                                max=3,
                                marks={
                                    0: {'label': 'Weak'},
                                    1: {'label': 'Neutral'},
                                    2: {'label': 'Resistant'},
                                    3: {'label': 'Immune'}
                                },
                                persistence_type='session',
                                persistence=True,
                                included=False
                            ),
                            #className="row flex-display",
                            ]),
                    ],
                    className="pretty_container five columns",
                ),
                html.Div(
                    [
                        html.P("Filter by Attack Type", className="control_label"),
                        html.Div(
                            # Weakness PIcker
                            dcc.Checklist(
                                id="atk_picker",
                                className="dcc_control",
                                value=[],
                                options=[
                                    {'label': 'Typeless', 'value': 'Typeless'},
                                    {'label': 'Frizz', 'value': 'Frizz'},
                                    {'label': 'Sizz', 'value': 'Sizz'},
                                    {'label': 'Crack', 'value': 'Crack'},
                                    {'label': 'Woosh', 'value': 'Woosh'},
                                    {'label': 'Bang', 'value': 'Bang'},
                                    {'label': 'Zap', 'value': 'Zap'},
                                    {'label': 'Zam', 'value': 'Zam'},
                                ],
                                persistence_type='session',
                                persistence=True,
                                style={'max-height': '350px', 'overflow': 'auto'}
                            ),
                            className="row flex-display",
                            ),
                    ],
                    className="pretty_container two columns",
                ),
            ],
            className="row flex-display"
        ),
        html.Div(
            [
                # Datatable DIV
                html.Div(
                    [
                        html.H1("Searchable Monster Table", className="control_label"),
                        html.P("Example: Typing >400 in Max ATK column will show monsters with greater than 400 attack stat", className="control_label"),
                        html.Br(), html.Br(),
                        html.Div(id='table_statistics'),
                        html.Div(
                            [dash_table.DataTable(
                                id='monster_table',
                                columns=[{"name": i, "id": i} for i in dqt.columns],
                                #data = dqt.to_dict('records'),
                                editable=True,
                                filter_action='native',
                                sort_action='native',
                                page_action='native',
                                sort_mode='single',
                                column_selectable='multi',
                                row_selectable='multi',
                                #page_size= 45
                            )],
                            className='row flex-display',
                        ),
                    ],
                    className='pretty_container'
                )

            ],
            className="row flex-display",
        ),
    ]
)

def html_image(encoded_image):
    return html.Img(src='data:image/png;base64,{}'.format(encoded_image.decode()))


### Callbacks
## Callback for data table
@app.callback([Output('monster_table', 'data')],
              [Input('rarity_picker', 'value'),
               Input('family_picker', 'value'),
               Input('frizz_res', 'value'),
               Input('sizz_res', 'value'),
               Input('crack_res', 'value'),
               Input('woosh_res', 'value'),
               Input('bang_res', 'value'),
               Input('zap_res', 'value'),
               Input('zam_res', 'value'),
               Input('sleep_res', 'value'),
               Input('poison_res', 'value'),
               Input('physical_lock_res', 'value'),
               Input('spell_lock_res', 'value'),
               Input('martial_lock_res', 'value'),
               Input('breath_lock_res', 'value'),
               Input('hobble_res', 'value'),
               Input('stun_res', 'value'),
               Input('blind_res', 'value'),
               Input('curse_res', 'value'),
               Input('paralysis_res', 'value'),
               Input('confusion_res', 'value'),
               Input('charmed_res', 'value'),
               Input('atk_picker', 'value')])
def update_table(rarity, family, frizz, sizz, crack, woosh, bang, zap, zam, sleep, poison, physical_lock, spell_lock, martial_lock, breath_lock, hobble, stun, blind, curse, paralysis, confusion, charmed, atk):
    #print('rarity', rarity, 'family', family, 'res', res, 'weak', weak)
    if rarity == []:
        rarity = dqt["Rarity"].unique()
    if family == []:
        family = dqt["Family"].unique()
    res_array = [frizz, sizz, crack, woosh, bang, zap, zam]
    condition_array = [sleep, poison, physical_lock, spell_lock, martial_lock, breath_lock, hobble, stun, blind, curse, paralysis, confusion, charmed]
    filtered_dqt = filterDF(dqt, rarity, family, res_array, condition_array, atk)
    data = filtered_dqt.to_dict('records')
    return [data]

"""
@app.callback([Output('table_statistics', 'children')],
              [Input('rarity_picker', 'value'),
               Input('family_picker', 'value'),
               Input('resistance_picker', 'value'),
               Input('weakness_picker', 'value'),
               Input('atk_picker', 'value')])
def update_stats(rarity, family, res, weak, atk):
    if rarity == []:
        rarity_key = '0'
        rarity = dqt["Rarity"].unique()
    else:
        rarity_key = '1'
    if family == []:
        family_key = '0'
        family = dqt["Family"].unique()
    else:
        family_key = '1'
    if res == []:
        res_key = '0'
    else:
        res_key = '1'
    if weak == []:
        weak_key = '0'
    else:
        weak_key = '1'
    if atk == []:
        atk_key = '0'
    else:
        atk_key = '1'

    filtered_dqt = filterDF(dqt, rarity, family, res, weak, atk)
    output_key = {
        '00000': 'There are {} total monsters'.format(len(filtered_dqt)),
        '00001': 'There are {} monsters weak to {}'.format(len(filtered_dqt), weak),
        '00010': 'There are {} monsters resistant to {}'.format(len(filtered_dqt), res),
        '00011': 'There are {} monsters resistant to {} and weak to {}'.format(len(filtered_dqt), res, weak),
        '00100': 'There are {} monsters in {}'.format(len(filtered_dqt), family),
        '00101': 'There are {} monsters in {} who are weak to {}'.format(len(filtered_dqt), family, weak),
        '00110': 'There are {} monsters in {} who are resistant to {}'.format(len(filtered_dqt), family, res),
        '00111': 'There are {} monsters in {} who are resistant to {} and weak to {}'.format(len(filtered_dqt), rarity, res, weak),
        '01000': 'There are {} monsters of rank {}'.format(len(filtered_dqt), rarity),
        '01001': 'There are {} monsters of rank {} who are weak to {}'.format(len(filtered_dqt), rarity, weak),
        '01010': 'There are {} monsters of rank {} who are resistant to {}'.format(len(filtered_dqt), rarity, res),
        '01011': 'There are {} monsters of rank {} who are resistant to {} and weak to {}'.format(len(filtered_dqt), rarity, res, weak),
        '01100': 'There are {} monsters of rank {} who are in {}'.format(len(filtered_dqt), rarity, family),
        '01101': 'There are {} monsters of rank {} who are in {} who are weak to {}'.format(len(filtered_dqt), rarity, family, weak),
        '01110': 'There are {} monsters of rank {} who are in {} who are resistant to {}'.format(len(filtered_dqt), rarity, family, res),
        '01111': 'There are {} monsters of rank {} who are in {} who are resistant to {} and weak to {}'.format(len(filtered_dqt), rarity, family, res, weak),
        '10000': 'There are {} total monsters with an attack of type {}'.format(len(filtered_dqt), atk),
        '10001': 'There are {} monsters weak to {} with an attack of type {}'.format(len(filtered_dqt), weak, atk),
        '10010': 'There are {} monsters resistant to {} with an attack of type {}'.format(len(filtered_dqt), res, atk),
        '10011': 'There are {} monsters resistant to {} and weak to {} with an attack of type {}'.format(len(filtered_dqt), res, weak, atk),
        '10100': 'There are {} monsters in {} with an attack of type {}'.format(len(filtered_dqt), family, atk),
        '10101': 'There are {} monsters in {} who are weak to {} with an attack of type {}'.format(len(filtered_dqt), family, weak, atk),
        '10110': 'There are {} monsters in {} who are resistant to {} with an attack of type {}'.format(len(filtered_dqt), family, res, atk),
        '10111': 'There are {} monsters in {} who are resistant to {} and weak to {} with an attack of type {}'.format(len(filtered_dqt), rarity, res, weak, atk),
        '11000': 'There are {} monsters of rank {} with an attack of type {}'.format(len(filtered_dqt), rarity, atk),
        '11001': 'There are {} monsters of rank {} who are weak to {} with an attack of type {}'.format(len(filtered_dqt), rarity, weak, atk),
        '11010': 'There are {} monsters of rank {} who are resistant to {} with an attack of type {}'.format(len(filtered_dqt), rarity, res, atk),
        '11011': 'There are {} monsters of rank {} who are resistant to {} and weak to {} with an attack of type {}'.format(len(filtered_dqt), rarity, res, weak, atk),
        '11100': 'There are {} monsters of rank {} who are in {} with an attack of type {}'.format(len(filtered_dqt), rarity, family, atk),
        '11101': 'There are {} monsters of rank {} who are in {} who are weak to {} with an attack of type {}'.format(len(filtered_dqt), rarity, family, weak, atk),
        '11110': 'There are {} monsters of rank {} who are in {} who are resistant to {} with an attack of type {}'.format(len(filtered_dqt), rarity, family, res, atk),
        '11111': 'There are {} monsters of rank {} who are in {} who are resistant to {} and weak to {} with an attack of type {}'.format(len(filtered_dqt), rarity, family, res, weak, atk),
    }
    return [html.P(output_key[atk_key + rarity_key + family_key + res_key + weak_key])]


______________
Pull database directly from the google sheet


______________
One tab with the table, one tab with a list of images


______________
Add new filter for attack type

______________
encoded_image = encoded_images[1]
return [[html.Div(html.Img(src='data:image/png;base64,{}'.format(encoded_image.decode()))), html.Div(html.Img(src='data:image/png;base64,{}'.format(encoded_images[2].decode())))]]


______________
Toggleable Resist and Weakness to make it filter by using 'AND' or 'OR'
example: Resistant to Frizz AND Sizz AND Crack, vs. Frizz OR Sizz OR Crack

______________
Button that says "filter" that inputs all of the filters and updates the table
likewise, a reset button that will erase all saved inputs
"""

#dqt.to_csv("C:\\Users\\Peter\\Desktop\\DQT_dashboard\\unit_database_cleaned_new.csv")

# Main
if __name__ == "__main__":
    app.run_server(debug=True)
