import pandas as pd
import numpy as np
import re
import math
import plotly.express as px
import plotly.graph_objects as go
import dash
import dash_core_components as dcc
import dash_html_components as html

pd.set_option('display.max_columns', None)
pd.set_option('display.width', 400)


df = pd.read_csv("completed survey final.csv", encoding='cp1252')
#df_demo = pd.read_csv("C:/Users/Peter/Desktop/Creative Science Behavioral Assessment/completed survey final.csv", encoding='cp1252')

### Data Cleaning

## CRT

crt = df.loc[:, 'CRT Q1' : 'CRT Q6']   # dataframe with only the CRT questions

crt['CRT Q1'].replace(to_replace = 10, value = 'c', inplace = True)
crt['CRT Q1'].replace(to_replace = 5, value = 'i', inplace = True)
crt['CRT Q2'].replace(to_replace = 5, value = 'c', inplace = True)
crt['CRT Q2'].replace(to_replace = 100, value = 'i', inplace = True)
crt['CRT Q3'].replace(to_replace = 47, value = 'c', inplace = True)
crt['CRT Q3'].replace(to_replace = 24, value = 'i', inplace = True)
crt['CRT Q4'].replace(to_replace = 4, value = 'c', inplace = True)
crt['CRT Q4'].replace(to_replace = 9, value = 'i', inplace = True)
crt['CRT Q5'].replace(to_replace = 29, value = 'c', inplace = True)
crt['CRT Q5'].replace(to_replace = 30, value = 'i', inplace = True)
crt['CRT Q6'].replace(to_replace = 'has lost money', value = 'c', inplace = True)
crt['CRT Q6'].replace(to_replace = 'is ahead of where he began', value = 'i', inplace = True)

# Create an array that will include all instances where the respondent answered with the "correct" response
fast_scores = []
for value in crt.values:
    count = 0
    for element in value:
        if (element == 'c'):
            count += 1
    fast_scores.append(count)
# Create a new column in the dataframe with every respondent's Slow Scores
crt['CRT Slow Score'] = fast_scores
# Create an array that will include all instances where the respondent answered with the "Fast" response
Fast_scores = []
for value in crt.values:
    count = 0
    for element in value:
        if (element == 'i'):
            count += 1
    Fast_scores.append(count)
# Create a new column in the dataframe with every respondent's Fast scores
crt['CRT Fast Score'] = Fast_scores
# Convert the arrays into numpy arrays so we can manipulate them
fast_np = np.array(fast_scores)
Fast_np = np.array(Fast_scores)
# create the "in between" array that is simply the remaining amount of points not attributed to fast or Fast
in_between_scores = np.add(fast_np, Fast_np)
# Create the column that includes every respondent's in between scores
crt['CRT In Between Score'] = 6 - in_between_scores
# Create a new column that identifies whether the person is a fast, slow, or in between thinker
fast_or_slow = []
for value in crt.values:
    i_count = 0
    c_count = 0
    for element in value:
        if (element == 'i'):
            i_count += 1
        elif (element == 'c'):
            c_count += 1
    if c_count > i_count:
        fast_or_slow.append('Slow')
    elif i_count > c_count:
        fast_or_slow.append('Fast')
    else:
        fast_or_slow.append('In Between')
crt['Thinking Speed'] = fast_or_slow
df['Thinking Speed'] = crt['Thinking Speed']

# Bring over the newly created columns into our main dataframe
df['Cognitive Slow'], df['Cognitive Fast'], df['Cognitive In Between'] = crt['CRT Slow Score'], crt['CRT Fast Score'], crt['CRT In Between Score']

## TP

