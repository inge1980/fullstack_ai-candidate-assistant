using System.Threading;
using System.Threading.Tasks;

namespace Application.Knowledge;

public interface IKnowledgeRetrievalService
{
    Task<KnowledgeRetrievalResult> RetrieveAsync(
        string query,
        int retrievalLimit = 10,
        CancellationToken cancellationToken = default);
}