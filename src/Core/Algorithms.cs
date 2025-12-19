using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Core
{
    public interface IShortestPathAlgorithm
    {
        string Name { get; }
        Path Compute(IGraph g, int src, int dst, IMetricsSink? metrics = null, IHeuristic? h = null);
    }

    public sealed class Bfs : IShortestPathAlgorithm
    {
        public string Name => "BFS";
        public Path Compute(IGraph g, int src, int dst, IMetricsSink? metrics = null, IHeuristic? _ = null)
        {
            // BFS implementation
            // record metrics?.OnNodeExpanded(v) etc.
            // returns Path or NoPath (if not found - see Core/Path.cs)

            int numVertices = g.VertexCount;
            var prev = new int[numVertices]; // To reconstruct the path
            var dist = new int[numVertices]; // Distance from source
            Array.Fill(prev, -1);
            Array.Fill(dist, int.MaxValue);

            var queue = new Queue<int>();
            int frontierSize = 0;

            // Define the push and pop methods for the queue
            void Push(int v) // Enqueue and update frontier size metric
            {
                queue.Enqueue(v);
                frontierSize++;
                metrics?.OnFrontierSize(frontierSize); // Update frontier size metric
            }

            int Pop() // Dequeue and update frontier size metric
            {
                var v = queue.Dequeue();
                frontierSize--;
                metrics?.OnFrontierSize(frontierSize); // Update frontier size metric
                return v;
            }

            var stopwatch = Stopwatch.StartNew(); // Start timing
            dist[src] = 0;
            Push(src);

            while (queue.Count > 0)
            {
                int u = Pop();
                metrics?.OnNodeExpanded(u);

                if (u == dst) break; // Found the destination

                foreach (var v in g.Neighbors(u))
                {
                    if (dist[v] != int.MaxValue) continue; // Already visited
                    prev[v] = u; // Set predecessor
                    dist[v] = dist[u] + 1; // Update distance
                    metrics?.OnEdgeRelax(u, v, float.PositiveInfinity, dist[v]);
                    Push(v); // Enqueue neighbor
                }
            }
            stopwatch.Stop(); // Stop timing

            Path path;
            if (dist[dst] == int.MaxValue)
            {
                path = Path.NoPath; // No path found
            }
            else
            {
                var vertices = Reconstruct(prev, src, dst);
                path = new Path(vertices, dist[dst]); // Cost is the distance in unweighted graph
            }

            metrics?.OnFinish((float)stopwatch.Elapsed.TotalMilliseconds, path.TotalCost, path.Vertices.Count);
            return path;
        }

        private static IReadOnlyList<int> Reconstruct(int[] prev, int src, int dst)
        {
            if (src == dst) return new[] { src }; // Base case: path from src to src
            var stack = new Stack<int>();
            for (int v = dst; v != -1; v = prev[v])
            {
                stack.Push(v);
            }
            if (stack.Count == 0 || stack.Peek() != src)
            {
                return Array.Empty<int>(); // No path found due to an empty stack or src not at the top
            }
            return stack.ToArray(); // Convert stack to array for the path
        }
    }

    public sealed class Dijkstra : IShortestPathAlgorithm
    {
        public string Name => "Dijkstra";
        public Path Compute(IGraph g, int src, int dst, IMetricsSink? metrics = null, IHeuristic? h = null)
        {
            // priority_queue<(cost, int)>, dist[], prev[]
            // record metrics?.OnNodeExpanded(v) etc.
            // returns Path or NoPath (if not found)

            int n = g.VertexCount;

            var dist = new float[n];
            var prev = new int[n];
            Array.Fill(dist, float.PositiveInfinity);
            Array.Fill(prev, -1);

            // (vertex, priority) = distance
            var pq = new PriorityQueue<int, float>();
            int frontierSize = 0;

            void Push(int v, float priority)
            {
                pq.Enqueue(v, priority);
                frontierSize++;
                metrics?.OnFrontierSize(frontierSize);
            }

            int Pop(out float priority)
            {
                pq.TryDequeue(out int v, out priority);
                frontierSize--;
                metrics?.OnFrontierSize(frontierSize);
                return v;
            }

            var stopwatch = Stopwatch.StartNew();

            dist[src] = 0f;
            Push(src, 0f);

            while (pq.Count > 0)
            {
                int u = Pop(out float du);

                // Skip old queue entries
                if (du > dist[u])
                    continue;

                metrics?.OnNodeExpanded(u);

                // Check end condition
                if (u == dst)
                    break;

                foreach (var v in g.Neighbors(u))
                {
                    float w = g.Weighted ? g.GetWeight(u, v) : 1f;
                    float alt = dist[u] + w; // get current neighbor dist cost

                    if (alt < dist[v])
                    {
                        float old = dist[v];
                        dist[v] = alt;
                        prev[v] = u;

                        metrics?.OnEdgeRelax(u, v, old, alt);
                        Push(v, alt);
                    }
                }
            }

            stopwatch.Stop();

            Path path;
            if (float.IsPositiveInfinity(dist[dst]))
            {
                path = Path.NoPath;
            }
            else
            {
                var vertices = Reconstruct(prev, src, dst);
                path = new Path(vertices, dist[dst]);
            }

            metrics?.OnFinish((float)stopwatch.Elapsed.TotalMilliseconds, path.TotalCost, path.Vertices.Count);

            return path;
        }


        private static IReadOnlyList<int> Reconstruct(int[] prev, int src, int dst)
        {
            if (src == dst)
                return new[] { src };

            var stack = new Stack<int>();
            for (int v = dst; v != -1; v = prev[v])
                stack.Push(v);

            if (stack.Count == 0 || stack.Peek() != src)
                return Array.Empty<int>();

            return stack.ToArray();
        }
    }

    public sealed class AStar : IShortestPathAlgorithm
    {
        public string Name => "A*";
        public Path Compute(IGraph g, int src, int dst, IMetricsSink? metrics = null, IHeuristic? h = null)
        {
            // priority_queue<(f_score, int)>, g_score[], f_score[], prev[]
            // uses h?.Estimate(u, dst) for heuristic (uses ZeroHeuristic if h is null)

            h ??= new ZeroHeuristic(); // Default heuristic (no heuristic)
            
            int n = g.VertexCount;

            var dist = new float[n];
            var prev = new int[n];
            Array.Fill(dist, float.PositiveInfinity);
            Array.Fill(prev, -1);

            // priority = g(v) = h(v, dst)
            var pq = new PriorityQueue<int, float>();
            int frontierSize = 0;

            void Push(int v, float priority)
            {
                pq.Enqueue(v, priority);
                frontierSize++;
                metrics?.OnFrontierSize(frontierSize);
            }

            int Pop(out float priority)
            {
                pq.TryDequeue(out int v, out priority);
                frontierSize--;
                metrics?.OnFrontierSize(frontierSize);
                return v;
            }

            var stopwatch = Stopwatch.StartNew();

            dist[src] = 0f;
            Push(src, h.Estimate(src, dst));

            while (pq.Count > 0)
            {
                int u = Pop(out float fu);

                // Skip old queue entries
                float expected = dist[u] + h.Estimate(u, dst);
                if (fu > expected)
                    continue;

                metrics?.OnNodeExpanded(u);

                // Check end condition
                if (u == dst)
                    break;

                foreach (var v in g.Neighbors(u))
                {
                    float w = g.Weighted ? g.GetWeight(u, v) : 1f;
                    float alt = dist[u] + w; // get current neighbor dist cost

                    if (alt < dist[v])
                    {
                        float old = dist[v];
                        dist[v] = alt;
                        prev[v] = u;

                        metrics?.OnEdgeRelax(u, v, old, alt);

                        float priority = alt + h.Estimate(v, dst);
                        Push(v, priority);
                    }
                }
            }

            stopwatch.Stop();

            Path path;
            if (float.IsPositiveInfinity(dist[dst]))
            {
                path = Path.NoPath;
            }
            else
            {
                var vertices = Reconstruct(prev, src, dst);
                path = new Path(vertices, dist[dst]);
            }

            metrics?.OnFinish((float)stopwatch.Elapsed.TotalMilliseconds, path.TotalCost, path.Vertices.Count);

            return path;
        }


        private static IReadOnlyList<int> Reconstruct(int[] prev, int src, int dst)
        {
            if (src == dst)
                return new[] { src };

            var stack = new Stack<int>();
            for (int v = dst; v != -1; v = prev[v])
                stack.Push(v);

            if (stack.Count == 0 || stack.Peek() != src)
                return Array.Empty<int>();

            return stack.ToArray();
        }
    }
}
