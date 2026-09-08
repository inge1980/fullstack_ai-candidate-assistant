using Microsoft.Extensions.Configuration;
using Application.Questions;

namespace Api.Services;

public interface IQuestionService
{
    Task<AskQuestionResponse> AskAsync(
        string question,
        bool includeDebug = false,
        CancellationToken cancellationToken = default,
        IConfiguration configuration = null!);
}