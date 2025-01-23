import networkx as nx
import matplotlib.pyplot as plt
import argparse
import json
import re
import plotly.graph_objects as go
import numpy as np
import seaborn as sns
import mplcursors


parser = argparse.ArgumentParser("arg_parser")
parser.add_argument("json", help="JSON file with mapped relationships", type=str)
args = parser.parse_args()
jsonFile = args.json
sns.set_theme(style="whitegrid")

def read_json(jsonFile):
    with open(jsonFile) as file:
        data = json.load(file)
    return data

def get_nodes(data):
    nodes = []
    for item in data:
        nodes.append(item["namespaceRegex"])
    return nodes

def get_nodes_mapping(data):
    nodes = {}
    for i, item in enumerate(data):
        nodes[item["namespaceRegex"]] = i
    return nodes

def get_expected_architecture(data, nodes):
    edges = []
    for item in data:
        for requiredReference in item["mustReference"]:
            nodes_list = list(nodes.keys())
            matched_nodes_regex = list(filter(lambda x: re.match(requiredReference, x), nodes_list))
            best_match = sorted(matched_nodes_regex, key=lambda x: len(x), reverse=True)[0]

            if best_match:
                edges.append((item["namespaceRegex"], best_match))
    return edges

def get_actual_architecture(data, nodes):
    edges = []
    for item in data:
        for divergence in item["divergences"]:
            nodes_list = list(nodes.keys())
            matched_nodes_regex = list(filter(lambda x: re.match(divergence, x), nodes_list))
            best_match = sorted(matched_nodes_regex, key=lambda x: len(x), reverse=True)[0]
            divergence_count = item["divergences"].count(best_match)

            if best_match:
                edges.append((item["namespaceRegex"], best_match, int(divergence_count)))
    return edges

def scale_positions(pos, scale_factor=0.5):
    center = np.array([0.5, 0.5])
    for key in pos:
        pos[key] = center + scale_factor * (pos[key] - center)
    return pos

def add_subplot_border(ax, width=1, color=None ):

    fig = ax.get_figure()

    # Convert bottom-left and top-right to display coordinates
    x0, y0 = ax.transAxes.transform((0, 0))
    x1, y1 = ax.transAxes.transform((1, 1))

    # Convert back to Axes coordinates
    x0, y0 = ax.transAxes.inverted().transform((x0, y0))
    x1, y1 = ax.transAxes.inverted().transform((x1, y1))

    rect = plt.Rectangle(
        (x0, y0), x1-x0, y1-y0,
        color=color,
        transform=ax.transAxes,
        zorder=-1,
        lw=2*width+1,
        fill=None,
    )
    fig.patches.append(rect)



data = read_json(jsonFile)

G_expected = nx.DiGraph()
G_actual = nx.DiGraph()

nodes = get_nodes(data)
mapped_nodes = get_nodes_mapping(data)
G_expected.add_nodes_from(nodes)
G_actual.add_nodes_from(nodes)


expected = get_expected_architecture(data, G_expected.nodes)
G_expected.add_edges_from(expected)

actual = get_actual_architecture(data, G_actual.nodes)
G_actual.add_weighted_edges_from(actual)

fig, (ax1, ax2) = plt.subplots(1, 2, figsize=(15, 7))


# Draw the graph
ax1.set_title("Expected Architecture")
add_subplot_border(ax1, 0.5, color="black")

pos1 = nx.spring_layout(G_expected)
pos1 = scale_positions(pos1)
# nx.draw(G_expected, pos, with_labels=False, node_color='lightblue',
#         node_size=1000, font_size=10, font_weight='bold', arrows=True, node_shape = "o", )

nx.draw(G_expected, pos1, ax=ax1, with_labels=False, node_size=700, arrows=True, node_shape="o")
edge_labels = nx.get_edge_attributes(G_expected, 'weight')
nx.draw_networkx_edge_labels(G_expected, pos1, edge_labels=edge_labels, ax=ax1)
nodes = nx.draw_networkx_nodes(G_expected, pos1, node_size=600, ax=ax1)
mplcursors.cursor(nodes, hover=True).connect("add", lambda sel: sel.annotation.set_text(list(G_expected.nodes)[sel.index]))

# Add border to the second subplot
for spine in ax1.spines.values():
    spine.set_edgecolor('black')
    spine.set_linewidth(2)

ax2.set_title("Actual Architecture")
add_subplot_border(ax2, 0.5, color="black")

pos2 = nx.spring_layout(G_actual)
pos2 = scale_positions(pos2)

nx.draw(G_actual, pos2, ax=ax2, with_labels=False, node_size=700, arrows=True, node_shape="o")
edge_labels = nx.get_edge_attributes(G_actual, 'weight')
nx.draw_networkx_edge_labels(G_actual, pos2, edge_labels=edge_labels, ax=ax2)
nodes = nx.draw_networkx_nodes(G_actual, pos2, node_size=600, ax=ax2)
mplcursors.cursor(nodes, hover=True, ).connect("add", lambda sel: sel.annotation.set_text(list(G_actual.nodes)[sel.index]))

# Add border to the second subplot
for spine in ax2.spines.values():
    spine.set_edgecolor('black')
    spine.set_linewidth(2)

# plt.tight_layout()
plt.show()