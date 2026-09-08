using System.Diagnostics;
using Infrastructure.Embeddings;
using Infrastructure.Reranking;

namespace Application.Knowledge;

public sealed class KnowledgeRetrievalService(
    EmbeddingService embeddingService,
    VectorStore vectorStore,
    MetadataEvidenceScorer evidenceScorer)
    : IKnowledgeRetrievalService
{
    public async Task<KnowledgeRetrievalResult> RetrieveAsync(
        string query,
        int retrievalLimit = 10,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException(
                "Query cannot be empty.",
                nameof(query));
        }

        if (retrievalLimit <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(retrievalLimit),
                "Retrieval limit must be greater than zero.");
        }

        Console.WriteLine($"-------------------------------------------");
        var embeddingStopwatch = Stopwatch.StartNew();
        // Console.WriteLine("Generating query embedding...");
        var embedding =
            await embeddingService.Create(query);
        // Console.WriteLine($"Embedding dimensions: {embedding.Length}");
        embeddingStopwatch.Stop();
        Console.WriteLine($"[Timing] Query embedding: {embeddingStopwatch.ElapsedMilliseconds} ms");

        var vectorSearchStopwatch = Stopwatch.StartNew();
        // Console.WriteLine();
        // Console.WriteLine("Searching PostgreSQL...");
        var results =
            await vectorStore.SearchAsync(
                embedding,
                limit: retrievalLimit);
        vectorSearchStopwatch.Stop();
        Console.WriteLine($"[Timing] Vector search: {vectorSearchStopwatch.ElapsedMilliseconds} ms");

        foreach (var result in results)
        {
            evidenceScorer.Score(query, result);
        }

        var rankedResults =
            results
                .OrderByDescending(
                    result => result.CombinedScore)
                .Select(
                    result => new KnowledgeRetrievalItem(
                        Source: result.Chunk.Source,
                        Heading: result.Chunk.HeadingPath,
                        SemanticType: result.Chunk.SemanticType,
                        Content: result.Chunk.Content,
                        Metadata: result.Chunk.Metadata,
                        CombinedScore: result.CombinedScore,
                        VectorScore: result.VectorScore,
                        MetadataScore: result.MetadataScore,
                        EvidenceScore: result.EvidenceScore))
                .ToList();

        return new KnowledgeRetrievalResult(
            Items: rankedResults);
    }
}