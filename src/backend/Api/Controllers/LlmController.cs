using Infrastructure.LLM;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/llm")]
public class LLMController : ControllerBase
{
    private readonly LlmClientFactory _llmClientFactory;

    public LLMController(
        LlmClientFactory llmClientFactory)
    {
        _llmClientFactory = llmClientFactory;
    }

    /// <summary>
    /// Testing connection to LLM provider and generating a simple response.
    /// This endpoint is for testing purposes and should be removed in production.
    /// </summary>
    [HttpGet("test")]
    public async Task<IActionResult> Test(
        CancellationToken cancellationToken)
    {
        var client =
            _llmClientFactory.Create();

        var response =
            await client.GenerateAsync(
                "Say hello in one sentence.",
                cancellationToken);

        return Ok(new
        {
            response
        });
    }
}