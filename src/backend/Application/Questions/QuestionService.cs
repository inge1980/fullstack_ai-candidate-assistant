using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Application.Questions;
using Application.Knowledge;
using Infrastructure.LLM;

namespace Api.Services;

public sealed class QuestionService(
    IKnowledgeRetrievalService knowledgeRetrievalService,
    LlmClientFactory llmClientFactory,
    IConfiguration configuration)
    : IQuestionService
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<AskQuestionResponse> AskAsync(
        string question,
        string locale = QuestionLocale.Us,
        bool includeDebug = false,
        CancellationToken cancellationToken = default,
        IConfiguration configuration = null!)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            throw new ArgumentException(
                "Question cannot be empty.",
                nameof(question));
        }

        locale = QuestionLocale.Normalize(locale);

        var intent = QuestionIntentDetector.Detect(question);
        Console.WriteLine(
            $"[Intent] {intent.Category} n={intent.RequestedCount?.ToString() ?? "-"}");

        var client = llmClientFactory.Create();

        var retrievalQuery = question;

        if (QuestionLocale.RequiresQueryTranslation(locale))
        {
            var translationStopwatch = Stopwatch.StartNew();

            retrievalQuery =
                await TranslateQueryToEnglishAsync(
                    client,
                    question,
                    cancellationToken);

            translationStopwatch.Stop();
            Console.WriteLine($"[Timing] Query translation: {translationStopwatch.ElapsedMilliseconds} ms");
        }

        // --------------------------------------------------------
        // 1. Retrieve and rank knowledge
        // --------------------------------------------------------

        var retrievalStopwatch = Stopwatch.StartNew();

        var retrievalLimit =
            PromptContextSelector.RetrievalLimit(intent);

        var retrieval =
            await knowledgeRetrievalService.RetrieveAsync(
                query: retrievalQuery,
                retrievalLimit: retrievalLimit,
                cancellationToken: cancellationToken);

        retrievalStopwatch.Stop();
        Console.WriteLine($"[Timing] Retrieval: {retrievalStopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"[Retrieval] limit={retrievalLimit} hits={retrieval.Items.Count}");

        // --------------------------------------------------------
        // 2. Select the context that will be sent to the LLM
        // --------------------------------------------------------

        var contextSelectionStopwatch = Stopwatch.StartNew();

        var promptResults =
            PromptContextSelector.Select(
                intent,
                retrieval.Items);

        contextSelectionStopwatch.Stop();
        Console.WriteLine($"[Timing] Context selection: {contextSelectionStopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine(
            $"[Context] uniqueSources={retrieval.Items.Select(item => item.Source).Distinct(StringComparer.OrdinalIgnoreCase).Count()} selected={promptResults.Count} projects={string.Join(", ", promptResults.Select(item => AnswerPromptFormatter.ProjectId(item.Source)))}");

        // --------------------------------------------------------
        // 3. Build the context for the answer prompt
        // --------------------------------------------------------
        
        var contextBuildStopwatch = Stopwatch.StartNew();

        var context =
            AnswerPromptFormatter.FormatContext(promptResults);
        
        contextBuildStopwatch.Stop();
        Console.WriteLine($"[Timing] Context build: {contextBuildStopwatch.ElapsedMilliseconds} ms");

        // --------------------------------------------------------
        // 4. Load the answer prompt template
        // --------------------------------------------------------

        var promptLoadStopwatch = Stopwatch.StartNew();

        var promptTemplate =
            await LoadAnswerPromptAsync(
                cancellationToken);

        promptLoadStopwatch.Stop();
        Console.WriteLine($"[Timing] Prompt loading: {promptLoadStopwatch.ElapsedMilliseconds} ms");

        // --------------------------------------------------------
        // 5. Build the final LLM prompt
        // --------------------------------------------------------

        var promptBuildStopwatch = Stopwatch.StartNew();

        var prompt =
            AnswerPromptFormatter.Fill(
                promptTemplate,
                question,
                context,
                locale);

        promptBuildStopwatch.Stop();
        Console.WriteLine($"[Timing] Prompt build: {promptBuildStopwatch.ElapsedMilliseconds} ms");

        // --------------------------------------------------------
        // 6. Send prompt to the configured LLM
        // --------------------------------------------------------

        var llmStopwatch = Stopwatch.StartNew();

        var answer =
            await client.GenerateAsync(
                prompt,
                cancellationToken);

        llmStopwatch.Stop();
        Console.WriteLine($"[Timing] LLM: {llmStopwatch.ElapsedMilliseconds} ms");

        // --------------------------------------------------------
        // 7. Map retrieval results to API sources
        // --------------------------------------------------------

        var sourceMappingStopwatch = Stopwatch.StartNew();

        var sources =
            promptResults
                .Select(
                    result =>
                        new QuestionSource(
                            ProjectId: AnswerPromptFormatter.ProjectId(result.Source),
                            Title: AnswerPromptFormatter.ProjectTitle(result),
                            Url: GetProjectUrl(result.Source),
                            Heading: result.Heading,
                            SemanticType: result.SemanticType,
                            Content: result.Content,
                            Source: includeDebug
                                ? result.Source
                                : null,
                            Relevance: includeDebug
                                ? new QuestionRelevance(
                                    Combined: result.CombinedScore,
                                    Vector: result.VectorScore,
                                    Metadata: result.MetadataScore,
                                    Evidence: result.EvidenceScore)
                                : null))
                .ToList();

        sourceMappingStopwatch.Stop();
        Console.WriteLine($"[Timing] Source mapping: {sourceMappingStopwatch.ElapsedMilliseconds} ms");

        // --------------------------------------------------------
        // 8. Return final API response
        // --------------------------------------------------------

        return new AskQuestionResponse(
            Answer: answer,
            Sources: sources,
            Prompt: includeDebug ? prompt : null,
            Intent: intent);
    }

    private static async Task<string> LoadAnswerPromptAsync(
        CancellationToken cancellationToken)
    {
        var promptPath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Prompts",
                "answer",
                "answer-prompt-v8.md");

        if (!File.Exists(promptPath))
        {
            throw new FileNotFoundException(
                $"Could not find answer prompt at: {promptPath}");
        }

        return await File.ReadAllTextAsync(
            promptPath,
            cancellationToken);
    }

    private static async Task<string> TranslateQueryToEnglishAsync(
        ILLMClient client,
        string question,
        CancellationToken cancellationToken)
    {
        var promptPath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Prompts",
                "translate",
                "query-translate-prompt-v1.md");

        if (!File.Exists(promptPath))
        {
            throw new FileNotFoundException(
                $"Could not find query translate prompt at: {promptPath}");
        }

        var promptTemplate =
            await File.ReadAllTextAsync(promptPath, cancellationToken);

        var prompt =
            promptTemplate.Replace("{{question}}", question);

        var translated =
            (await client.GenerateAsync(prompt, cancellationToken))
            .Trim()
            .Trim('"');

        return string.IsNullOrWhiteSpace(translated)
            ? question
            : translated;
    }

    private string GetProjectUrl(
        string source)
    {
        var GithubProjectBaseUrl = "https://github.com/" + _configuration["Github:Owner"] + "/" + _configuration["Github:Repository"] + "/blob/" + _configuration["Github:Branch"] + "/" + _configuration["Github:ProjectsFolder"] + "/";
        var projectId =
            AnswerPromptFormatter.ProjectId(source);

        return $"{GithubProjectBaseUrl}/{projectId}.md";
    }
}