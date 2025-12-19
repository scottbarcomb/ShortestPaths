using Core;

namespace IO.Snap
{
    public static class SnapCombinedLoader
    {
        public static IGraph Load(string path)
        {
            // The SNAP (Stanford Large Network Dataset Collection) combined social network graphs
            // are represented as `.txt` files with rows of (u, v) node edges.

            // Parse edges
            var edges = new List<(int u, int v)>();
            var idMap = new Dictionary<int, int>();

            if (path == null)
                throw new ArgumentNullException("No filepath to SNAP dataset found in config file.");

            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line) || line[0] == '#')
                    continue;

                var parts = line.Split(' ', '\t', ',');
                int uRaw = int.Parse(parts[0]);
                int vRaw = int.Parse(parts[1]);

                edges.Add((uRaw, vRaw));

                if (!idMap.ContainsKey(uRaw))
                    idMap[uRaw] = idMap.Count;
                if (!idMap.ContainsKey(vRaw))
                    idMap[vRaw] = idMap.Count; // bi-directional edges (undirected)
            }

            int n = idMap.Count;

            // Build adjacency lists
            var adj = new List<int>[n];
            for (int i = 0; i < n; i++)
                adj[i] = new List<int>();

            foreach (var (uRaw, vRaw) in edges)
            {
                int u = idMap[uRaw];
                int v = idMap[vRaw];

                adj[u].Add(v);
                adj[v].Add(u); // bi-lateral edge (undirected)
            }

            // Convert to CSR graph
            n = adj.Length;
            var rowPtr = new int[n + 1];
            int edgeCount = 0;

            for (int i = 0; i < n; i++)
            {
                rowPtr[i] = edgeCount;
                edgeCount += adj[i].Count;
            }
            rowPtr[n] = edgeCount;

            var colIdx = new int[edgeCount];
            int k = 0;
            for (int i = 0; i < n; i++)
                foreach (var v in adj[i])
                    colIdx[k++] = v;

            return new CsrGraph(n, rowPtr, colIdx, null, false, idMap);
        }
    }
}
