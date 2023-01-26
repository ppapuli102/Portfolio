import pandas as pd
import plotly.express as px  # (version 4.7.0)
import plotly.graph_objects as go
import numpy as np
import dash
import dash_core_components as dcc
import dash_html_components as html
from dash.dependencies import Input, Output
app = dash.Dash()


np.random.seed(42)
random_x = np.random.randint(1,101,100)
random_y = np.random.randint(1,101,100)

app.layout = html.Div([
    dcc.Graph(id='scatter plot',
              figure= {'data':[
                        go.Scatter(
                        x = random_x,
                        y = random_y,
                        mode = "markers",
                        marker = {
                            'size':12,
                            'color': 'rgb(51,204,153)',
                            'symbol':'pentagon',
                            'line':{'width':2}
                            }
                            )],
              'layout': go.Layout(title="My Scatterplot",
                                  xaxis = {'title': "some x title"})}
                               ),
    dcc.Graph(id='scatter plot2',
              figure= {'data':[
                        go.Scatter(
                        x = random_x,
                        y = random_y,
                        mode = "markers",
                        marker = {
                            'size':15,
                            'color': 'rgb(200,204,153)',
                            'symbol':'pentagon',
                            'line':{'width':2}
                            }
                            )],
              'layout': go.Layout(title="My Scatterplot",
                                  xaxis = {'title': "some x title"})}
                               )
                        ])
if __name__=='__main__':
    app.run_server()