tp = df.loc[:, 'TP Q1' : 'TP Q3E']    # dataframe with only the time patience and preference questions
# Replace responses with a numerical representation of how much their response is worth towards the final patience score
tp['TP Q1'] = tp['TP Q1'] * 0.2   # tp q1 is worth 20% of the tp score
tp['TP Q2'] = tp['TP Q2'] * 0.2  # tp q2 is worth 20% of the tp score
tp['TP Q3A'].replace(to_replace = "$170 now", value = 0, inplace = True)
tp['TP Q3A'].replace(to_replace = "$340 a year from now", value = 1.2, inplace = True)
tp['TP Q3B'].replace(to_replace = "$170 now", value = 0, inplace = True)
tp['TP Q3B'].replace(to_replace = "$255 a year from now", value = 1.2, inplace = True)
tp['TP Q3C'].replace(to_replace = "$170 now", value = 0, inplace = True)
tp['TP Q3C'].replace(to_replace = "$213 a year from now", value = 1.2, inplace = True)
tp['TP Q3D'].replace(to_replace = "$170 now", value = 0, inplace = True)
tp['TP Q3D'].replace(to_replace = "$191 a year from now", value = 1.2, inplace = True)
tp['TP Q3E'].replace(to_replace = "$170 now", value = 0, inplace = True)
tp['TP Q3E'].replace(to_replace = "$175 a year from now", value = 1.2, inplace = True)
# Add up every respondent's scores and store it in a new column
tp['TP Score'] = tp.agg("sum", axis = 1)
# Then bring in the new column back into the original dataframe
df['Patience'] = tp['TP Score']


## RSK

rsk = df.loc[:, 'RSK Q1' : 'RSK Q2H']   # dataframe with only the risk aversion questions
# Replace responses with a numerical representation of how much their response is worth towards the final risk score
rsk['RSK Q1'] = rsk['RSK Q1'] * 0.4     # rsk q1 is worth 40% of the rsk score
rsk['RSK Q2A'].replace(to_replace = "100% chance of $35", value = 0, inplace = True)
rsk['RSK Q2A'].replace(to_replace = "50/50 chance of $565", value = 0.75, inplace = True)
rsk['RSK Q2B'].replace(to_replace = "100% chance of $70", value = 0, inplace = True)
rsk['RSK Q2B'].replace(to_replace = "50/50 chance of $565", value = 0.75, inplace = True)
rsk['RSK Q2C'].replace(to_replace = "100% chance of $150", value = 0, inplace = True)
rsk['RSK Q2C'].replace(to_replace = "50/50 chance of $565", value = 0.75, inplace = True)
rsk['RSK Q2D'].replace(to_replace = "100% chance of $230", value = 0, inplace = True)
rsk['RSK Q2D'].replace(to_replace = "50/50 chance of $565", value = 0.75, inplace = True)
rsk['RSK Q2E'].replace(to_replace = "100% chance of $320", value = 0, inplace = True)
rsk['RSK Q2E'].replace(to_replace = "50/50 chance of $565", value = 0.75, inplace = True)
rsk['RSK Q2F'].replace(to_replace = "100% chance of $410", value = 0, inplace = True)
rsk['RSK Q2F'].replace(to_replace = "50/50 chance of $565", value = 0.75, inplace = True)
rsk['RSK Q2G'].replace(to_replace = "100% chance of $480", value = 0, inplace = True)
rsk['RSK Q2G'].replace(to_replace = "50/50 chance of $565", value = 0.75, inplace = True)
rsk['RSK Q2H'].replace(to_replace = "100% chance of $550", value = 0, inplace = True)
rsk['RSK Q2H'].replace(to_replace = "50/50 chance of $565", value = 0.75, inplace = True)
# Add up every respondent's scores and store it in a new column
rsk['RSK Score'] = rsk.agg("sum", axis = 1)
# Then bring in the new column back into the original dataframe
df['Risk Prone'] = rsk['RSK Score']

## STI

sti = df.loc[:, 'STI Q1' : 'STI Q2G']   # dataframe with only the response to stimuli questions
# Create a function to convert the dollar amount in sti q1 to an integer that we can do calculations with
def dollar_to_num(s):
    return int(s.split('$')[1])
