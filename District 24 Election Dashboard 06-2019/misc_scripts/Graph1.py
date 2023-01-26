import pandas as pd
import plotly.express as px  # (version 4.7.0)
import plotly.graph_objects as go

import dash
import dash_core_components as dcc
import dash_html_components as html
from dash.dependencies import Input, Output


help(go.Layout.yaxis)










#app = dash.Dash()



#if __name__ == "__main__":
    #app.run_server(debug=True, port=8051)

"""
df = pd.read_csv("C:/Users/Peter/Desktop/Mf24/election.csv")

fig = px.bar(df, x = 'election_id', y='votes_cast', log_y=True, color='votes_cast')

'''trace1 = go.Bar(
    x = 'election_id',
    y='votes_cast',
    name='Historic Votes per Election'
)'''

layout=go.Layout(
title={
     'text': "Historical Election Results",
     'y':0,
     'x':0,
     'xanchor': 'left',
     'yanchor': 'top'},
xaxis_title="Candidate",
yaxis_title="Number of Votes",
font=dict(
     family="Courier New, monospace",
     size=18,
     color="#7f7f7f"
    )
)

# Add dropdown
fig.update_layout(
    updatemenus=[
        dict(
            buttons=list([
                dict(
                    args=["type", "bar"],
                    label="Bar Chart",
                    method="restyle"
                ),
                dict(
                    args=["type", "scatter"],
                    label="Scatter Plot",
                    method="restyle"
                )
            ]),
            direction="down",
            pad={"r": 10, "t": 10},
            showactive=True,
            x=0.1,
            xanchor="left",
            y=1.14,
            yanchor="top"
        )
    ],
        annotations=[
            dict(text="Trace type:", showarrow=False,
            x=5, y=1.085, yref="paper", align="left")
        ]
    )


app.layout = html.Div([
    dcc.Graph(id='bar_plot',
              figure=go.Figure(data=fig,
                               layout=layout
                               )
              )
    ])
"""
