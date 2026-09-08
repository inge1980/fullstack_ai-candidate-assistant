using Infrastructure.Embeddings;

namespace Infrastructure.Reranking;

public sealed class RerankResult
{
    public required SearchResult Result { get; init; }

    public double Score { get; init; }
}