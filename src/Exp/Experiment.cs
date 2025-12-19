using Core;

namespace Exp
{
    public sealed class ExperimentConfig
    {
        public required string GraphName { get; init; } // name of the graph
        public required string GraphType { get; init; } // the type of graph (grid, SNAP)
        public int Trials { get; init; } = 100;
        public int Seed { get; init; } = 42; // random seed for source/destination 
        // public bool Directed { get; init; } = true; // treat graph as directed
        // public bool Weighted { get; init; } = false; // treat graph as weighted
        public int NumEdges { get; init; } = 0; // number of edges in the graph
        public int? Src { get; init; } = null; // optional fixed source vertex
        public int? Dst { get; init; } = null; // optional fixed destination vertex
        public string? Heuristic { get; init; } = null; // "Zero" or "Embedding" (Zero by default)
        public string? EmbeddingPath { get; init; } = null; // required if Heuristic is "Embedding"
    }

    public sealed class ExperimentRunner
    {
        private readonly ExperimentConfig _config;
        private readonly IGraph _graph;
        private readonly IShortestPathAlgorithm _algorithm;
        private readonly IHeuristic? _heuristic;

        public ExperimentRunner(ExperimentConfig config, IGraph graph, IShortestPathAlgorithm algorithm, IHeuristic? heuristic)
        {
            _config = config;
            _graph = graph;
            _algorithm = algorithm;
            _heuristic = heuristic;
        }

        // This function runs each trial (num trials defined in config) with a random start and goal node
        // using the defined graph, algorithm, and heuristic
        public IEnumerable<RunResult> RunAll()
        {
            var rng = new Random(_config.Seed);
            
            for (int i = 0; i < _config.Trials; i++)
            {
                int src = _config.Src ?? rng.Next(_graph.VertexCount);
                int dst = _config.Dst ?? rng.Next(_graph.VertexCount);
                while (dst == src) dst = rng.Next(_graph.VertexCount); // Ensure src != dst

                var metrics = new RunMetrics();
                var path = _algorithm.Compute(_graph, src, dst, metrics, _heuristic);
                // Console.Write("\r{0}%        ");
                // Console.Write($"{i + 1}/{_config.Trials}");

                yield return new RunResult(
                    RunId: i,
                    Timestamp: DateTime.UtcNow,
                    Algorithm: _algorithm.Name,
                    Heuristic: _heuristic?.GetType().Name ?? "none",
                    GraphName: _config.GraphName,
                    GraphType: _config.GraphType,
                    Directed: _graph.Directed,
                    Weighted: _graph.Weighted,
                    NumVertices: _graph.VertexCount,
                    NumEdges: _config.NumEdges,
                    Source: src,
                    Target: dst,
                    Path: path,
                    Metrics: metrics
                );
            }
        }
    }
}
