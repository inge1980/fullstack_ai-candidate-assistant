using Infrastructure.Embeddings;
using Infrastructure.Reranking;
using Application.Knowledge;
using Application.Questions;
using Microsoft.Extensions.Configuration;
using Infrastructure.Configuration;

const int retrievalLimit = 25; // Limit the number of results retrieved from the vector store
const int promptContextLimit = 10; // Limit the number of results included in the prompt context

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

var answerPromptTemplate =
    await File.ReadAllTextAsync(promptPath);

// Use global config file for all projects in solution
var configuration =
    AppConfiguration.Build();

// Initialize services
var embeddingService =
    new EmbeddingService();

var connectionString =
    configuration.GetConnectionString("Postgres")
    ?? throw new InvalidOperationException(
        "ConnectionStrings__Postgres environment variable is missing.");

//var connectionString =
//    Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
//    ?? throw new InvalidOperationException(
//        "ConnectionStrings__Postgres environment variable is missing.");

var vectorStore =
    new VectorStore(connectionString);
    
var evidenceScorer =
    new MetadataEvidenceScorer();

var knowledgeRetrievalService =
    new KnowledgeRetrievalService(
        embeddingService,
        vectorStore,
        evidenceScorer);

// TEST: Retrieval evaluation code
var questions = new[]
{
    // Test retrieval of projects with PostgreSQL
    "How many project have you worked on with PostgreSQL, and what where they all about?",

    // Test retrieval of projects with specific technologies
    //"Gi meg topp 10 prosjekter du har gjort som inneholder html, css, sql eller javascript.",

    // Test broad technology experience
    //"What experience do you have with agile development?",

    // Test document diversity
    //"What experience do you have working with AI or LLMs?",

    // Test summary of a job advertisement to evaluate retrieval and ranking
    //"Which of my projects demonstrate experience relevant to a Platform Engineer role involving software development, developer experience, internal developer platforms, Kubernetes, IaC, CI/CD, automation, and hybrid on-prem/cloud?"

    // Test small variations of the same question to evaluate retrieval and ranking
    //"Have you used PostgreSQL?",
    //"Have you used PostgreSQL in production?",
    //"Have you used PostgreSQL in a school project?",
    //"Have you used PostgreSQL for personal projects?",
    //"What production experience do I have?"

    // PostgreSQL-related questions to evaluate retrieval and ranking
    //"Have you built systems involving PostgreSQL?",     //  Multiple projects 
    //"What experience do I have with PostgreSQL?",       // Broad knowledge and specific examples
    //"Have you used pgvector?",                          // RAG-prosject ranked as nr 1, but also other projects
    //"Have you used PostgreSQL with .NET?",              // Lost & Found high ranked
    //"What databases have I worked with?",               // PostgreSQL + others
    //"Have you used PostgreSQL in production?"           // Should avoid school projects and focus on real-world experience

/* 
// Initial test questions for retrieval evaluation
    // Broad technology experience
    "What experience do I have with ASP.NET Core?",

    // Specific implementation detail
    "How did you authenticate your Azure deployment?",

    // Known ranking problem
    "Have you worked with CI/CD?",

    // Potential false positive / noisy retrieval
    "Have you built systems involving PostgreSQL?",

    // Broader domain experience
    "What experience do I have with ERP systems?" //,
 */

    // Other specific technologies
    /* "What experience do I have with .NET and PostgreSQL?",
    "What projects involved React and TypeScript?",
    "What experience do I have with GDPR and form builders?",
    "Have you worked with Docker?",
    "What Azure experience do you have?",
    "Have you worked with Terraform?",
    "What experience do I have with Terraform?",
    "What projects demonstrate backend development?",
    "Have you worked with APIs and integrations?" */
};

Console.WriteLine();
Console.WriteLine("==============================");
Console.WriteLine("RAG RETRIEVAL EVALUATION");
Console.WriteLine("==============================");

foreach (var question in questions)
{
    Console.WriteLine();
    Console.WriteLine("==============================");
    Console.WriteLine($"Question: {question}");
    Console.WriteLine("==============================");
    Console.WriteLine();

    var retrieval =
        await knowledgeRetrievalService.RetrieveAsync(
            query: question,
            retrievalLimit: retrievalLimit);

    var rank = 1;

    foreach (var result in retrieval.Items)
    {
        Console.WriteLine();
        Console.WriteLine($"#{rank} Combined: {result.CombinedScore:F4}");
        Console.WriteLine($"   Vector: {result.VectorScore:F4}, Metadata: {result.MetadataScore:F4}, Evidence: {result.EvidenceScore:F4}");
        Console.WriteLine($"   Source: {result.Source}");
        Console.WriteLine($"   Heading: {result.Heading}");
        Console.WriteLine($"   Semantic Type: {result.SemanticType}");

        var preview =
            result.Content
                .Replace("\r\n", " ")
                .Replace("\n", " ")
                .Trim();

        if (preview.Length > 300)
        {
            preview =
                preview[..300] + "...";
        }

        Console.WriteLine($"   Content: {preview}");

        rank++;
    }

    var context =
        string.Join(
            "\n\n",
            retrieval.Items
            .Take(promptContextLimit)
                .Select(
                    (result, index) =>
                        $"[{index + 1}] {result.Source}\n" +
                        $"Heading: {result.Heading}\n" +
                        $"Semantic Type: {result.SemanticType}\n" +
                        $"Content: {result.Content}"));

    var prompt =
        answerPromptTemplate
            .Replace("{{question}}", question)
            .Replace("{{context}}", context)
            .Replace(
                "{{answer_language_instruction}}",
                QuestionLocale.AnswerLanguageInstruction(QuestionLocale.Us));

    Console.WriteLine();
    Console.WriteLine("==============================");
    Console.WriteLine("GENERATED ANSWER PROMPT");
    Console.WriteLine("==============================");
    Console.WriteLine();
    Console.WriteLine(prompt);
    Console.WriteLine();
    Console.WriteLine("==============================");
    Console.WriteLine("GENERATED ANSWER PROMPT COMPLETE");
    Console.WriteLine("==============================");
}

Console.WriteLine();
Console.WriteLine("==============================");
Console.WriteLine("RETRIEVAL EVALUATION COMPLETE");
Console.WriteLine("==============================");