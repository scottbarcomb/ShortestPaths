using System.Runtime.Serialization.Formatters;

namespace Core;

public interface IHeuristic
{
    // f(u): estimated cost from u to goal
    float Estimate(int u, int goal);
}

public sealed class ZeroHeuristic : IHeuristic
{
    public float Estimate(int u, int goal) => 0f;
}

public sealed class ManhattanHeuristic : IHeuristic
{
    private readonly GridGraph _g;

    public ManhattanHeuristic(GridGraph g) => _g = g;

    public float Estimate(int u, int goal)
    {
        var (x1, y1) = _g.Coords(u);
        var (x2, y2) = _g.Coords(goal);
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }
}

// Embedding-based cosine distance heuristic
public sealed class EmbeddingHeuristic : IHeuristic
{
    private readonly float[][] _emb; // _emb[v] is a vector
    public EmbeddingHeuristic(float[][] embeddings) => _emb = embeddings;

    public float Estimate(int u, int goal)
    {
        var a = _emb[u]; var b = _emb[goal];
        double dot = 0, na = 0, nb = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            na += a[i] * a[i];
            nb += b[i] * b[i];
        }
        var cos = dot / (Math.Sqrt(na) * Math.Sqrt(nb) + 1e-12);
        return (float)(1.0 - cos); // distance like
    }
}
