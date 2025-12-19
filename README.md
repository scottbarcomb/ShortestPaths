### Usage:
`dotnet run --project src/Cli --config configs/<config_file>.json`

### Config files:
To run this project, you must use a configuration file (Two example ones are provided in ./configs).
Config files have three main sections: `graph`, `experiment`, and `output`.

`graph` takes a `type`, and depending on this `type`, other parameters too.

Graph type: `grid`; Required parameters: `width` and `height`; Optional parameter: `blocked`(an array of untraversable nodes in the grid)

Graph type: `snap`; Required parameter: `path` (filesystem path to the dataset - ./datasets/facebook.txt)



`experiment` always takes the parameters `graphName`, `graphType`, `algorithm`, and `heuristic`.

`graphName`: whatever string you like;
`graphType`: whatever string you like;
`algorithm`: can be 'BFS', 'Dijkstra', or 'AStar'. The spelling is case sensitive;
`heuristic`: can be 'none', 'manhattan', or 'embedding_cosine'. 'manhattan' only functions with grid graph types, and 'embedding_cosine' only function with snap graph types.

`embedding_cosine` heuristic must take two addtional config paramaters: `embeddingPath` (./embeddings/<embedding>.emb) and `embeddingDim` (if using the uploaded embedding, this must be set to 128, as it is in the example config file in ./configs/).



`output` holds one parameter: `runsCsv`, which needs a specified output path to write the experiment data.

### Example config file:
```
{
  "graph": {
    "type": "grid",
    "width": 10,
    "height": 6,
    "blocked": [[3,1],[3,2],[3,3],[3,4],[6,0],[6,1],[6,2]]
  },
  "experiment": {
    "graphName": "grid_10x6",
    "graphType": "grid",
    "algorithm": "AStar",
    "heuristic": "manhattan",
    "trials": 20,
    "seed": 42
  },
  "output": {
    "runsCsv": "results/runs.csv"
  }
}
```
