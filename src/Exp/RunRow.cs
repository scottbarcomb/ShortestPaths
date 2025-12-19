namespace Exp
{
    // This is the CSV-friendly formatted data as a single object
    public sealed record RunRow(
        int RunId,
        DateTime Timestamp,
        string Algorithm,
        string Heuristic,
        string GraphName,
        string GraphType,
        bool Directed,
        bool Weighted,
        int NumVertices,
        int NumEdges,
        int Source,
        int Target,
        bool PathFound,
        float PathCost,
        int PathLength,
        long NodesExpanded,
        long EdgesRelaxed,
        long MaxFrontier,
        float RuntimeMs
    );
}
