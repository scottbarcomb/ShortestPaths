namespace Core;

public interface IMetricsSink
{
    void OnNodeExpanded(int v);
    void OnEdgeRelax(int u, int v, float oldCost, float newCost);
    void OnFrontierSize(int size);
    void OnFinish(float runtimeMs, float pathCost, int pathLen);
}

public sealed class RunMetrics : IMetricsSink
{
    public long NodesExpanded { get; private set; }
    public long EdgesRelaxed { get; private set; }
    public long MaxFrontier { get; private set; }
    public float RuntimeMs { get; private set; }
    public float PathCost { get; private set; }
    public int PathLength { get; private set; }

    public void OnNodeExpanded(int v) => NodesExpanded++;
    public void OnEdgeRelax(int u, int v, float oldCost, float newCost) => EdgesRelaxed++;
    public void OnFrontierSize(int size) => MaxFrontier = Math.Max(MaxFrontier, size);
    public void OnFinish(float runtimeMs, float pathCost, int pathLen)
    { RuntimeMs = runtimeMs; PathCost = pathCost; PathLength = pathLen; }
    public override string ToString() => 
        $"NodesExpanded: {NodesExpanded}, EdgesRelaxed: {EdgesRelaxed}, MaxFrontier: {MaxFrontier}, " +
        $"RuntimeMs: {RuntimeMs:F3}, PathCost: {PathCost}, PathLength: {PathLength}";
}
