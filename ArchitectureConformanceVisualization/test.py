import networkx as nx
import pandas as pd
import altair as alt


# Create a directed graph with weights
G = nx.DiGraph()
G.add_weighted_edges_from([(1, 2, 1.5), (2, 3, 2.0), (3, 4, 2.5), (4, 1, 3.0), (1, 3, 1.0)])

# Extract nodes and edges
nodes, edges = G.nodes(), G.edges(data=True)

# Create a DataFrame for nodes
nodes_df = pd.DataFrame(nodes, columns=['id'])

# Create a DataFrame for edges
edges_df = pd.DataFrame([(u, v, d['weight']) for u, v, d in edges], columns=['source', 'target', 'weight'])

# Create a chart for nodes
nodes_chart = alt.Chart(nodes_df).mark_circle(size=100).encode(
    x=alt.X('id:O', axis=alt.Axis(title='Node ID')),
    y=alt.Y('id:O', axis=alt.Axis(title='Node ID')),
    tooltip=['id']
)

# Create a chart for edges
edges_chart = alt.Chart(edges_df).mark_line().encode(
    x='source:O',
    x2='target:O',
    y='source:O',
    y2='target:O',
    size=alt.Size('weight', legend=None),
    tooltip=['source', 'target', 'weight']
)

# Combine the charts
graph_chart = nodes_chart + edges_chart

# Display the chart
graph_chart.show()