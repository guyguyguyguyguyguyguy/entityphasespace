import pandas as pd 
import numpy as np 
import matplotlib.pyplot as plt 


df = pd.read_csv("text.csv", names= ['x', 'y'])

df.x = df.x.str.extract(r'(-?\d+)', expand=False).astype(float)
df.y = df.y.str.extract(r'(-?\d+)', expand=False).astype(float)

dfs = np.split(df, df[df.isnull().any(1)].index)
    
for i in range(1, len(dfs)):
    dfs[i] = dfs[i].iloc[1:]

for i in np.arange(0, len(dfs)-1, 2):

    ax = dfs[i+1].plot.scatter(x='x', y='y', c='DarkBlue', s=.5, zorder = 2, label='points', figsize=(10, 10))
    ax2 = dfs[i].plot.line(x='x', y='y', style='-', c = 'gold', label='hull', linewidth=2, zorder= 1, ax=ax)
    ax2.scatter(dfs[i].iloc[0].x, dfs[i].iloc[0].y, s= 50, alpha = .5, c= 'red', zorder= 3)
    
    plt.title("df number {}".format(i+1))
    plt.legend()
    plt.show()

dfs[3].to_csv("~/cSharp/convex_hull/test.csv")
