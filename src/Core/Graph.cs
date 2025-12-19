namespace Core;

public interface IGraph
{
    int VertexCount { get; }
    bool Directed { get; }
    bool Weighted { get; }
    IReadOnlyDictionary<int, int>? OriginalToInternalId { get; }

    // Iterate neighbors of u (edge (u -> v) with optional weight w)
    ReadOnlySpan<int> Neighbors(int u);
    float GetWeight(int u, int v); // return 1f for unweighted
    bool HasEdge(int u, int v);
}

// Compact Sparse Row graph (int/int/float arrays)
public sealed class CsrGraph : IGraph
{
    // CSR: rowPtr[v]..rowPtr[v+1]-1 indexes into colIdx / weights
    private readonly int[] _rowPtr;
    private readonly int[] _colIdx;
    private readonly float[]? _w;
    public int VertexCount { get; }
    public bool Directed { get; }
    public bool Weighted => _w is not null;
    public IReadOnlyDictionary<int, int> OriginalToInternalId { get; }

    public CsrGraph(int vertexCount, int[] rowPtr, int[] colIdx, float[]? weights, bool directed, IReadOnlyDictionary<int, int> idMap)
    {
        OriginalToInternalId = idMap;
        VertexCount = vertexCount;
        _rowPtr = rowPtr; _colIdx = colIdx; _w = weights; Directed = directed;
    }

    public ReadOnlySpan<int> Neighbors(int u)
        => new ReadOnlySpan<int>(_colIdx, _rowPtr[u], _rowPtr[u + 1] - _rowPtr[u]);

    public float GetWeight(int u, int v)
    {
        // Simple linear scan inside the row (opt: binary search if sorted)
        for (int i = _rowPtr[u]; i < _rowPtr[u + 1]; i++)
            if (_colIdx[i] == v) return _w is null ? 1f : _w[i];
        return float.PositiveInfinity;
    }

    public bool HasEdge(int u, int v)
    {
        for (int i = _rowPtr[u]; i < _rowPtr[u + 1]; i++)
            if (_colIdx[i] == v) return true;
        return false;
    }
}

// 4-neighbor grid graph (up/down/left/right) with optional blocked cells
public sealed class GridGraph : IGraph
{
    private readonly int _w, _h;
    private readonly bool[] _blocked; // flattened 2D array
    private readonly int[] _scratchNeighbors = new int[4]; // reused with each call

    public GridGraph(int width, int height, IEnumerable<(int x, int y)>? blocked = null)
    {
        _w = width; _h = height;
        _blocked = new bool[_w * _h];
        if (blocked != null)
            foreach (var (x, y) in blocked)
                if (InBounds(x, y)) _blocked[Idx(x, y)] = true; // mark blocked cells
    }

    public int VertexCount => _w * _h;
    public bool Directed => false;
    public bool Weighted => false;
    public IReadOnlyDictionary<int, int>? OriginalToInternalId => null;

    public bool InBounds(int x, int y) => x >= 0 && x < _w && y >= 0 && y < _h;
    public int Idx(int x, int y) => y * _w + x;
    public (int x, int y) Coords(int id) => (id % _w, id / _w);
    public bool IsBlocked(int x, int y) => !InBounds(x, y) || _blocked[Idx(x, y)];

    public ReadOnlySpan<int> Neighbors(int u)
    {
        var (x, y) = Coords(u); // get (x,y) from id
        int n = 0;

        void TryAdd(int nx, int ny) // try to add neighbor
        {
            if (!IsBlocked(nx, ny))
                _scratchNeighbors[n++] = Idx(nx, ny); // add neighbor id
        }

        TryAdd(x, y - 1); // up
        TryAdd(x, y + 1); // down
        TryAdd(x - 1, y); // left
        TryAdd(x + 1, y); // right

        return new ReadOnlySpan<int>(_scratchNeighbors, 0, n); // return valid neighbors
    }

    public float GetWeight(int u, int v) => 1f; // unweighted graph

    public bool HasEdge(int u, int v)
    {
        foreach (var neighbor in Neighbors(u))
            if (neighbor == v) return true;
        return false;
    }
}
