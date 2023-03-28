# Portfolio
Hi, my name's Peter. Here are some miscellaneous projects I've worked on. Please also check out my other repositories as well!

## Campaign Finance Dashboard

I worked on a campaign finance dashboard that collected public campaign contributions data and put it in an easy to digest dashboard for local politicians to use to gain an edge over their competition.

I made this alongside another colleague using Pandas to wrangle the data, creating visualizations using Plotly and then serving it to a Flask instance using Dash.

After cleaning the data, we were able to use some HTML/css to create some range sliders and dropdowns to create filters for race type and candidate names. This filters our dataframe and dynamically updates values in some key cards and a contributions bar graph to the right (as shown in the image below)

![ex1](District%2024%20Election%20Dashboard%2006-2019/campaign_dashboard_1.PNG)

Furthermore, since elections are divided by districts, we utilized a geographic map of New York City along with the library pygeoj to create an interactive heat map. After hovering on a zip code, the summary statistics and time series of contributions for that zip code are shown in the graphs to the right of the map.

![ex2](District%2024%20Election%20Dashboard%2006-2019/campaign_dashboard_2.PNG)

Afterwards, all of the data that has been filtered is exportable via the dash table for easy-to-access information

![ex3](District%2024%20Election%20Dashboard%2006-2019/campaign_dashboard_3.PNG)


## Behavioral Assessment of Healthcare Professionals

An assessment was done aiming to measure various cognitive traits within healthcare professions. I was fortunate enough to aggregate, visualize, and analyze the results of this survey of around 2000 professionals from:
 - Surgeons
 - Admins
 - Nurses
 - Operating Room Staff
 - General Physicians
 
After determining the weight of each question's answer I cleaned the data using Pandas and ran some exploratory analysis on the results. I was able to visualize some key results using Plotly and serving it to a Flask server and hosting that using Heroku for easy access to my client. Below are some of the visualizations:

![img1](Creative%20Science%20Dashboard/data/img1.PNG)
![img2](Creative%20Science%20Dashboard/data/img2.PNG)
![img3](Creative%20Science%20Dashboard/data/img3.PNG)

Participants were measured along 5 categories:
 - Cognitive Thinking
 - Altruism
 - Risk Averseness
 - Trust
