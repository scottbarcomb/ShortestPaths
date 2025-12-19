using Exp;

namespace Cli
{
    public sealed class RootConfig
    {
        public required GraphConfig Graph { get; init; }
        public required ExperimentConfig Experiment { get; init; }
        public required OutputConfig Output { get; init; }
    }

    public sealed class GraphConfig
    {
        public required string Type { get; init; } // "grid" or "snap"
        public int Width { get; init; } // height and width only applicable for grid graphs
        public int Height { get; init; }
        public int[][]? Blocked { get; init; } // used if you want to add untraversable nodes in the grid graph
        public string? Path { get; init; } // path to the SNAP dataset in the filesystem
    }

    public sealed class ExperimentConfig
    {
        public required string GraphName { get; init; }
        public required string GraphType { get; init; }
        public required string Algorithm { get; init; } // BFS, Dijkstra, or AStar
        public string? Heuristic { get; init; }         // manhattan, embedded_cosine, or none
        public string? EmbeddingPath { get; init; }     // location of the embedding file in filesystem
        public int EmbeddingDim { get; init; } = 0;     // the embedding file used for this project had a dimension of 128
        public int Trials { get; init; } = 1;           // the number of experimental runs to do
        public int Seed { get; init; } = 42;            // the random seed, default 42
    }

    public sealed class OutputConfig
    {
        public required string RunsCsv { get; init; } // the filepath for the output CSV data file
    }
}
