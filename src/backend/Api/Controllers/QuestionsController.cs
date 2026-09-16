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
            return QuestionRequired();
        }

        var response = await service.AskAsync(
            request.Question,
            QuestionLocale.Normalize(request.Locale),
            includeDebug,
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Same ask pipeline as POST /api/v1/Questions, with SSE phase events
    /// (translating, searching, writing, trying-another-model) and one final JSON result.
    /// </summary>
    [HttpPost("progress")]
    [Produces("text/event-stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task Progress(
        [FromBody] AskQuestionRequest request,
        [FromQuery] bool includeDebug = false,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            await Response.WriteAsJsonAsync(QuestionRequired().Value, cancellationToken);
            return;
        }

        ServerSentEvents.Prepare(Response);

        try
        {
            var response = await service.AskAsync(
                request.Question,
                QuestionLocale.Normalize(request.Locale),
                includeDebug,
                cancellationToken,
                onProgress: async (phase, token) =>
                    await ServerSentEvents.WritePhaseAsync(
                        Response,
                        phase.ToEventCode(),
                        token));

            await ServerSentEvents.WriteJsonAsync(
                Response,
                "result",
                response,
                cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            await ServerSentEvents.WriteJsonAsync(
                Response,
                "error",
                new ProblemDetails
                {
                    Title = "Request failed",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError
                },
                CancellationToken.None);
        }
    }

    /// <summary>
    /// Classify question intent with keyword rules. No retrieval or LLM.
    /// </summary>
    [HttpPost("intent")]
    [ProducesResponseType(typeof(QuestionIntent), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<QuestionIntent> DetectIntent(
        [FromBody] AskQuestionRequest request)
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

        return Ok(QuestionIntentDetector.Detect(request.Question));
    }

    private BadRequestObjectResult QuestionRequired()
    {
        return BadRequest(new ProblemDetails
        {
            Title = "Validation error",
            Detail = "Question is required.",
            Status = StatusCodes.Status400BadRequest
        });
    }
}