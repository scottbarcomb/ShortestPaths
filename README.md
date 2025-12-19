### Usage:
`dotnet run --project src/Cli --config configs/<config_file>.json`

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