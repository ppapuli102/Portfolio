import pandas as pd
import plotly.graph_objects as go
import dash
import dash_core_components as dcc
import dash_html_components as html
from dash.dependencies import Input, Output

app = dash.Dash()

# Figure 1 Attributes
trace1 = go.Bar(
    x=['Mahfuzul', 'Baldeo', 'Weprin'],
    y=[1600, 1250, 850],
    name='Absentee Ballots'
)
trace2 = go.Bar(
    x=['Mahfuzul', 'Baldeo', 'Weprin'],
    y=[850, 600, 550],
    name='In-Person casts'
)
layout = go.Layout(
    title={
         'text': "Votes per Candidate",
         'y':0.9,
         'x':0.5,
         'xanchor': 'center',
         'yanchor': 'top'
    },
    xaxis= {'title': 'Zip Code',
            'tickformat': '.00'},
    yaxis = {'title': 'Number of Votes',
            'tickformat': '.00'},
    barmode = 'stack',
    font=dict(
         family="Baskerville Old Face, monospace",
         size=22,
         color="#7f7f7f"
    ),
    plot_bgcolor="#111111",
    paper_bgcolor="#111111"
)

# Figure 2 Attributes
trace3 = go.Scatter(
    x=['10452', '10453', '10454', '10455', '10456', '10457'],
    y=[500, 250, 130, 100, 509, 1200],
    name='People Contacted'
)
trace4 = go.Scatter(
    x=['10452', '10453', '10454', '10455', '10456', '10457'],
    y=[100, 45, 56, 12, 150, 356],
    name='People Pledged'
)
layout2 = go.Layout(
    title={
         'text': "Voter Outreach",
         'y':0.9,
         'x':0.5,
         'xanchor': 'center',
         'yanchor': 'top'
    },
    xaxis= {'title': 'Zip Code',
            'tickformat': '.00'},
    yaxis = {'title': 'Number of People',
            'tickformat': '.00'},
    font=dict(
         family="Baskerville Old Face, monospace",
         size= 22,
         color="#7f7f7f"
    ),
    plot_bgcolor="#111111",
    paper_bgcolor="#111111"
)

# Figure 3 Attributes
df = pd.read_csv("C:/Users/Peter/Desktop/Mf24/election.csv")
trace5 = go.Bar(
    x = df['election_id'],
    y = df['votes_cast'],
    name = 'Historic Votes per Election'
)
layout3 = go.Layout(
    title={
         'text': "Historical Election Results",
         'xanchor': 'center',
         'yanchor': 'top'
    },
    xaxis= {'title': 'Election',
            'tickformat': '.00'},
    yaxis = {'title': 'Vote Count',
            'tickformat': '.00'},
    font=dict(
         family="Baskerville Old Face, monospace",
         size=22,
         color="#7f7f7f"
    ),
    plot_bgcolor="#111111",
    paper_bgcolor="#111111",
)

# Figure 4 Attributes
trace6 = go.Bar(
    x=['Mahfuzul', 'Baldeo', 'Weprin'],
    y=[1522, 1234, 1100],
    name='Confirmed Votes'
)
layout4 = go.Layout(
    title={
         'text': "Confirmed Votes per Candidate",
         'y':0.9,
         'x':0.5,
         'xanchor': 'center',
         'yanchor': 'top'},
    barmode='stack',
    xaxis= {'title': 'Election',
            'tickformat': '.00'},
    yaxis = {'title': 'Vote Count',
            'tickformat': '.00'},
    font=dict(
         family="Baskerville Old Face, monospace",
         size=22,
         color="#7f7f7f"
    ),
    plot_bgcolor="#111111",
    paper_bgcolor="#111111",
)

app.layout = html.Div([
    dcc.Graph(
        id='Votes per Candidate',
        figure= {
            'data': [trace1, trace2],
            'layout': layout
        }
    ),
    dcc.Graph(
        id='Voter Outreach',
        figure= {
            'data': [trace3, trace4],
            'layout': layout2
        }
    ),
    dcc.Graph(
        id='Historical Elections',
        figure= {
            'data': [trace5],
            'layout': layout3
        }
    ),
    dcc.Graph(
        id='Confirmed Votes per Candidate',
        figure= {
            'data': [trace6],
            'layout': layout4
        }
    )
])

if __name__ == "__main__":
    app.run_server()
