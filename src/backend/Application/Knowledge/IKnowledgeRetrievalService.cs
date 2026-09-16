using System.Threading;
using System.Threading.Tasks;

namespace Application.Knowledge;

public interface IKnowledgeRetrievalService
{
    Task<KnowledgeRetrievalResult> RetrieveAsync(
        string query,
        int retrievalLimit = 10,
        bool includeMatchingOrganizationOverviews = false,
        IReadOnlyList<string>? technologySlugs = null,
        CancellationToken cancellationToken = default);
}