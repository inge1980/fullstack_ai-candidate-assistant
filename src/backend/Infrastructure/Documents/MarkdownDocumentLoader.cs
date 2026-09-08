namespace Infrastructure.Documents;

public class MarkdownDocumentLoader
{
    private static readonly string[] IgnoredFiles =
    {
        "_template_project.md"
    };

    private readonly FrontmatterParser parser = new();


    public IEnumerable<MarkdownDocument> Load(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException(path);
        }

        // foreach (var file in Directory.GetFiles(path, "*.md", SearchOption.AllDirectories).Take(3)) // Limit to 3 files for testing
        foreach (var file in Directory.GetFiles(path, "*.md", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileName(file);

            if (IgnoredFiles.Contains(
                    fileName,
                    StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            Console.WriteLine(
                $"Parsing: {fileName}");

            var markdown =
                File.ReadAllText(file);

            ParsedMarkdown parsed;
            try
            {
                parsed = parser.Parse(markdown);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to parse frontmatter in '{fileName}'.",
                    ex);
            }

            var document = new MarkdownDocument
            {
                FileName = fileName,
                Content = parsed.Content,
                Metadata = parsed.Metadata
            };

            yield return document;
        }
    }
}