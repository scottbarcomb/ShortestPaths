namespace IO.Embeddings
{
    // This class takes in the embedding file and turns it into the low-dimensional array to be fed
    // to the embedding heuristic class
    public static class EmbeddingLoader
    {
        public static float[][] Load(string path, IReadOnlyDictionary<int, int> originalToInternalId, int embeddingDim)
        {
            var embeddings = new float[originalToInternalId.Count][];
            foreach (var kv in originalToInternalId)
                embeddings[kv.Value] = new float[embeddingDim];

            foreach (var line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line) || line[0] == '#')
                    continue;

                var parts = line.Split(' ', '\t', ',');
                int originalId = int.Parse(parts[0]);

                if (!originalToInternalId.TryGetValue(originalId, out int internalId))
                    continue; // node not in graph

                var vec = new float[embeddingDim];
                for (int i = 0; i < embeddingDim; i++)
                    vec[i] = float.Parse(parts[i + 1]);

                embeddings[internalId] = vec;
            }

            return embeddings;
        }
    }
}
