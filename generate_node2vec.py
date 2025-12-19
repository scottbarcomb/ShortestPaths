import argparse
import networkx as nx
from node2vec import Node2Vec

def load_snap_edge_list(path: str) -> nx.Graph:
    """
    Load edge list from snap format (`u v` per line)
    Ignores comment lines starting with '#'
    :param path: file path to snap edge list
    :return: networkx graph
    """
    g = nx.Graph()
    with open(path, 'r') as f:
        for line in f:
            line = line.strip()
            if not line or line.startswith('#'):
                continue
            u, v = line.split(',')
            g.add_edge(int(u), int(v))
    return g

def main():
    parser = argparse.ArgumentParser(description='Generate node2vec embeddings from SNAP edge list')
    parser.add_argument('--input', required=True, help='input snap edge list file')
    parser.add_argument('--output', required=True, help='output embeddings file')
    parser.add_argument('--dim', type=int, default=128, help='embedding dimension')
    args = parser.parse_args()

    print('Loading graph...')
    g = load_snap_edge_list(args.input)
    print(f'Graph loaded: {g.number_of_nodes()} nodes and {g.number_of_edges()} edges')

    print('Running Node2Vec...')
    node2vec = Node2Vec(
        g,
        dimensions=args.dim,
        walk_length=80,
        num_walks=10,
        p=1,
        q=1,
        workers=4,
        seed=42,
    )

    model = node2vec.fit(
        window=args.window,
        min_count=1,
        batch_words=4,
        seed=args.seed
    )

    print(f'Saving embeddings to {args.output}')
    with open(args.output, 'w') as f:
        for node in g.nodes():
            vec = model.wv[str(node)]
            f.write(str(node))
            for x in vec:
                f.write(f' {x:.6f}')
            f.write('\n')

    print('Done')

if __name__ == '__main__':
    main()