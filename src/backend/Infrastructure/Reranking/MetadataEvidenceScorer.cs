using System.Text.RegularExpressions;
using Infrastructure.Documents;
using Infrastructure.Embeddings;

namespace Infrastructure.Reranking;

public class MetadataEvidenceScorer
{
    public SearchResult Score(
        string query,
        SearchResult result)
    {
        var queryTerms =
            Tokenize(query);

        var metadataScore =
            CalculateMetadataScore(
                queryTerms,
                result.Chunk);

        var evidenceScore =
            CalculateEvidenceScore(
                queryTerms,
                result.Chunk);

        result.MetadataScore =
            metadataScore;

        result.EvidenceScore =
            evidenceScore;

        result.CombinedScore =
            CalculateCombinedScore(
                result.VectorScore,
                metadataScore,
                evidenceScore);

        return result;
    }

    private double CalculateMetadataScore(
        HashSet<string> queryTerms,
        DocumentChunk chunk)
    {
        var metadataTerms =
            ExtractMetadataTerms(
                chunk.Metadata);

        if (metadataTerms.Count == 0)
        {
            return 0;
        }

        var matches =
            queryTerms
                .Count(metadataTerms.Contains);

        return Math.Min(
            1.0,
            (double)matches / Math.Max(1, queryTerms.Count));
    }

    private double CalculateEvidenceScore(
        HashSet<string> queryTerms,
        DocumentChunk chunk)
    {
        var contentTerms =
            Tokenize(chunk.Content);

        var headingPathTerms =
            Tokenize(chunk.HeadingPath);

        var contentMatches =
            queryTerms
                .Count(contentTerms.Contains);

        var headingPathMatches =
            queryTerms
                .Count(headingPathTerms.Contains);

        var contentScore =
            queryTerms.Count == 0
                ? 0
                : (double)contentMatches / queryTerms.Count;

        var headingPathScore  =
            queryTerms.Count == 0
                ? 0
                : (double)headingPathMatches / queryTerms.Count;

        return Math.Min(
            1.0,
            (contentScore * 0.7) +
            (headingPathScore * 0.3));
    }

    private double CalculateCombinedScore(
        double vectorScore,
        double metadataScore,
        double evidenceScore)
    {
        return
            (vectorScore * 0.70) +
            (metadataScore * 0.20) +
            (evidenceScore * 0.10);
    }

    private HashSet<string> ExtractMetadataTerms(
        Dictionary<string, object?> metadata)
    {
        var terms =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var key in new[]
        {
            "title",
            "organization",
            "role",
            "environment",
            "technologies",
            "concepts",
            "dependencies"
        })
        {
            if (!metadata.TryGetValue(
                    key,
                    out var value))
            {
                continue;
            }

            AddMetadataValue(
                value,
                terms);
        }

        return terms;
    }

    private void AddMetadataValue(
        object? value,
        HashSet<string> terms)
    {
        if (value == null)
        {
            return;
        }

        if (value is IEnumerable<object?> collection &&
            value is not string)
        {
            foreach (var item in collection)
            {
                AddMetadataValue(
                    item,
                    terms);
            }

            return;
        }

        var text =
            value.ToString();

        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        foreach (var token in Tokenize(text))
        {
            terms.Add(token);
        }
    }

    private HashSet<string> Tokenize(
        string text)
    {
        var normalized =
            text
                .ToLowerInvariant()
                .Replace("/", "-")
                .Replace(".", "-")
                .Replace("_", "-");

        var matches =
            Regex.Matches(
                normalized,
                @"[a-z0-9]+(?:-[a-z0-9]+)*");

        return matches
            .Select(match => match.Value)
            .Where(token => token.Length > 1)
            .ToHashSet(
                StringComparer.OrdinalIgnoreCase);
    }
}