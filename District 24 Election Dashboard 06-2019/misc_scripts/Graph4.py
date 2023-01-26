import pandas as pd
import plotly.express as px  # (version 4.7.0)
import plotly.graph_objects as go

import dash
import dash_core_components as dcc
import dash_html_components as html
from dash.dependencies import Input, Output

app = dash.Dash()

# Figure 2
trace5 = go.Bar(
    x=['Mahfuzul', 'Baldeo', 'Weprin'],
    y=[1522, 1234, 1100],
    name='Confirmed Votes'
)

layout=go.Layout(
title={
     'text': "Confirmed Votes per Candidate",
     'y':0.9,
     'x':0.5,
     'xanchor': 'center',
     'yanchor': 'top'},
barmode='stack',
xaxis_title="Candidate",
yaxis_title="Number of Votes",
font=dict(
     family="Courier New, monospace",
     size=18,
     color="#7f7f7f"
     )
)

app.layout = html.Div([
    dcc.Graph(id='bar_plot',
              figure=go.Figure(data=[trace5],
                               layout=layout
                               )
              )
    ])

if __name__ == "__main__":
    app.run_server(debug=True, port=8054)