# Clean the STI Q1 column
sti['STI Q1'].replace(to_replace = "No Gift", value = '$0', inplace = True) # change 'no gift' to '$0' in sti q1
sti['STI Q1'] = sti['STI Q1'].apply(dollar_to_num)  # apply the dollar to number function across the sti q1 column
sti['STI Q1'] = sti['STI Q1'] / 25 # Convert the numerical value to its respective score
# flip the responses to its corresponding "negative" value since the connotation of the question is negative
sti['STI Q2A'] = abs(sti['STI Q2A'] - 11)
sti['STI Q2B'] = abs(sti['STI Q2B'] - 11)
sti['STI Q2C'] = abs(sti['STI Q2C'] - 11)
sti['STI Q2D'] = abs(sti['STI Q2D'] - 11)
# Clean the rest of the columns to reflect their score value
sti['STI Q2A'] = sti['STI Q2A'] * (.6/7)
sti['STI Q2B'] = sti['STI Q2B'] * (.6/7)
sti['STI Q2C'] = sti['STI Q2C'] * (.6/7)
sti['STI Q2D'] = sti['STI Q2D'] * (.6/7)
sti['STI Q2E'] = sti['STI Q2E'] * (.6/7)
sti['STI Q2F'] = sti['STI Q2F'] * (.6/7)
sti['STI Q2G'] = sti['STI Q2G'] * (.6/7)
# Add up every respondent's scores and store it in a new column
sti['STI Score'] = sti.agg("sum", axis = 1).round(2)
# Then bring in the new column back into the original dataframe
df['Positive Relationships'] = sti['STI Score']


## TRU

tru = df.loc[:, 'TRU Q1']   # dataframe with only the trust question
df['Trust'] = tru   # no cleaning required


## ALT
alt = df.loc[:, 'ALT Q1' : 'ALT Q2']    # dataframe with only the altruism questions
alt['ALT Q1'] = alt['ALT Q1'] * 0.4
alt['ALT Q2'] = (alt['ALT Q2'] / 300).round(2)
# Add up every respondent's scores and store it in a new column
alt['ALT Score'] = alt.agg("sum", axis = 1)
# Then bring in the new column back into the original dataframe
df['Altruism'] = alt['ALT Score']


## Aggregate
# Create new dataframes showing profession statistics
profession_stats = df.groupby("Profession").agg("mean").round(2)
profession_scores = profession_stats.loc[:, 'Cognitive Slow':'Altruism'].sort_values(by = ['Cognitive Slow', 'Cognitive Fast', 'Cognitive In Between', 'Altruism', 'Trust', 'Positive Relationships', 'Risk Prone', 'Patience'])
profession_age = df.groupby("Age").agg("mean").round(2)
profession_involement = df.groupby("MD Purchase Involvement").agg("mean").round(2)
df2 = df.groupby("Profession")["Gender"].value_counts()
# Create a new Dataframe showing race statistics
race_amount = df["Ethnicity"].value_counts().reset_index()
race_amount.rename({"index":"Ethnicity", "Ethnicity":"Amount"}, inplace=True, axis="columns")
thinking_speed = df.groupby("Thinking Speed").agg("mean").round(2)
thinking_speed_by_profession = df.groupby("Profession")["Thinking Speed"].value_counts()
fast_thinkers = df[df["Thinking Speed"]=='Fast'].groupby('Profession').agg("mean").round(2)
slow_thinkers = df[df["Thinking Speed"]=='Slow'].groupby('Profession').agg("mean").round(2)
in_betweeners = df[df["Thinking Speed"]=='In Between'].groupby('Profession').agg("mean").round(2)
# Scores by thinking speed
think_speed_fig = px.line(
    thinking_speed,
    x = thinking_speed.index,
    y = ['Patience', 'Risk Prone', 'Positive Relationships', 'Trust', 'Altruism']
)

