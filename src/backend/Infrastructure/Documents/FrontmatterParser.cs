using YamlDotNet.Serialization;

namespace Infrastructure.Documents;

public class FrontmatterParser
{
    private readonly IDeserializer _deserializer;

    public FrontmatterParser()
    {
        _deserializer =
            new DeserializerBuilder()
                .Build();
    }

    public ParsedMarkdown Parse(string markdown)
    {
        var result = new ParsedMarkdown();

        if (!markdown.StartsWith("---"))
        {
            result.Content = markdown;
            return result;
        }

        var endIndex =
            markdown.IndexOf(
                "\n---",
                3,
                StringComparison.Ordinal);

        if (endIndex == -1)
        {
            result.Content = markdown;
            return result;
        }

        var frontmatter =
            markdown
                .Substring(
                    3,
                    endIndex - 3)
                .Trim();

        var metadata =
            _deserializer.Deserialize<Dictionary<string, object?>>(
                frontmatter);

        result.Metadata =
            metadata ?? new Dictionary<string, object?>();

        result.Content =
            markdown[(endIndex + 4)..]
                .Trim();

        return result;
    }
}