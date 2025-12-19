using Core;

namespace Cli
{
    public static class HeuristicFactory
    {
        public static IHeuristic? Create(string? name, ExperimentResources res) =>
            name switch
            {
                null or "none" => null,
                "manhattan" when res.Graph is GridGraph gg => new ManhattanHeuristic(gg),
                "embedding_cosine" when res.Embeddings != null => new EmbeddingHeuristic(res.Embeddings),
                _ => throw new ArgumentException($"Unknown or incompatible heuristic '{name}'")
            };
    }
}
