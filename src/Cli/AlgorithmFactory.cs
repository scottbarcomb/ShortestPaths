using Core;

namespace Cli
{
    public static class AlgorithmFactory
    {
        public static IShortestPathAlgorithm Create(string name) =>
            name switch
            {
                "BFS" => new Bfs(),
                "Dijkstra" => new Dijkstra(),
                "AStar" => new AStar(),
                _ => throw new ArgumentException($"Unkown algorithm '{name}'")
            };
    }
}
