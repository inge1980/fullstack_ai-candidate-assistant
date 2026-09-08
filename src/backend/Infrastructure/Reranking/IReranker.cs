using Infrastructure.Embeddings;

namespace Infrastructure.Reranking;

public interface IReranker
{
    Task<IReadOnlyList<RerankResult>> RerankAsync(
        string query,
        IReadOnlyList<SearchResult> candidates,
        int limit = 5);
}