### Visualize
app = dash.Dash(__name__)
app.layout = html.Div(
    children=[
        html.Div(       # This division is for the title at the top of the page
            [
                html.Div(
                    [
                        html.Div(
                            [
                                html.H3(
                                    "Behavioral Assessment Analysis of Healthcare Professionals",
                                    style={"margin-bottom": "0px"},
                                ),
                                html.H5(
                                    "A Dashboard by Creative Science", style={"margin-top": "0px"}
                                ),
                            ]
                        )
                    ],
                    className="twelve columns",
                    id="title",
                ),
            ],
            id="header",
            className="row flex-display",
            style={"margin-bottom": "25px"},
        ),
        html.Div(
            [
                dcc.Graph(      # Gender of each Profession Stacked Bar Graph
                    figure = {
                        'data' :
                            [go.Bar(
                                name = 'Male',
                                y = [df2["Nurse", "Male"], df2["Healthcare Admin", "Male"], df2["General Physician", "Male"], df2["Surgeon", "Male"], df2["Operating Room Staff", "Male"]],
                                x = ["Nurse", "Healthcare Admin", "General Physician", "Surgeon", "Operating Room Staff"],
                            ),
                            go.Bar(
                                name = 'Female',
                                y = [df2["Nurse", "Female"], df2["Healthcare Admin", "Female"], df2["General Physician", "Female"], df2["Surgeon", "Female"], df2["Operating Room Staff", "Female"]],
                                x = ["Nurse", "Healthcare Admin", "General Physician", "Surgeon", "Operating Room Staff"],
                            )
                        ],
                        'layout' :
                            go.Layout(
                                barmode = 'stack',
                                title = {
                                    'text':'<b>Gender by Profession</b>',
                                    'font':{
                                        'size':25
                                    },
                                    'x':0.5
                                },
                                xaxis_title = '<b>Profession</b>',
                                yaxis_title = '<b>Amount of Respondents</b>',
                                title_x = 0.5,
                                xaxis = {
                                    'showgrid':True,
                                    'gridwidth':1,
                                    'gridcolor':'rgb(124, 127, 130)'
                                },
                                yaxis = {
                                    'showgrid':True,
                                    'gridwidth':1,
                                    'gridcolor':'rgb(124, 127, 130)'
                                },
                                plot_bgcolor = 'rgb(70, 72, 74)',
                                paper_bgcolor = 'rgb(250, 253, 255)'
                            )
                    }
                ),   # End of Gender Bar Graph Division
            ],
            style={"margin-bottom": "25px"}
        ),
        html.Div(
            [
                dcc.Graph(          # Race Pie Chart
                    figure = {
                        'data' :
                            [go.Pie(
                                values = race_amount['Amount'],
                                labels = ['White', 'Black', 'Latino/Hispanic', 'Asian'],
                                textinfo='label+percent',
                            )],
                        'layout' :
                            go.Layout(
                                title = {
                                    'text':'<b>Ethnicity/Race of Respondents</b>',
                                    'font':{
                                        'size':25
                                    },
                                    'x':0.5
                                },
                                title_x = 0.5,
                                plot_bgcolor = 'rgb(70, 72, 74)',
                                paper_bgcolor = 'rgb(250, 253, 255)'
                            )
                    }
                )           # End of Race Pie Chart
            ],
            style={"margin-bottom": "25px"}
        ),
        html.Div(
            [
                dcc.Graph(
                    figure = {
                        'data' :
                            [go.Bar(
                                name='Cognitive Slow',
                                x = profession_scores.index,
                                y = profession_scores['Cognitive Slow']
                            ),
                            go.Bar(
                                name='Cognitive Fast',
                                x = profession_scores.index,
                                y = profession_scores['Cognitive Fast']
                            ),
                            go.Bar(
                                name='Cognitive In Between',
                                x = profession_scores.index,
                                y = profession_scores['Cognitive In Between']
                            ),
                            go.Bar(
                                name='Altruism',
                                x = profession_scores.index,
                                y = profession_scores['Altruism']
                            ),
                            go.Bar(
                                name='Trust',
                                x = profession_scores.index,
                                y = profession_scores['Trust']
                            ),
                            go.Bar(
                                name='Risk Prone',
                                x = profession_scores.index,
                                y = profession_scores['Risk Prone']
                            ),
                            go.Bar(
                                name='Patience',
                                x = profession_scores.index,
                                y = profession_scores['Patience']
                            )
                            ],
                        'layout' :
                            go.Layout(
                                    barmode='group',
                                    title = {
                                        'text':'<b>Average Behavioral Scores per Profession</b>',
                                        'font':{
                                            'size':25
                                        },
                                        'x':0.5
                                    },
                                    yaxis_title = '<b>Average Score</b>',
                                    xaxis_title = '<b>Profession</b>',
                                    title_x = 0.5,
                                    xaxis = {
                                        'showgrid':True,
                                        'gridwidth':1,
                                        'gridcolor':'rgb(124, 127, 130)'
                                    },
                                    yaxis = {
                                        'showgrid':True,
                                        'gridwidth':1,
                                        'gridcolor':'rgb(124, 127, 130)'
                                    },
                                    plot_bgcolor = 'rgb(70, 72, 74)',
                                    paper_bgcolor = 'rgb(250, 253, 255)'
                            )
                    }
                )
            ],
            style={"margin-bottom": "25px"}
        ),
        html.Div(
            [
                dcc.Graph(
                    figure = {
                        'data' :
                                [
                                    go.Bar(
                                        name = 'Patience',
                                        x = fast_thinkers.index,
                                        y = fast_thinkers['Patience'],
                                    ),
                                    go.Bar(
                                        name = 'Risk Prone',
                                        x = fast_thinkers.index,
                                        y = fast_thinkers['Risk Prone'],
                                    ),
                                    go.Bar(
                                        name = 'Positive Relationships',
                                        x = fast_thinkers.index,
                                        y = fast_thinkers['Positive Relationships'],
                                    ),
                                    go.Bar(
                                        name = 'Trust',
                                        x = fast_thinkers.index,
                                        y = fast_thinkers['Trust'],
                                    ),
                                    go.Bar(
                                        name = 'Altruism',
                                        x = fast_thinkers.index,
                                        y = fast_thinkers['Altruism'],
                                    )
                                ],
                        'layout' :
                            go.Layout(
                                    barmode='group',
                                    title = {
                                        'text':'<b>Average Behavioral Scores Among Fast Thinkers</b>',
                                        'font':{
                                            'size':25
                                        },
                                        'x':0.5
                                    },
                                    yaxis_title = '<b>Average Score</b>',
                                    xaxis_title = '<b>Profession</b>',
                                    title_x = 0.5,
                                    xaxis = {
                                        'showgrid':True,
                                        'gridwidth':1,
                                        'gridcolor':'rgb(124, 127, 130)'
                                    },
                                    yaxis = {
                                        'showgrid':True,
                                        'gridwidth':1,
                                        'gridcolor':'rgb(124, 127, 130)'
                                    },
                                    plot_bgcolor = 'rgb(70, 72, 74)',
                                    paper_bgcolor = 'rgb(250, 253, 255)'
                            )
                    }
                )
            ],
            style={"margin-bottom": "25px"}
        ),
        html.Div(
            [
                dcc.Graph(
                    figure = {
                        'data' :
                                [
                                    go.Bar(
                                        name = 'Patience',
                                        x = slow_thinkers.index,
                                        y = slow_thinkers['Patience'],
                                    ),
                                    go.Bar(
                                        name = 'Risk Prone',
                                        x = slow_thinkers.index,
                                        y = slow_thinkers['Risk Prone'],
                                    ),
                                    go.Bar(
                                        name = 'Positive Relationships',
                                        x = slow_thinkers.index,
                                        y = slow_thinkers['Positive Relationships'],
                                    ),
                                    go.Bar(
                                        name = 'Trust',
                                        x = slow_thinkers.index,
                                        y = slow_thinkers['Trust'],
                                    ),
                                    go.Bar(
                                        name = 'Altruism',
                                        x = slow_thinkers.index,
                                        y = slow_thinkers['Altruism'],
                                    )
                                ],
                        'layout' :
                            go.Layout(
                                    barmode='group',
                                    title = {
                                        'text':'<b>Average Behavioral Scores Among Slow Thinkers</b>',
                                        'font':{
                                            'size':25
                                        },
                                        'x':0.5
                                    },
                                    yaxis_title = '<b>Average Score</b>',
                                    xaxis_title = '<b>Profession</b>',
                                    title_x = 0.5,
                                    xaxis = {
                                        'showgrid':True,
                                        'gridwidth':1,
                                        'gridcolor':'rgb(124, 127, 130)'
                                    },
                                    yaxis = {
                                        'showgrid':True,
                                        'gridwidth':1,
                                        'gridcolor':'rgb(124, 127, 130)'
                                    },
                                    plot_bgcolor = 'rgb(70, 72, 74)',
                                    paper_bgcolor = 'rgb(250, 253, 255)'
                            )
                    }
                )
            ],
            style={"margin-bottom": "25px"}
        ),
        html.Div(
            [
                dcc.Graph(
                    figure = {
                        'data' :
                            [go.Scatter(
                                name = 'Patience',
                                x = thinking_speed.index,
                                y = thinking_speed['Patience']
                            ),
                            go.Scatter(
                                name = 'Risk Prone',
                                x = thinking_speed.index,
                                y = thinking_speed['Risk Prone']
                            ),
                            go.Scatter(
                                name = 'Positive Relationships',
                                x = thinking_speed.index,
                                y = thinking_speed['Positive Relationships']
                            ),
                            go.Scatter(
                                name = 'Trust',
                                x = thinking_speed.index,
                                y = thinking_speed['Trust']
                            ),
                            go.Scatter(
                                name = 'Altruism',
                                x = thinking_speed.index,
                                y = thinking_speed['Altruism']
                            ),
                            ],
                        'layout' :
                            go.Layout(
                                title = {
                                    'text':'<b>Behavioral Scores Across Thinking Speed</b>',
                                    'font':{
                                        'size':25
                                    },
                                    'x':0.5
                                },
                                yaxis_title = '<b>Average Score</b>',
                                xaxis_title = '<b>Thinking Speed</b>',
                                title_x = 0.5,
                                xaxis = {
                                    'showgrid':True,
                                    'gridwidth':1,
                                    'gridcolor':'rgb(124, 127, 130)'
                                },
                                yaxis = {
                                    'showgrid':True,
                                    'gridwidth':1,
                                    'gridcolor':'rgb(124, 127, 130)'
                                },
                                plot_bgcolor = 'rgb(70, 72, 74)',
                                paper_bgcolor = 'rgb(250, 253, 255)'
                            )
                    }
                )
            ]
        ),
        html.Div(
            [
                dcc.Graph(
                    figure = {
                        'data' :
                            [go.Scatter(
                                name='Cognitive Slow',
                                x = profession_age.index,
                                y = profession_age['Cognitive Slow'],
                            ),
                            go.Scatter(
                                name='Cognitive Fast',
                                x = profession_age.index,
                                y = profession_age['Cognitive Fast'],
                            ),
                            go.Scatter(
                                name='Cognitive In Between',
                                x = profession_age.index,
                                y = profession_age['Cognitive In Between'],
                            ),
                            go.Scatter(
                                name='Altruism',
                                x = profession_age.index,
                                y = profession_age['Altruism'],
                            ),
                            go.Scatter(
                                name='Trust',
                                x = profession_age.index,
                                y = profession_age['Trust'],
                            ),
                            go.Scatter(
                                name='Positive Relationships',
                                x = profession_age.index,
                                y = profession_age['Positive Relationships'],
                            ),
                            go.Scatter(
                                name='Risk Prone',
                                x = profession_age.index,
                                y = profession_age['Risk Prone'],
                            ),
                            go.Scatter(
                                name='Patience',
                                x = profession_age.index,
                                y = profession_age['Patience'],
                            )],
                        'layout' :
                            go.Layout(
                                barmode = 'group',
                                title = {
                                    'text':'<b>Behavioral Scores by Age Group</b>',
                                    'font':{
                                        'size':25
                                    },
                                    'x':0.5
                                },
                                yaxis_title = '<b>Average Score</b>',
                                xaxis_title = '<b>Age</b>',
                                title_x = 0.5,
                                xaxis = {
                                    'showgrid':True,
                                    'gridwidth':1,
                                    'gridcolor':'rgb(124, 127, 130)'
                                },
                                yaxis = {
                                    'showgrid':True,
                                    'gridwidth':1,
                                    'gridcolor':'rgb(124, 127, 130)'
                                },
                                plot_bgcolor = 'rgb(70, 72, 74)',
                                paper_bgcolor = 'rgb(250, 253, 255)'
                            )
                    }
                )
            ],
            style={"margin-bottom": "25px"}
        ),
        html.Div(
            [
                dcc.Graph(
                    figure = {
                        'data' :
                            [go.Scatter(
                                name='Cognitive Slow',
                                x = profession_involement.index,
                                y = profession_involement['Cognitive Slow'],
                            ),
                            go.Scatter(
                                name='Cognitive Fast',
                                x = profession_involement.index,
                                y = profession_involement['Cognitive Fast'],
                            ),
                            go.Scatter(
                                name='Cognitive In Between',
                                x = profession_involement.index,
                                y = profession_involement['Cognitive In Between'],
                            ),
                            go.Scatter(
                                name='Altruism',
                                x = profession_involement.index,
                                y = profession_involement['Altruism'],
                            ),
                            go.Scatter(
                                name='Trust',
                                x = profession_involement.index,
                                y = profession_involement['Trust'],
                            ),
                            go.Scatter(
                                name='Positive Relationships',
                                x = profession_involement.index,
                                y = profession_involement['Positive Relationships'],
                            ),
                            go.Scatter(
                                name='Risk Prone',
                                x = profession_involement.index,
                                y = profession_involement['Risk Prone'],
                            ),
                            go.Scatter(
                                name='Patience',
                                x = profession_involement.index,
                                y = profession_involement['Patience'],
                            )],
                        'layout' :
                            go.Layout(
                                title = {
                                    'text':'<b>Behavioral Scores as per Purchase Involvment</b>',
                                    'font':{
                                        'size':25
                                    },
                                    'x':0.5
                                },
                                yaxis_title = '<b>Average Score</b>',
                                xaxis_title = '<b>Reported Purchase Involvement</b>',
                                title_x = 0.5,
                                xaxis = {
                                    'showgrid':True,
                                    'gridwidth':1,
                                    'gridcolor':'rgb(124, 127, 130)'
                                },
                                yaxis = {
                                    'showgrid':True,
                                    'gridwidth':1,
                                    'gridcolor':'rgb(124, 127, 130)'
                                },
                                plot_bgcolor = 'rgb(70, 72, 74)',
                                paper_bgcolor = 'rgb(250, 253, 255)'
                            )
                    }
                )
            ],
            style={"margin-bottom": "25px"}
        ),
        html.Div(
            [
                dcc.Graph(
                    figure = {
                        'data' :
                            [go.Scatter(
                                name = 'General Physician',
                                x = [profession_stats['MD Purchase Involvement']['General Physician']],
                                y = [profession_stats['MD Purchase Hours/Week']['General Physician']],
                                marker=dict(symbol="diamond-dot",size=25,color=0),
                            ),
                            go.Scatter(
                                name = 'Healthcare Admin',
                                x = [profession_stats['MD Purchase Involvement']['Healthcare Admin']],
                                y = [profession_stats['MD Purchase Hours/Week']['Healthcare Admin']],
                                marker=dict(symbol="diamond-dot",size=25,color=1),
                            ),
                            go.Scatter(
                                name = 'Nurse',
                                x = [profession_stats['MD Purchase Involvement']['Nurse']],
                                y = [profession_stats['MD Purchase Hours/Week']['Nurse']],
                                marker=dict(symbol="diamond-dot",size=25,color=2),
                            ),
                            go.Scatter(
                                name = 'Operating Room Staff',
                                x = [profession_stats['MD Purchase Involvement']['Operating Room Staff']],
                                y = [profession_stats['MD Purchase Hours/Week']['Operating Room Staff']],
                                marker=dict(symbol="diamond-dot",size=25,color=3),
                            ),
                            go.Scatter(
                                name = 'Surgeon',
                                x = [profession_stats['MD Purchase Involvement']['Surgeon']],
                                y = [profession_stats['MD Purchase Hours/Week']['Surgeon']],
                                marker=dict(symbol="diamond-dot",size=25,color=4),
                            ),
                            ],
                        'layout' :
                            go.Layout(
                                title = {
                                    'text':'<b>Perceived MD Purchasing Involement vs. Amount of Time Spent Each Week Buying Devices</b>',
                                    'font':{
                                        'size':25
                                    },
                                    'x':0.5
                                },
                                yaxis_title = '<b>Purchase Hours/Week</b>',
                                xaxis_title = '<b>MD Purchase Involvment</b>',
                                title_x = 0.5,
                                xaxis = {
                                    'showgrid':True,
                                    'gridwidth':1,
                                    'gridcolor':'rgb(124, 127, 130)'
                                },
                                yaxis = {
                                    'showgrid':True,
                                    'gridwidth':1,
                                    'gridcolor':'rgb(124, 127, 130)'
                                },
                                plot_bgcolor = 'rgb(70, 72, 74)',
                                paper_bgcolor = 'rgb(250, 253, 255)'
                            )
                    }
                )
            ],
            style={"margin-bottom": "25px"}
        )
    ]
)

server = app.server

# Main
if __name__ == "__main__":
    app.run_server(debug=True)


### Export to a csv
#profession_stats.to_csv("Creative_Science_HCP_Survey_Results_by_Profession.csv")
