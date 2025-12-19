using Core;

namespace Cli
{
    public sealed class ExperimentResources
    {
        public required IGraph Graph { get; init; }
        public float[][]? Embeddings { get; init; } // Optional embedding for the A* heuristic, must be present if using the embedding_cosine heuristic
    }
}
