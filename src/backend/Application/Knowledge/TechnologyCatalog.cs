using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Application.Knowledge;

public static class TechnologyCatalog
{
    public const int RelatedFillCap = 3;
    public const int RelatedFamilySizeCap = 8;

    private static readonly Lazy<Taxonomy> Loaded =
        new(Load, LazyThreadSafetyMode.ExecutionAndPublication);

    public static IReadOnlyList<string> ResolveNamed(string? question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return [];
        }

        var normalized = NormalizeForMatch(question);
        var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var (phrase, slug) in Loaded.Value.PhrasesByLength)
        {
            if (ContainsPhrase(normalized, phrase))
            {
                found.Add(slug);
            }
        }

        return found
            .OrderBy(slug => slug, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static IReadOnlyList<string> SlugsToMerge(
        IReadOnlyList<string> named)
    {
        if (named.Count == 0)
        {
            return [];
        }

        var slugs = new HashSet<string>(named, StringComparer.OrdinalIgnoreCase);

        foreach (var slug in named)
        {
            foreach (var related in FamilySlugs(slug))
            {
                slugs.Add(related);
            }
        }

        return slugs
            .OrderBy(slug => slug, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static IReadOnlyList<string> FamilySlugs(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return [];
        }

        foreach (var family in Loaded.Value.Families)
        {
            if (!family.Slugs.Contains(slug))
            {
                continue;
            }

            if (family.Slugs.Count > RelatedFamilySizeCap)
            {
                return [];
            }

            return family.Slugs.ToList();
        }

        return [];
    }

    public static string? FamilyId(string slug)
    {
        foreach (var family in Loaded.Value.Families)
        {
            if (family.Slugs.Contains(slug)
                && family.Slugs.Count <= RelatedFamilySizeCap)
            {
                return family.Id;
            }
        }

        return null;
    }

    public static IReadOnlyList<string> RelatedFamilySlugsPresent(
        string? question,
        IReadOnlyList<KnowledgeRetrievalItem> items)
    {
        var named = ResolveNamed(question);
        if (named.Count == 0 || items.Count == 0)
        {
            return [];
        }

        var familyMembers = SlugsToMerge(named)
            .Where(slug => !named.Contains(slug, StringComparer.OrdinalIgnoreCase))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (familyMembers.Count == 0)
        {
            return [];
        }

        return items
            .SelectMany(ProjectSlugs)
            .Where(familyMembers.Contains)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(RelatedSlugOrder)
            .ThenBy(slug => slug, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static HashSet<string> ProjectSlugs(KnowledgeRetrievalItem item)
    {
        var slugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        object? value = null;
        foreach (var pair in item.Metadata)
        {
            if (string.Equals(pair.Key, "technologies", StringComparison.OrdinalIgnoreCase))
            {
                value = pair.Value;
                break;
            }
        }

        if (value is null)
        {
            return slugs;
        }

        foreach (var raw in EnumerateTechnologyValues(value))
        {
            AddProjectSlug(slugs, raw);
        }

        if (slugs.Count == 0)
        {
            var text = value is JsonElement json
                ? json.ToString()
                : value.ToString();
            foreach (var token in TokenizeTechnologyText(text))
            {
                AddProjectSlug(slugs, token);
            }
        }

        return slugs;
    }

    public static bool ProjectHasSlug(
        KnowledgeRetrievalItem item,
        string slug)
    {
        return ProjectSlugs(item)
            .Contains(NormalizeSlug(slug));
    }

    public static bool ProjectHasAnySlug(
        KnowledgeRetrievalItem item,
        IReadOnlyList<string> slugs)
    {
        if (slugs.Count == 0)
        {
            return false;
        }

        var project = ProjectSlugs(item);
        return slugs.Any(project.Contains);
    }

    public static bool ProjectHasEverySlug(
        KnowledgeRetrievalItem item,
        IReadOnlyList<string> slugs)
    {
        if (slugs.Count == 0)
        {
            return false;
        }

        var project = ProjectSlugs(item);
        return slugs.All(project.Contains);
    }

    public static string MatchInstruction(
        string? question,
        IReadOnlyList<KnowledgeRetrievalItem>? items)
    {
        var named = ResolveNamed(question);
        if (named.Count == 0)
        {
            return string.Empty;
        }

        if (items is null || items.Count == 0)
        {
            return
                $"Named technologies resolved from the question: {string.Join(", ", named)}. "
                + "No project Technologies field lists them. Do not claim they were used. "
                + "Do not invent projects, experience, or URLs. "
                + "If retrieved context is empty, say the experience is not in the project record.";
        }

        var exactFound = named
            .Where(slug => items.Any(item => ProjectHasSlug(item, slug)))
            .ToList();

        var missing = named
            .Where(slug => !exactFound.Contains(slug, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var relatedItems = items
            .Where(item => !ProjectHasAnySlug(item, named)
                && ProjectHasAnySlug(item, SlugsToMerge(missing)))
            .ToList();

        var relatedSlugs = relatedItems
            .SelectMany(ProjectSlugs)
            .Where(slug => SlugsToMerge(missing).Contains(slug)
                && !named.Contains(slug, StringComparer.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(slug => slug, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var parts = new List<string>
        {
            $"Named technologies resolved from the question: {string.Join(", ", named)}."
        };

        if (exactFound.Count > 0)
        {
            parts.Add(
                $"Exact Technologies matches in this context: {string.Join(", ", exactFound)}. Treat these as used.");
        }

        if (missing.Count > 0)
        {
            parts.Add(
                $"No project Technologies field lists: {string.Join(", ", missing)}. Do not claim those were used.");
        }

        if (relatedSlugs.Count > 0)
        {
            var familyIds = missing
                .Select(FamilyId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            parts.Add(
                "Related technologies in this context"
                + (familyIds.Any()
                    ? $" (family {string.Join(", ", familyIds)})"
                    : string.Empty)
                + $": {string.Join(", ", relatedSlugs)}. "
                + "These are related experience only. Say clearly that the named missing technology is not in the project record, then mention the related work. Never upgrade related to used.");
        }

        return string.Join(" ", parts);
    }

    public static string DisplayName(string slug)
    {
        var normalized = NormalizeSlug(slug);
        return normalized switch
        {
            "csharp" => "C#",
            "aspnet-core" => "ASP.NET Core",
            "next.js" => "Next.js",
            "sql-server" => "SQL Server",
            "docker-compose" => "Docker Compose",
            "container-registry" => "Container Registry",
            _ => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(
                normalized.Replace('-', ' '))
        };
    }

    public static string FormatMatchLine(
        KnowledgeRetrievalItem item,
        IReadOnlyList<string> named)
    {
        if (named.Count == 0)
        {
            return string.Empty;
        }

        if (ProjectHasAnySlug(item, named))
        {
            var matched = named
                .Where(slug => ProjectHasSlug(item, slug))
                .ToList();

            return $"Match: exact ({string.Join(", ", matched)})\n";
        }

        var missing = named
            .Where(slug => !ProjectHasSlug(item, slug))
            .ToList();
        var familySlugs = SlugsToMerge(missing)
            .Where(slug => !named.Contains(slug, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (!ProjectHasAnySlug(item, familySlugs))
        {
            return string.Empty;
        }

        var listed = ProjectSlugs(item)
            .Where(familySlugs.Contains)
            .OrderBy(slug => slug, StringComparer.OrdinalIgnoreCase);

        var familyIds = missing
            .Select(FamilyId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.OrdinalIgnoreCase);

        return "Match: related"
            + (familyIds.Any()
                ? $" (family {string.Join(", ", familyIds)})"
                : string.Empty)
            + $"; not used: {string.Join(", ", missing)}"
            + $"; listed: {string.Join(", ", listed)}\n";
    }

    private static Taxonomy Load()
    {
        var path = FindTaxonomyPath();
        if (path is null)
        {
            return Taxonomy.Empty;
        }

        return Parse(File.ReadAllText(path));
    }

    private static string? FindTaxonomyPath()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var copiedPath = Path.Combine(
                directory.FullName,
                "Prompts",
                "taxonomy",
                "technology-families.md");
            if (File.Exists(copiedPath))
            {
                return copiedPath;
            }

            var sourcePath = Path.Combine(
                directory.FullName,
                "src",
                "backend",
                "Application",
                "Prompts",
                "taxonomy",
                "technology-families.md");
            if (File.Exists(sourcePath))
            {
                return sourcePath;
            }

            directory = directory.Parent;
        }

        return null;
    }

    internal static Taxonomy Parse(string markdown)
    {
        var aliases = new Dictionary<string, List<string>>(
            StringComparer.OrdinalIgnoreCase);
        var families = new List<Family>();
        var section = "";

        foreach (var rawLine in markdown.Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith("<!--", StringComparison.Ordinal))
            {
                continue;
            }

            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                section = OnlyLetters(line[3..].Trim().ToLowerInvariant());
                continue;
            }

            if (!line.StartsWith("- ", StringComparison.Ordinal))
            {
                continue;
            }

            var bullet = line[2..].Trim();
            var split = bullet.IndexOf(':');
            if (split <= 0)
            {
                continue;
            }

            var key = NormalizeSlug(bullet[..split]);
            var values = bullet[(split + 1)..]
                .Split(
                    ',',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(NormalizeForMatch)
                .Where(value => value.Length > 0)
                .ToList();

            if (key.Length == 0)
            {
                continue;
            }

            if (section is "alias" or "aliases")
            {
                if (!aliases.TryGetValue(key, out var aliasPhrases))
                {
                    aliasPhrases = [];
                    aliases[key] = aliasPhrases;
                }

                aliasPhrases.Add(key);
                aliasPhrases.Add(key.Replace('-', ' '));
                aliasPhrases.AddRange(values);
            }
            else if (section is "family" or "families")
            {
                var slugs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var slug in values.Select(NormalizeSlug))
                {
                    if (slug.Length > 0)
                    {
                        slugs.Add(slug);
                    }
                }

                if (slugs.Count > 0)
                {
                    families.Add(new Family(key, slugs));
                }
            }
        }

        var phrasePairs = new List<(string Phrase, string Slug)>();
        foreach (var (slug, slugPhrases) in aliases)
        {
            foreach (var phrase in slugPhrases.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (phrase.Length >= 2)
                {
                    phrasePairs.Add((phrase, slug));
                }
            }
        }

        return new Taxonomy(
            phrasePairs
                .OrderByDescending(item => item.Phrase.Length)
                .ToList(),
            families);
    }

    private static string OnlyLetters(string value)
    {
        return new string(value.Where(char.IsAsciiLetter).ToArray());
    }

    private static string NormalizeSlug(string value)
    {
        return Regex.Replace(
                value.Trim().ToLowerInvariant().Replace('_', '-').Replace(' ', '-'),
                "-{2,}",
                "-")
            .Trim('-');
    }

    private static string NormalizeForMatch(string value)
    {
        var lowered = value.Trim().ToLowerInvariant();
        lowered = lowered.Replace('_', ' ').Replace('-', ' ');
        return Regex.Replace(lowered, @"\s+", " ").Trim();
    }

    private static bool ContainsPhrase(string haystack, string phrase)
    {
        if (phrase.Length == 0 || haystack.Length < phrase.Length)
        {
            return false;
        }

        var index = 0;
        while (index <= haystack.Length - phrase.Length)
        {
            var found = haystack.IndexOf(phrase, index, StringComparison.Ordinal);
            if (found < 0)
            {
                return false;
            }

            var beforeOk = found == 0 || !IsTokenChar(haystack[found - 1]);
            var afterIndex = found + phrase.Length;
            var afterOk = afterIndex == haystack.Length
                || !IsTokenChar(haystack[afterIndex]);

            if (beforeOk && afterOk)
            {
                return true;
            }

            index = found + 1;
        }

        return false;
    }

    private static bool IsTokenChar(char character)
    {
        return char.IsLetterOrDigit(character) || character is '#' or '.';
    }

    private static void AddProjectSlug(HashSet<string> slugs, string? raw)
    {
        var slug = NormalizeSlug(raw ?? string.Empty);
        if (slug.Length > 0)
        {
            slugs.Add(slug);
        }
    }

    private static int RelatedSlugOrder(string slug)
    {
        return NormalizeSlug(slug) switch
        {
            "docker" => 0,
            "docker-compose" => 1,
            "container-registry" => 2,
            _ => 100
        };
    }

    private static IEnumerable<string> TokenizeTechnologyText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        var trimmed = text.Trim();
        if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
        {
            List<string>? parsed = null;
            try
            {
                using var document = JsonDocument.Parse(trimmed);
                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    parsed = EnumerateTechnologyValues(document.RootElement.Clone())
                        .ToList();
                }
            }
            catch (JsonException)
            {
                parsed = null;
            }

            if (parsed is not null)
            {
                foreach (var nested in parsed)
                {
                    yield return nested;
                }

                yield break;
            }
        }

        foreach (Match match in Regex.Matches(
            trimmed.ToLowerInvariant(),
            @"[a-z0-9]+(?:[.-][a-z0-9]+)*"))
        {
            if (match.Value.Length > 1)
            {
                yield return match.Value;
            }
        }
    }

    private static IEnumerable<string> EnumerateTechnologyValues(object value)
    {
        if (value is JsonElement json)
        {
            if (json.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in json.EnumerateArray())
                {
                    var text = item.ValueKind == JsonValueKind.String
                        ? item.GetString()
                        : item.ToString();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        yield return text;
                    }
                }

                yield break;
            }

            if (json.ValueKind == JsonValueKind.String)
            {
                var text = json.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    yield return text;
                }
            }

            yield break;
        }

        if (value is IEnumerable<object?> collection && value is not string)
        {
            foreach (var item in collection)
            {
                if (item is null)
                {
                    continue;
                }

                foreach (var nested in EnumerateTechnologyValues(item))
                {
                    yield return nested;
                }
            }

            yield break;
        }

        var raw = value.ToString();
        if (!string.IsNullOrWhiteSpace(raw))
        {
            yield return raw;
        }
    }

    internal sealed record Family(
        string Id,
        HashSet<string> Slugs);

    internal sealed record Taxonomy(
        IReadOnlyList<(string Phrase, string Slug)> PhrasesByLength,
        IReadOnlyList<Family> Families)
    {
        public static Taxonomy Empty { get; } =
            new([], []);
    }
}
