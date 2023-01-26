import plotly.express as px
import pandas as pd


df = pd.read_csv("C:/Users/Peter/Desktop/mtaAssessment.csv")

df['year'] = df["DATE"]

for i in range(len(df['DATE'])):
    df['year'][i] = df['year'][i][-4:]

year_list = ['2012', '2013', '2014']
new_df = df[df['year'].isin(year_list)]

station_dict = {}

for station in new_df['STATION'].unique():
    if new_df['STATION'] == station:
        for year in year_list:
            if new_df['year'] == year:
                year_dict = {}
                year_dict.update(year = (new_df.agg({'ENTRIES':'sum'}) + new_df.agg({'EXITS':'sum'}))
        station_dict.update(station = year_dict)

plot_data = pd.DataFrame.from_dict(station_dict)


min_calculations = []

for year in year_list:
    min_calculations.append(plot_data[year].min())

fig = px.bar(min_calculations, x = 'Station', y = 'Total Entries and Exits')
fig.show()
