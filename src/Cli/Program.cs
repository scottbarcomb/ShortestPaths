using Cli;
using Core;
using Exp;
using IO.Embeddings;
using System.Text.Json;

internal static class Program
{
    // The main entry point for the program
    static int Main(string[] args)
    {
        if (args.Length < 2 || args[0] != "--config")
        {
            Console.Error.WriteLine("Usage: dotnet run --project src/Cli --config <path/to/config>");
            return 1;
        }

        var configPath = args[1];
        var config = LoadConfig(configPath);

        // Build graph
        var graph = GraphFactory.Create(config.Graph);

        // Load embedding (optional but required when using the embedding_cosine heuristic)
        float[][]? embeddings = null;
        if (config.Experiment.Heuristic == "embedding_cosine")
        {
            embeddings = EmbeddingLoader.Load(
                config.Experiment.EmbeddingPath,
                graph.OriginalToInternalId,
                config.Experiment.EmbeddingDim
            );
        }

        // Create experiment resource holding graph and embeddings
        var resources = new ExperimentResources
        {
            Graph = graph,
            Embeddings = embeddings // is nullable
        };

        // Build algorithm and heuristic
        var algorithm = AlgorithmFactory.Create(config.Experiment.Algorithm);
        var heuristic = HeuristicFactory.Create(config.Experiment.Heuristic, resources);

        // Create ExperimentRunner
        var runner = new ExperimentRunner(
            config: new Exp.ExperimentConfig
            {
                GraphName = config.Experiment.GraphName,
                GraphType = config.Experiment.GraphType,
                Trials = config.Experiment.Trials,
                Seed = config.Experiment.Seed,
                NumEdges = EstimateEdges(graph)
            },
            graph: graph,
            algorithm: algorithm,
            heuristic: heuristic
        );

        // Run and write experiment data to CSV
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(config.Output.RunsCsv)!); // Create directory/output if not already there

        using var writer = new CsvRunWriter(config.Output.RunsCsv);

        foreach (var result in runner.RunAll())
        {
            var row = ResultMapper.ToRunRow(result);
            writer.Write(row);
        }

        Console.WriteLine("Experiment completed.");
        return 0;
    }

    private static RootConfig LoadConfig(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<RootConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    private static int EstimateEdges(IGraph g)
    {
        int sum = 0;
        for (int u = 0; u < g.VertexCount; u++)
            sum += g.Neighbors(u).Length;
        return g.Directed ? sum : sum / 2; // Directed graph will always have half the edges of a non-directed graph
    }
}