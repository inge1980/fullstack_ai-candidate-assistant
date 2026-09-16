using System.Diagnostics;
using System.Text.RegularExpressions;
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
        bool includeMatchingOrganizationOverviews = false,
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
        var embedding =
            await embeddingService.Create(query);
        embeddingStopwatch.Stop();
        Console.WriteLine($"[Timing] Query embedding: {embeddingStopwatch.ElapsedMilliseconds} ms");

        var vectorSearchStopwatch = Stopwatch.StartNew();
        var results =
            (await vectorStore.SearchAsync(
                embedding,
                limit: retrievalLimit))
            .ToList();
        vectorSearchStopwatch.Stop();
        Console.WriteLine($"[Timing] Vector search: {vectorSearchStopwatch.ElapsedMilliseconds} ms");

        if (includeMatchingOrganizationOverviews)
        {
            var organizationStopwatch = Stopwatch.StartNew();
            var organizationHits =
                await vectorStore.SearchOverviewsMatchingOrganizationAsync(
                    embedding,
                    OrganizationQueryTerms(query));
            organizationStopwatch.Stop();

            var existingIds = results
                .Select(result => result.Chunk.Id)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var added = 0;

            foreach (var hit in organizationHits)
            {
                if (existingIds.Add(hit.Chunk.Id))
                {
                    results.Add(hit);
                    added++;
                }
            }

            Console.WriteLine(
                $"[Timing] Organization overviews: {organizationStopwatch.ElapsedMilliseconds} ms added={added}");
        }

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

    private static IReadOnlyList<string> OrganizationQueryTerms(string query)
    {
        return Regex.Matches(query.ToLowerInvariant(), @"[a-z0-9]+(?:-[a-z0-9]+)*")
            .Select(match => match.Value)
            .Where(token => token.Length >= 4)
            .Distinct()
            .ToList();
    }
}