using System.Text.RegularExpressions;

namespace Infrastructure.Documents;

public class DocumentChunker
{
    private readonly SemanticTypeResolver _semanticTypeResolver;

    public DocumentChunker(SemanticTypeResolver semanticTypeResolver)
    {
        _semanticTypeResolver =
            semanticTypeResolver;
    }

    private static readonly Regex HeadingRegex = new(
        @"^(#{1,6})\s+(.+?)\s*$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    public List<DocumentChunk> Chunk(string markdown, string source, Dictionary<string, object?>? metadata = null)
    {
        var chunks = new List<DocumentChunk>();
        var sections = SplitIntoSections(markdown);

        for (var index = 0; index < sections.Count; index++)
        {
            var section = sections[index];
            var content = CleanContent(section.Content);

            if (string.IsNullOrWhiteSpace(content))
                continue;


            var semanticType =
                _semanticTypeResolver.Resolve(
                    section.HeadingPath);

            chunks.Add(new DocumentChunk
            {
                Id = $"{source}-{index:D3}-{CreateIdPart(section.HeadingPath)}",
                Source = source,
                HeadingPath = section.HeadingPath,
                SemanticType = semanticType,
                Content = content,
                Metadata = metadata is null
                    ? new Dictionary<string, object?>()
                    : new Dictionary<string, object?>(metadata)
            });

        }

        return chunks;
    }


    private static List<MarkdownSection> SplitIntoSections(string markdown)
    {
        var sections = new List<MarkdownSection>();
        var headingStack = new string[6];

        var matches = HeadingRegex.Matches(markdown);

        for (var index = 0; index < matches.Count; index++)
        {
            var match = matches[index];

            var level = match.Groups[1].Value.Length;
            var title = match.Groups[2].Value.Trim();

            var start = match.Index;
            var end = index + 1 < matches.Count
                ? matches[index + 1].Index
                : markdown.Length;

            var content = markdown[start..end];

            headingStack[level - 1] = title;

            for (var i = level; i < headingStack.Length; i++)
            {
                headingStack[i] = string.Empty;
            }

            var headingPath = string.Join(
                " > ",
                headingStack.Where(x => !string.IsNullOrWhiteSpace(x)));

            sections.Add(new MarkdownSection
            {
                HeadingLevel = level,
                Title = title,
                HeadingPath = headingPath,
                Content = content
            });
        }

        return sections;
    }

    private static string CleanContent(string content)
    {
        var lines = content
            .Replace("\r\n", "\n")
            .Split('\n')
            .ToList();

        if (lines.Count > 0 && HeadingRegex.IsMatch(lines[0]))
        {
            lines.RemoveAt(0);
        }

        return string.Join("\n", lines)
            .Replace("---", "")
            .Trim();
    }

    private static string CreateIdPart(string headingPath)
    {
        return Regex.Replace(
            headingPath.ToLowerInvariant(),
            @"[^a-z0-9]+",
            "-")
            .Trim('-');
    }

    private sealed class MarkdownSection
    {
        public int HeadingLevel { get; set; }

        public string Title { get; set; } = string.Empty;

        public string HeadingPath { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}