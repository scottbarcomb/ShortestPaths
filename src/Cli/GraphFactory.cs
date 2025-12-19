using Core;
using IO.Snap;

namespace Cli
{
    public static class GraphFactory
    {
        public static IGraph Create(GraphConfig cfg)
        {
            return cfg.Type switch
            {
                "grid" => new GridGraph(cfg.Width, cfg.Height, cfg.Blocked?.Select(b => (b[0], b[1]))),
                "snap" => SnapCombinedLoader.Load(cfg.Path),
                _ => throw new ArgumentException($"Unknown graph type '{cfg.Type}'")
            };
        }
    }
}
