namespace Core;

public sealed record Path(IReadOnlyList<int> Vertices, float TotalCost)
{
    public static Path NoPath => new(Array.Empty<int>(), float.PositiveInfinity);
    public int Length => Vertices.Count;
    public bool Found => !float.IsPositiveInfinity(TotalCost);
}
