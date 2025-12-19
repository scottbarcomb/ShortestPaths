using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exp
{
    // This class takes each run result ran by the experiment runner and turns it into
    // a CSV-writable friendly object
    public static class ResultMapper
    {
        public static RunRow ToRunRow(RunResult r)
        {
            return new RunRow(
                RunId: r.RunId,
                Timestamp: r.Timestamp,
                Algorithm: r.Algorithm,
                Heuristic: r.Heuristic,
                GraphName: r.GraphName,
                GraphType: r.GraphType,
                Directed: r.Directed,
                Weighted: r.Weighted,
                NumVertices: r.NumVertices,
                NumEdges: r.NumEdges,
                Source: r.Source,
                Target: r.Target,
                PathFound: r.Path.Found,
                PathCost: r.Path.Found ? r.Path.TotalCost : float.NaN, // path cost or NaN if no path found
                PathLength: r.Path.Found ? r.Path.Length : 0, // path length or 0 if no path found
                NodesExpanded: r.Metrics.NodesExpanded,
                EdgesRelaxed: r.Metrics.EdgesRelaxed,
                MaxFrontier: r.Metrics.MaxFrontier,
                RuntimeMs: r.Metrics.RuntimeMs
            );
        }
    }
}
