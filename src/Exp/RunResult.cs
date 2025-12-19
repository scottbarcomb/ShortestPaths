using Core;

namespace Exp
{
    // This is the data the experiment runner yields after every trial run
    public sealed record RunResult(
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
        Core.Path Path,
        RunMetrics Metrics
    );
}
