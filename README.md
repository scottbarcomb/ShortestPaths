### Usage:
`dotnet run --project src/Cli --config configs/<config_file>.json`

### Config files:
To run this project, you must use a configuration file (Two example ones are provided in ./configs).
Config files have three main section: `graph`, `experiment`, and `output`.

`graph` takes a `type`, and depending on this `type`, other parameters too.

Graph type: `grid`
Required parameters: `width` and `height`
Optional parameter: `blocked`(an array of untraversable nodes in the grid)

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
