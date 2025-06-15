import matplotlib.pyplot as plt
import networkx as nx
import time

# Исходный граф
edges = [
    ('A', 'B', 1),
    ('A', 'C', 3),
    ('B', 'C', 2),
    ('C', 'D', 4),
    ('B', 'D', 5)
]

# Построение графа
G = nx.Graph()
for u, v, w in edges:
    G.add_edge(u, v, weight=w)

# Визуализация графа на каждом шаге
def draw_graph(step, mst_edges, skipped_edges):
    pos = nx.spring_layout(G, seed=42)
    plt.figure(figsize=(8, 6))
    nx.draw_networkx_nodes(G, pos, node_color='lightblue', node_size=700)
    nx.draw_networkx_labels(G, pos)
    nx.draw_networkx_edges(G, pos, edgelist=G.edges(), edge_color='gray', width=1, style='dotted')
    labels = nx.get_edge_attributes(G, 'weight')
    nx.draw_networkx_edge_labels(G, pos, edge_labels=labels)

    if mst_edges:
        nx.draw_networkx_edges(G, pos, edgelist=mst_edges, edge_color='green', width=2)
    if skipped_edges:
        nx.draw_networkx_edges(G, pos, edgelist=skipped_edges, edge_color='red', width=2, style='dashed')

    plt.title(f"Шаг {step}: Построение MST")
    plt.axis('off')
    plt.show()

# Алгоритм Краскала с визуализацией
parent = {}
def find(v):
    while parent[v] != v:
        v = parent[v]
    return v

def union(u, v):
    parent[find(v)] = find(u)

for node in G.nodes():
    parent[node] = node

sorted_edges = sorted(edges, key=lambda x: x[2])
mst = []
skipped = []
step = 1

for u, v, w in sorted_edges:
    if find(u) != find(v):
        union(u, v)
        mst.append((u, v))
    else:
        skipped.append((u, v))
    draw_graph(step, mst, skipped)
    step += 1
    time.sleep(0.5)

print("Минимальное остовное дерево:")
for u, v in mst:
    print(f"{u} - {v}")
print("Суммарный вес:", sum(G[u][v]['weight'] for u, v in mst))
