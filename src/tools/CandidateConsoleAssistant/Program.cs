using Infrastructure.Embeddings;
using Infrastructure.Reranking;
using Application.Knowledge;
using Application.Questions;
using Microsoft.Extensions.Configuration;
using Infrastructure.Configuration;

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
    // Tests: Emotions of the developer
    "What decision in what project are you most proud of?",
    //Error: Hallicinates feelings.

    // Tests: list intent with status
    "Which projects are still active?",
    // Error: Projects with status completed are shown in the answer.

    // Tests: Section inventory without a specific projects --> forced detail. Will we get one projects Challenge chunk instead of a cross-project inventory?
    "List the challenges.",
    // Error: Lists challenges for the random top 10 project-chunks that got sent to the LLM.




    // Tests: Union (what experience + and)
    //"What experience do I have with SQL Server and MySQL?",

    // Tests: Intent. "what projects" is a list cue; Next.js is a technology filter -> filter-list.
    //"What projects involved Next.js?",

    // Tests: Catalog + named organization (list, unique-by-source, keep Episteme AS only).
    //"What projects have you done for Episteme?",

    // Tests: Union of multiple companies
    //"What projects have you done for Episteme and Moava?",

    //Tests: Intent. which of my projects is now a list cue.
    //"Which of my projects used SQL Server?",

    // Tests: Unsupported-claim refusal (Kubernetes appears only as an eval topic inside the RAG knowledge doc, not as a technology)
    //"Have you used Kubernetes?",

    // Tests: Intersection (have you used A and B)
    //"Have you used PHP and C# in the same project?",

    // Tests: Section-inventory override (list + technical decisions + named project → detail)
    //"List the technical decisions in the Lost & Found API.",

    // Tests: Unique-sources the retrieve window and lets the LLM number them.
    //"How many projects have I completed at Moava?",

    // Tests: Broad “have you done” + production language that may not match in production / production experience
    //"Have you done production work at Episteme?",

    // Tests: Named-project retrieval, sibling-doc bleed (ERP/canteen)
    //"Tell me about the PIM integration at Episteme.",

    //Tests: React Native ≠ React web
    //"Have you used React Native?",

    // Tests: Concepts (offline-first) rather than a technologies token
    //"What experience do I have with offline-first mobile apps?",

    // Tests: Rare stack token
    //"Have you used Subversion?",

    // Tests: Token drop (C# length 1) plus production filter. Metadata "csharp" may never match the query term
    //"Have you used C# in production?",

    // Tests: Role metadata vs “AI/LLM experience”. n8n is the only AI Developer role.
    //"Have you worked as an AI Developer?",

    // Tests: Organization Personal and environment: production. Production filter keeps portfolio, while shopping list and n8n are personal development.
    //"Have you shipped a personal project to production?",

    // Tests: Alias (24SevenOffice → Finago)
    //"Describe the hotel booking case for Finago.",

    // Tests: Dependency that is not ASP.NET Core, Docker, or PostgreSQL-with-.NET. Should get Lost & Found, while RAG should not invent EF.
    //"Have you used Entity Framework Core?",

    // Tests: Documented non-capability. Prompt must refuse or clearly say publishing was out of scope.
    //"Did you publish social media posts with the n8n workflow?",

    // Test retrieval of projects with PostgreSQL
    //"How many project have you worked on with PostgreSQL, and what where they all about?",

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
    //"What experience do you have with PostgreSQL?",       // Broad knowledge and specific examples
    //"Have you used pgvector?",                          // RAG-prosject ranked as nr 1, but also other projects
    //"Have you used PostgreSQL with .NET?",              // Lost & Found high ranked
    //"What databases have you worked with?",               // PostgreSQL + others

    // Initial test questions for retrieval evaluation
    // Broad technology experience
    //"What experience do I have with ASP.NET Core?",

    // Specific implementation detail
    //"How did you authenticate your Azure deployment?",

    // Known ranking problem
    //"Have you worked with CI/CD?",

    // Broader domain experience
    //"What experience do I have with ERP systems?" //,

    // Other specific technologies
    //"What experience do I have with .NET and PostgreSQL?",
    //"What projects involved React and TypeScript?",
    //"What experience do I have with GDPR and form builders?",
    //"Have you worked with Docker?",
    //"What Azure experience do you have?",
    //"Have you worked with Terraform?",
    //"What experience do I have with Terraform?",
    //"What projects demonstrate backend development?",
    //"Have you worked with APIs and integrations?"
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

    var intent = QuestionIntentDetector.Detect(question);
    Console.WriteLine(
        $"[Intent] {intent.Category} n={intent.RequestedCount?.ToString() ?? "-"}");

    var retrievalLimit =
        PromptContextSelector.RetrievalLimit(intent, question);

    var retrieval =
        await knowledgeRetrievalService.RetrieveAsync(
            query: question,
            retrievalLimit: retrievalLimit,
            includeMatchingOrganizationOverviews:
                PromptContextSelector.IncludeMatchingOrganizationOverviews(
                    intent,
                    question),
            technologySlugs:
                PromptContextSelector.TechnologyOverviewSlugs(question));

    Console.WriteLine($"[Retrieval] limit={retrievalLimit} hits={retrieval.Items.Count}");

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

    var promptResults =
        PromptContextSelector.Select(
            intent,
            retrieval.Items,
            question);

    Console.WriteLine();
    Console.WriteLine(
        $"[Context] uniqueSources={retrieval.Items.Select(item => item.Source).Distinct(StringComparer.OrdinalIgnoreCase).Count()} selected={promptResults.Count} projects={string.Join(", ", promptResults.Select(item => AnswerPromptFormatter.ProjectId(item.Source)))}");

    var context =
        AnswerPromptFormatter.FormatContext(promptResults, question);

    var prompt =
        AnswerPromptFormatter.Fill(
            answerPromptTemplate,
            question,
            context,
            QuestionLocale.Us,
            promptResults);

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