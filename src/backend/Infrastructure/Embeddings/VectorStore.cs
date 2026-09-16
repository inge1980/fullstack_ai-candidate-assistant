// Embedding to vector

using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Infrastructure.Documents;
using Npgsql;
using Pgvector;

namespace Infrastructure.Embeddings;

public class VectorStore
{
    private readonly NpgsqlDataSource _dataSource;

    private readonly string _connectionString;

    public VectorStore(string connectionString)
    {
        _connectionString =
            connectionString
            ?? throw new InvalidOperationException(
                "Postgres connection string is missing.");

        var builder =
            new NpgsqlDataSourceBuilder(_connectionString);

        builder.UseVector();

        _dataSource = builder.Build();
    }

    public async Task InsertAsync(
        DocumentChunk chunk,
        float[] embedding)
    {
        if (embedding.Length != 384)
        {
            throw new ArgumentException(
                $"Expected a 384-dimensional embedding, but received {embedding.Length} dimensions.",
                nameof(embedding));
        }

        await using var db =
            await _dataSource.OpenConnectionAsync();

        await using var cmd =
            new NpgsqlCommand(
                """
                INSERT INTO document_chunks
                (
                    id,
                    source,
                    heading_path,
                    semantic_type,
                    content,
                    metadata,
                    embedding
                )
                VALUES
                (
                    @id,
                    @source,
                    @heading_path,
                    @semantic_type,
                    @content,
                    @metadata,
                    @embedding
                )
                ON CONFLICT (id)
                DO UPDATE SET
                    source = EXCLUDED.source,
                    heading_path = EXCLUDED.heading_path,
                    semantic_type = EXCLUDED.semantic_type,
                    content = EXCLUDED.content,
                    metadata = EXCLUDED.metadata,
                    embedding = EXCLUDED.embedding;
                """,
                db);

        cmd.Parameters.AddWithValue(
            "id",
            chunk.Id);

        cmd.Parameters.AddWithValue(
            "source",
            chunk.Source);

        cmd.Parameters.AddWithValue(
            "heading_path",
            chunk.HeadingPath);

        cmd.Parameters.AddWithValue(
            "semantic_type",
            chunk.SemanticType);

        cmd.Parameters.AddWithValue(
            "content",
            chunk.Content);

        cmd.Parameters.AddWithValue(
            "metadata",
            NpgsqlTypes.NpgsqlDbType.Jsonb,
            JsonSerializer.Serialize(chunk.Metadata));

        cmd.Parameters.AddWithValue(
            "embedding",
            new Vector(embedding));

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IReadOnlyList<SearchResult>> SearchAsync(
        float[] embedding,
        int limit = 5)
    {
        if (embedding.Length != 384)
        {
            throw new ArgumentException(
                $"Expected a 384-dimensional embedding, but received {embedding.Length} dimensions.",
                nameof(embedding));
        }

        if (limit <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Search limit must be greater than zero.");
        }

        await using var db =
            await _dataSource.OpenConnectionAsync();

        await using var cmd =
            new NpgsqlCommand(
                """
                SELECT
                    id,
                    source,
                    heading_path,
                    semantic_type,
                    content,
                    metadata,
                    embedding,
                    1 - (embedding <=> @embedding) AS similarity
                FROM document_chunks
                WHERE embedding IS NOT NULL
                ORDER BY embedding <=> @embedding
                LIMIT @limit;
                """,
                db);

        cmd.Parameters.AddWithValue(
            "embedding",
            new Vector(embedding));

        cmd.Parameters.AddWithValue(
            "limit",
            limit);

        await using var reader =
            await cmd.ExecuteReaderAsync();

        return await ReadSearchResultsAsync(reader);
    }

    public async Task<IReadOnlyList<SearchResult>> SearchOverviewsMatchingOrganizationAsync(
        float[] embedding,
        IReadOnlyList<string> organizationTerms)
    {
        if (embedding.Length != 384)
        {
            throw new ArgumentException(
                $"Expected a 384-dimensional embedding, but received {embedding.Length} dimensions.",
                nameof(embedding));
        }

        var terms = organizationTerms
            .Where(term => !string.IsNullOrWhiteSpace(term) && term.Length >= 4)
            .Select(term => term.Trim().ToLowerInvariant())
            .Distinct()
            .ToArray();

        if (terms.Length == 0)
        {
            return [];
        }

        await using var db =
            await _dataSource.OpenConnectionAsync();

        await using var cmd =
            new NpgsqlCommand(
                """
                SELECT
                    id,
                    source,
                    heading_path,
                    semantic_type,
                    content,
                    metadata,
                    embedding,
                    1 - (embedding <=> @embedding) AS similarity
                FROM document_chunks
                WHERE embedding IS NOT NULL
                  AND lower(semantic_type) = 'overview'
                  AND EXISTS (
                      SELECT 1
                      FROM unnest(@terms) AS term
                      WHERE strpos(
                          lower(coalesce(metadata->>'organization', '')),
                          term) > 0
                  );
                """,
                db);

        cmd.Parameters.AddWithValue(
            "embedding",
            new Vector(embedding));

        cmd.Parameters.AddWithValue(
            "terms",
            terms);

        await using var reader =
            await cmd.ExecuteReaderAsync();

        return await ReadSearchResultsAsync(reader);
    }

    public async Task<IReadOnlyList<SearchResult>> SearchOverviewsMatchingTechnologiesAsync(
        float[] embedding,
        IReadOnlyList<string> technologySlugs)
    {
        if (embedding.Length != 384)
        {
            throw new ArgumentException(
                $"Expected a 384-dimensional embedding, but received {embedding.Length} dimensions.",
                nameof(embedding));
        }

        var slugs = technologySlugs
            .Where(slug => !string.IsNullOrWhiteSpace(slug))
            .Select(slug => slug.Trim().ToLowerInvariant())
            .Distinct()
            .ToArray();

        if (slugs.Length == 0)
        {
            return [];
        }

        await using var db =
            await _dataSource.OpenConnectionAsync();

        await using var cmd =
            new NpgsqlCommand(
                """
                SELECT
                    id,
                    source,
                    heading_path,
                    semantic_type,
                    content,
                    metadata,
                    embedding,
                    1 - (embedding <=> @embedding) AS similarity
                FROM document_chunks
                WHERE embedding IS NOT NULL
                  AND lower(semantic_type) = 'overview'
                  AND jsonb_typeof(COALESCE(metadata->'technologies', '[]'::jsonb)) = 'array'
                  AND EXISTS (
                      SELECT 1
                      FROM jsonb_array_elements_text(
                          COALESCE(metadata->'technologies', '[]'::jsonb)) AS tech
                      INNER JOIN unnest(@slugs) AS slug
                          ON lower(tech) = slug
                  );
                """,
                db);

        cmd.Parameters.AddWithValue(
            "embedding",
            new Vector(embedding));

        cmd.Parameters.AddWithValue(
            "slugs",
            slugs);

        await using var reader =
            await cmd.ExecuteReaderAsync();

        return await ReadSearchResultsAsync(reader);
    }

    private static async Task<List<SearchResult>> ReadSearchResultsAsync(
        NpgsqlDataReader reader)
    {
        var results =
            new List<SearchResult>();

        while (await reader.ReadAsync())
        {
            var metadataJson =
                reader.IsDBNull(5)
                    ? null
                    : reader.GetString(5);

            var metadata =
                string.IsNullOrWhiteSpace(metadataJson)
                    ? new Dictionary<string, object?>()
                    : JsonSerializer.Deserialize<Dictionary<string, object?>>(
                        metadataJson)
                      ?? new Dictionary<string, object?>();

            var chunk =
                new DocumentChunk
                {
                    Id = reader.GetString(0),
                    Source = reader.GetString(1),
                    HeadingPath = reader.GetString(2),
                    SemanticType = reader.GetString(3),
                    Content = reader.GetString(4),
                    Metadata = metadata,
                    Embedding = reader.IsDBNull(6)
                        ? default!
                        : reader.GetFieldValue<Vector>(6)
                };

            var similarity =
                reader.GetDouble(7);

            results.Add(
                new SearchResult
                {
                    Chunk = chunk,
                    VectorScore = similarity
                });
        }

        return results;
    }
}