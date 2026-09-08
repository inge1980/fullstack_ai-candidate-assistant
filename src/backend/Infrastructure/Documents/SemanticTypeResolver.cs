namespace Infrastructure.Documents;

public sealed class SemanticTypeResolver
{
    private static readonly IReadOnlyList<SemanticTypeRule> Rules =
    [
        new("overview", ["overview"]),
        new("context", ["context"]),
        new("task", ["task"]),
        new("result", ["result"]),
        new("lessons-learned", ["lessons learned"]),
        new("future-improvements", ["future improvements"]),

        new("architecture", ["architecture"]),
        new("technical-decisions", ["technical decisions"]),
        new("implementation", ["implementation"]),
        new("challenge", ["challenge"])
    ];

    public string Resolve(string? headingPath)
    {
        var path = NormalizeHeadingPath(headingPath);

        if (path.Length == 0)
        {
            return "section";
        }

        /*
         * More specific semantic types are resolved from the most
         * specific meaningful segment in the heading path.
         *
         * Example:
         *
         * Action > Architecture > Backend
         *     -> architecture-backend
         *
         * Action > Technical Decisions > Chosen Solution
         *     -> technical-decision-solution
         *
         * Challenge > Some Challenge > Problem
         *     -> challenge-problem
         */

        if (TryResolveChallenge(path, out var challengeType))
        {
            return challengeType;
        }

        if (TryResolveTechnicalDecision(path, out var decisionType))
        {
            return decisionType;
        }

        if (TryResolveArchitecture(path, out var architectureType))
        {
            return architectureType;
        }

        if (TryResolveImplementation(path, out var implementationType))
        {
            return implementationType;
        }

        /*
         * Generic section types are resolved from the path itself.
         * We search from the most specific segment backwards so that
         * nested headings remain compatible with future template changes.
         */
        for (var index = path.Length - 1; index >= 0; index--)
        {
            var segment = path[index];

            var rule = Rules.FirstOrDefault(
                rule => rule.Matches(segment));

            if (rule is not null)
            {
                return rule.SemanticType;
            }
        }

        return "section";
    }

    private static bool TryResolveChallenge(
        IReadOnlyList<string> path,
        out string semanticType)
    {
        semanticType = string.Empty;

        var challengeIndex = FindSegment(path, "challenge");

        if (challengeIndex < 0)
        {
            return false;
        }

        var descendant = GetLastSegmentAfter(path, challengeIndex);

        semanticType = descendant switch
        {
            "problem" => "challenge-problem",
            "solution" => "challenge-solution",
            "result" => "challenge-result",
            _ => "challenge"
        };

        return true;
    }

    private static bool TryResolveTechnicalDecision(
        IReadOnlyList<string> path,
        out string semanticType)
    {
        semanticType = string.Empty;

        var decisionIndex = FindSegment(
            path,
            "technical decisions");

        if (decisionIndex < 0)
        {
            return false;
        }

        var descendant = GetLastSegmentAfter(path, decisionIndex);

        semanticType = descendant switch
        {
            "context" => "technical-decision-context",
            "chosen solution" => "technical-decision-solution",
            "alternatives considered" => "technical-decision-alternatives",
            "trade-offs" => "technical-decision-tradeoffs",
            _ => "technical-decisions"
        };

        return true;
    }

    private static bool TryResolveArchitecture(
        IReadOnlyList<string> path,
        out string semanticType)
    {
        semanticType = string.Empty;

        var architectureIndex = FindSegment(
            path,
            "architecture");

        if (architectureIndex < 0)
        {
            return false;
        }

        var descendant = GetLastSegmentAfter(path, architectureIndex);

        semanticType = descendant switch
        {
            "frontend" => "architecture-frontend",
            "backend" => "architecture-backend",
            "database" => "architecture-database",
            "file storage" => "architecture-file-storage",
            "infrastructure" => "architecture-infrastructure",
            _ => "architecture"
        };

        return true;
    }

    private static bool TryResolveImplementation(
        IReadOnlyList<string> path,
        out string semanticType)
    {
        semanticType = string.Empty;

        var implementationIndex = FindSegment(
            path,
            "implementation");

        if (implementationIndex < 0)
        {
            return false;
        }

        var descendant = GetLastSegmentAfter(
            path,
            implementationIndex);

        semanticType = descendant switch
        {
            "features" => "implementation-features",
            "apis" => "implementation-apis",
            "data and persistence" => "implementation-data",
            "automation" => "implementation-automation",
            "testing" => "implementation-testing",
            _ => "implementation"
        };

        return true;
    }

    private static int FindSegment(
        IReadOnlyList<string> path,
        string segment)
    {
        for (var index = 0; index < path.Count; index++)
        {
            if (string.Equals(
                    path[index],
                    segment,
                    StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        return -1;
    }

    private static string GetLastSegmentAfter(
        IReadOnlyList<string> path,
        int index)
    {
        return index + 1 < path.Count
            ? path[^1]
            : string.Empty;
    }

    private static string[] NormalizeHeadingPath(
        string? headingPath)
    {
        if (string.IsNullOrWhiteSpace(headingPath))
        {
            return [];
        }

        return headingPath
            .Split(
                '>',
                StringSplitOptions.RemoveEmptyEntries)
            .Select(NormalizeHeading)
            .Where(static heading => heading.Length > 0)
            .ToArray();
    }

    private static string NormalizeHeading(
        string heading)
    {
        return heading
            .Trim()
            .ToLowerInvariant()
            .Replace(":", "")
            .Replace("?", "-")
            .Replace("?", "-")
            .Replace('\u00A0', ' ')
            .Trim();
    }

    private sealed record SemanticTypeRule(
        string SemanticType,
        IReadOnlyList<string> Headings)
    {
        public bool Matches(string heading)
        {
            return Headings.Any(
                expected => string.Equals(
                    expected,
                    heading,
                    StringComparison.OrdinalIgnoreCase));
        }
    }
}