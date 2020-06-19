import matplotlib.pyplot as plt
from random import *
from numpy import *
import sys
import plotly.graph_objects as go
import pandas as pd




# model parameters
a = 0.2; b = 0.5; c = 0.3;  e = 0.02
dt = 0.001; max_time = 1000

# initial time and populations
t = 0; x = 10.0; y = 100.0

# empty lists in which to store time and populations
t_list = []; x_list = []; y_list = []

# initialize lists
t_list.append(t); x_list.append(x); y_list.append(y)

while t < max_time:
    # calc new values for t, x, y
    t = t + dt
    x = x + (a*x - b*x*y)*dt
    y = y + (-c*y + e*x*y)*dt

    # store new values in lists
    t_list.append(t)
    x_list.append(x)
    y_list.append(y)




# Plot the results    
#p = plt.plot(t_list, x_list, 'r', t_list, y_list, 'g', linewidth = 2)

df = pd.DataFrame(list(zip(t_list, x_list, y_list)), columns =['Tiempo', 'Presas', 'Depredadores'])

fig = go.Figure()
fig.add_scatter(x=df['Tiempo'], y=df['Presas'], mode='lines', name="Presas")
fig.add_scatter(x=df['Tiempo'], y=df['Depredadores'], mode='lines', name="Depredadores")

fig.update_layout(
    legend=dict(
        x=0,
        y=1,
        traceorder="normal",
        font=dict(
            family="sans-serif",
            size=12,
            color="black"
        ),
        bgcolor="LightSteelBlue",
        bordercolor="Black",
        borderwidth=2
    )
)

fig.show()
