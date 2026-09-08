using Application.Questions;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class QuestionsController(IQuestionService service) : ControllerBase
{
    /// <summary>
    /// Send a new question to the service for processing and receive a response.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AskQuestionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AskQuestionResponse>> Post(
        [FromBody] AskQuestionRequest request,
        [FromQuery] bool includeDebug = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Detail = "Question is required.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var response = await service.AskAsync(
            request.Question,
            includeDebug,
            cancellationToken);

        return Ok(response);
    }
}