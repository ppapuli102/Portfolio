import pandas as pd
import plotly.express as px  # (version 4.7.0)
import plotly.graph_objects as go

import dash
import dash_core_components as dcc
import dash_html_components as html
from dash.dependencies import Input, Output

app = dash.Dash()

# Figure 2
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

layout=go.Layout(
title={
     'text': "Voter Outreach",
     'y':0.9,
     'x':0.5,
     'xanchor': 'center',
     'yanchor': 'top'},
barmode='stack',
xaxis_title="Zip Code",
yaxis_title="Number of People",
font=dict(
     family="Courier New, monospace",
     size=18,
     color="#7f7f7f"
     )
)

app.layout = html.Div([
    dcc.Graph(id='scatter_plot',
              figure=go.Figure(data=[trace3, trace4],
                               layout=layout
                               )
              )
    ])

if __name__ == "__main__":
    app.run_server(debug=True, port=8053)
