# Portfolio
Hi, my name's Peter. Here are some miscellaneous projects I've worked on. Please also check out my other repositories as well!

I worked on a campaign finance dashboard that collected public campaign contributions data and put it in an easy to digest dashboard for local politicians to use to gain an edge over their competition.

I made this alongside another colleague using Pandas to wrangle the data, creating visualizations using Plotly and then serving it to a Flask instance using Dash.

After cleaning the data, we were able to use some HTML/css to create some range sliders and dropdowns to create filters for race type and candidate names. This filters our dataframe and dynamically updates values in some key cards and a contributions bar graph to the right (as shown in the image below)

![ex1](District%2024%20Election%20Dashboard%2006-2019/campaign_dashboard_1.PNG)

Furthermore, since elections are divided by districts, we utilized a geographic map of New York City along with the library pygeoj to create an interactive heat map. After hovering on a zip code, the summary statistics and time series of contributions for that zip code are shown in the graphs to the right of the map.

![ex2](District%2024%20Election%20Dashboard%2006-2019/campaign_dashboard_2.PNG)



![ex3](District%2024%20Election%20Dashboard%2006-2019/campaign_dashboard_3.PNG)
