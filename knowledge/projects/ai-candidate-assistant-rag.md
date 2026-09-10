---
title: AI Candidate Assistant with RAG Knowledge Base

organization: School Project

role: Backend Developer

environment: development

period:
  from: 2026-08
  to: 2026-08

status: completed

technologies:
  - dotnet
  - csharp
  - aspnet-core
  - postgresql
  - pgvector
  - docker
  - ollama
  - bge-small-en-v1.5
  - github
  - google-api
  - groq-api
  - openrouter-api

concepts:
  - artificial-intelligence
  - llm
  - rag
  - document-ingestion
  - text-chunking
  - metadata-extraction
  - embeddings
  - semantic-search
  - vector-database
  - knowledge-management
  - retrieval-evaluation
  - prompt-engineering
  - prompt-generation
  - manual-testing
  - api-design
  - backend-architecture
  - sequential-fallback

dependencies:

links:
  github: https://github.com/inge1980/fullstack_ai-candidate-assistant
  live:

---

# Overview

An AI and LLM application demonstrating practical experience building a Retrieval-Augmented Generation (RAG) system for answering candidate-oriented questions from a structured knowledge base of personal project experience.

The project combines multiple areas of applied AI and LLM development, including RAG architecture, embedding generation, semantic search, vector databases, LLM integration, prompt engineering, grounded generation, retrieval evaluation, and multi-provider LLM fallback.

The system uses BAAI `bge-small-en-v1.5` through Ollama to generate 384-dimensional embeddings for both project knowledge and user queries. PostgreSQL with pgvector is used to persist embeddings and perform semantic vector search over the indexed project documentation.

Retrieved project sections are supplied to an LLM as controlled context. The LLM then generates candidate-oriented responses grounded in the retrieved project evidence rather than relying on general model knowledge.

The project also includes a provider-independent LLM integration layer supporting multiple providers and multiple models per provider. The system can fall back between models within a provider and between different providers when a model or provider fails.

Practical LLM and AI concerns addressed by the project include:

- Retrieval-Augmented Generation (RAG).
- Embedding generation and vector search.
- Semantic retrieval of project experience.
- PostgreSQL and pgvector as a vector database.
- Metadata-aware retrieval foundations.
- Prompt engineering and prompt generation.
- Grounded LLM response generation.
- Retrieval and evidence evaluation.
- Distinguishing supported evidence from unsupported inference.
- Preventing unsupported claims about technologies, responsibilities, and production experience.
- Multi-model and multi-provider LLM integration.
- Model-level and provider-level fallback.
- LLM provider failure handling and observability.
- Manual evaluation of retrieval quality and generated prompts.

The project includes a console-based retrieval evaluation tool that runs representative candidate-oriented questions, displays ranked retrieval results and similarity information, and generates the LLM prompt for manual inspection before generation.

The intended workflow is:

1. A user provides a job description or candidate-oriented question, with a UI locale of `us` or `nb`.
2. If the locale is `nb`, the backend translates the question into English for retrieval. Indexed Markdown stays English.
3. The backend generates an embedding for the English query.
4. PostgreSQL with pgvector performs semantic similarity search against the indexed project knowledge.
5. Relevant project sections and metadata are retrieved.
6. Retrieved evidence is assembled into an English LLM prompt, with an instruction to answer in the requested language.
7. A configured LLM provider and model generate a grounded response. For `nb`, the answer is fluent Norwegian Bokmål rather than a literal translation of English evidence.
8. Source references are returned alongside the generated answer.

The project is intended to demonstrate practical AI, LLM, and RAG application development rather than simply calling an LLM with a large static prompt.

---

# Context

A general-purpose LLM does not reliably know the details of a candidate's personal project history.

Using an LLM without a controlled knowledge source creates a risk of generating plausible but unsupported claims about technologies, responsibilities, architectural decisions, or project outcomes.

The project was therefore designed as a practical application of Retrieval-Augmented Generation (RAG), where semantic retrieval determines which parts of the candidate's documented experience are provided to the LLM as evidence.

The AI pipeline combines several distinct components:

- Local embedding generation.
- Semantic vector search.
- Structured project metadata.
- Retrieval and evidence ranking.
- LLM prompt construction.
- Grounded response generation.
- Multi-provider and multi-model LLM fallback.
- Manual retrieval evaluation.

Project knowledge is maintained as version-controlled Markdown with YAML frontmatter. The Markdown files remain the source of truth, while PostgreSQL with pgvector acts as a generated retrieval index.

During ingestion, project documents are parsed into semantic sections and enriched with structured metadata such as role, technologies, concepts, organization, environment, period, and status. Embeddings are generated for the resulting chunks and stored alongside their metadata.

At query time, a natural-language question is converted into an embedding using the same embedding model. When the request locale is `nb`, that question is first translated to English so the embedding matches the English knowledge base. PostgreSQL with pgvector retrieves semantically related project sections, which are then assembled into English context for the LLM. The generation step may still answer in Norwegian Bokmål when the locale is `nb`.

The LLM is responsible for interpreting and synthesizing the retrieved evidence, while the retrieval system is responsible for determining which documented project experience is relevant.

This separation is important because a semantically plausible answer is not necessarily a supported answer. For example, evidence that a technology was used in a project does not automatically establish that it was used in production. The answer-generation instructions therefore explicitly require the model to distinguish documented evidence from unsupported inference.

The project also treats LLM providers as replaceable infrastructure rather than coupling the application to a single model or provider. Multiple models can be configured per provider, and the system can fall back sequentially across models and providers when failures occur.

The retrieval evaluation tooling was introduced to make AI system behavior inspectable rather than evaluating only the final generated answer. Representative candidate-oriented questions can be executed against the retrieval layer, with similarity scores, metadata, source sections, retrieved evidence, and the generated LLM prompt exposed for manual inspection.

The project therefore separates four primary concerns:

- Human-maintained project knowledge.
- Embedding and retrieval infrastructure.
- Semantic retrieval and evidence selection.
- LLM-based response generation.

The overall objective is to demonstrate practical experience designing and implementing an AI application where embeddings, semantic retrieval, vector storage, LLM integration, prompt engineering, evaluation, and grounded generation work together as distinct parts of the system.

---

# Task

The current task was to build and validate the knowledge ingestion, semantic retrieval, and LLM integration foundations for an AI-powered candidate assistant.

My responsibilities included:

- Designing the Markdown-based knowledge representation.
- Defining structured project metadata through YAML frontmatter.
- Building recursive Markdown document discovery.
- Excluding template documents from ingestion.
- Parsing YAML frontmatter.
- Converting Markdown documents into structured document objects.
- Splitting documents into semantic sections.
- Propagating project metadata to generated chunks.
- Validating generated chunks.
- Generating 384-dimensional embeddings locally.
- Persisting chunks, metadata, and vectors in PostgreSQL.
- Integrating pgvector for similarity search.
- Generating embeddings for user queries.
- Retrieving the most semantically relevant project sections.
- Inspecting retrieval results and similarity scores.
- Building a console-based retrieval evaluation tool for manual inspection.
- Running representative candidate-oriented retrieval questions through the evaluation tool.
- Inspecting generated answer prompts before LLM execution.
- Evaluating whether semantic retrieval is sufficient for candidate-oriented questions.
- Evaluating whether retrieved evidence supports the claims made in generated answers.
- Designing answer-generation instructions that prevent unsupported claims, including distinguishing technology usage from explicitly supported production experience.
- Designing a provider-independent LLM abstraction.
- Integrating multiple LLM providers.
- Supporting multiple models per provider.
- Implementing model-level and provider-level fallback.
- Moving LLM provider/model configuration into a shared environment-based configuration structure.
- Testing provider failures, invalid API keys, unavailable models, and successful fallback execution.
- Identifying where metadata-aware retrieval can improve the system.

---

# Challenge

## Challenge: Building a Reliable Personal Knowledge Retrieval Layer

### Problem

A RAG system is only useful if the retrieved information is relevant to the question and grounded in the underlying source material.

A general-purpose vector search can retrieve semantically similar text without necessarily retrieving the project that is most appropriate for a specific candidate question.

The system therefore needs to preserve enough structure from the original project documentation to make retrieval inspectable and eventually support more sophisticated ranking.

The main requirements were:

- Project documentation must have a predictable structure.
- Metadata must survive the ingestion pipeline.
- Retrieved chunks must retain their source project and section.
- Document and query embeddings must use the same embedding model.
- Retrieval must be easy to inspect and rebuild.
- The original Markdown documents must remain authoritative.
- The retrieval layer must be extensible toward metadata-aware and hybrid search.

### Solution

I designed the ingestion pipeline around structured Markdown documents with YAML frontmatter.

Each document is processed through a deterministic pipeline:

1. Discover Markdown files recursively.
2. Exclude template files.
3. Parse YAML frontmatter.
4. Separate metadata from Markdown content.
5. Split the content into sections based on Markdown headings.
6. Clean generated section content.
7. Ignore empty sections.
8. Propagate project metadata to each generated chunk.
9. Validate the generated chunks.
10. Generate embeddings for each chunk.
11. Persist the chunks, metadata, and vectors in PostgreSQL.
12. Generate an embedding for each user query.
13. Perform vector similarity search using pgvector.
14. Return the highest-ranked project sections for inspection.

The Markdown repository remains the source of truth, while PostgreSQL contains a generated representation that can be discarded and rebuilt.

### Result

The Markdown ingestion and chunking pipeline is operational.

The indexer can recursively discover project Markdown files, ignore the template, parse project metadata, split documents into semantic sections, propagate metadata to generated chunks, and display the resulting index structure for verification.

PostgreSQL is configured with pgvector and a `document_chunks` table capable of storing the generated retrieval representation and 384-dimensional embeddings.

The retrieval layer can be inspected independently from the LLM generation layer, making it possible to evaluate retrieval quality before introducing generation into the workflow.

Initial manual retrieval tests have been performed using representative candidate-oriented questions. The retrieval output is inspected together with similarity scores, source documents, sections, content, and project metadata.

---

## Challenge: Preserving Project Metadata During Retrieval

### Problem

Project documentation contains important structured information that cannot safely be inferred from arbitrary text.

For example, a project may explicitly declare that a technology was implemented, while the same technology might merely be mentioned in a discussion of alternatives or future improvements.

The retrieval system therefore needs to preserve the project's declared metadata rather than relying exclusively on semantic interpretation of the prose.

### Solution

Project metadata is stored in YAML frontmatter.

The metadata includes information such as:

- Project title.
- Organization.
- Role.
- Environment.
- Period.
- Status.
- Technologies.
- Concepts.
- Links.

The ingestion pipeline attaches this metadata to every generated chunk originating from the project.

This makes the metadata available alongside the retrieved content and creates a foundation for future filtering and ranking.

### Result

Retrieved chunks retain their project-level context instead of becoming anonymous pieces of text.

The metadata can later be used for:

- Technology filtering.
- Role filtering.
- Organization filtering.
- Environment filtering.
- Project status filtering.
- Project-level ranking.
- Retrieval explanations.
- Candidate-to-job matching.

---

## Challenge: Choosing a Chunking Strategy

### Problem

Whole-document embeddings are too coarse for precise retrieval.

A complete project document may contain an overview, several unrelated challenges, technical decisions, implementation details, results, and lessons learned.

Embedding the entire document as one vector could therefore cause a query about one technical problem to retrieve a large amount of unrelated project content.

At the same time, arbitrarily splitting the document by character count could break meaningful concepts across chunk boundaries.

### Solution

I chose Markdown headings as the initial chunk boundaries.

The project documentation already uses structured headings such as:

- Overview
- Context
- Task
- Challenge
- Result
- Technical Decisions
- Lessons Learned
- Future Improvements

These sections provide meaningful semantic boundaries and make the resulting chunks easy to inspect.

Each chunk retains:

- Source document.
- Section.
- Content.
- Project metadata.
- Embedding.

Very large sections are intentionally left as a known limitation for a later iteration.

### Result

The retrieval system produces understandable project sections rather than arbitrary fragments.

The initial indexer output confirms that documents are split into predictable sections and that each section becomes an independent chunk.

This makes retrieval results easier to inspect and debug while providing a reasonable initial semantic unit for embedding.

---

## Challenge: Choosing a Vector Storage Architecture

### Problem

The system needs semantic similarity search while also retaining structured project metadata.

Introducing a separate vector database would add another persistence technology to a relatively small personal knowledge base.

### Solution

PostgreSQL was selected as the primary persistence layer, with pgvector providing vector storage and similarity search.

The database stores the generated retrieval representation:

- Chunk content.
- Source document.
- Section.
- Project metadata.
- 384-dimensional embedding vector.

The original Markdown files remain outside the database as the authoritative knowledge source.

### Result

PostgreSQL is configured with the pgvector extension and a `document_chunks` table.

The database can store structured metadata and vector data together.

The original Markdown knowledge base remains authoritative, allowing the PostgreSQL retrieval index to be regenerated when the chunking strategy, metadata schema, or embedding model changes.

---

## Challenge: Evaluating Retrieved Evidence

### Problem

Semantic similarity alone does not guarantee that the retrieved evidence is sufficient to answer a candidate-oriented question correctly.

A retrieved section can be semantically related to a question while still failing to establish an important detail.

For example, evidence that a technology was used does not necessarily establish that it was used in production.

The answer-generation layer therefore needs to distinguish between what the retrieved evidence explicitly supports and what would require an unsupported inference.

### Solution

I implemented a console-based retrieval evaluation tool that runs representative candidate-oriented questions against the retrieval layer.

The evaluation tool displays:

- Query embedding timing.
- Vector search timing.
- Combined retrieval scores.
- Vector similarity.
- Metadata score.
- Evidence score.
- Source document.
- Section.
- Semantic type.
- Retrieved content.

The tool also generates and displays the answer-generation prompt, including the retrieved evidence, so that the retrieval results and prompt construction can be manually inspected before LLM execution.

The answer prompt was also strengthened with explicit instructions to:

- Use retrieved context as evidence rather than repeating it mechanically.
- Avoid inventing technologies, responsibilities, projects, or experience.
- Avoid inferring production experience unless the retrieved evidence explicitly supports it.
- Distinguish between evidence that a technology was used and evidence that it was used in production.
- State clearly when the evidence is insufficient.

Retrieval results are inspected manually before considering further changes to the retrieval strategy.

### Result

The system can be tested not only for whether it retrieves semantically related content, but also for whether the generated prompt contains appropriate evidence for answering the question.

The console evaluation tool provides a repeatable way to inspect retrieval behavior and prompt generation using representative candidate-oriented questions.

The retrieval evaluation includes candidate-oriented questions covering areas such as:

- ASP.NET Core.
- Azure authentication.
- CI/CD.
- PostgreSQL.
- pgvector.
- .NET with PostgreSQL.
- Databases.
- ERP systems.
- PostgreSQL production usage.
- Platform Engineering responsibilities involving software development, developer experience, internal developer platforms, Kubernetes, IaC, CI/CD, automation, and hybrid on-prem/cloud environments.

This evaluation is currently manual rather than an automated retrieval benchmark.

---

## Challenge: Supporting Multiple LLM Providers and Models

### Problem

Depending on a single LLM provider creates an unnecessary single point of failure.

API keys can be invalid, models can become unavailable, quotas can be exhausted, endpoints can return errors, or provider-specific availability can change.

Supporting multiple models within the same provider also requires a more granular fallback strategy than simply switching providers.

### Solution

I introduced a provider-independent `ILLMClient` abstraction and a `FallbackLlmClient` that executes configured provider/model combinations sequentially.

The configuration represents providers as ordered entries, with multiple models per provider.

The fallback order is therefore effectively:

`Provider 1 / Model 1` -> `Provider 1 / Model 2` -> `Provider 2 / Model 1` -> `Provider 2 / Model 2` -> ...

Each concrete client receives its model from the factory rather than keeping a single model inside the client configuration.

The factory creates the appropriate client for each configured provider/model combination.

The fallback client logs both provider and model, making it possible to see exactly which model was attempted and where the fallback occurred.

Provider failures are represented through `LlmProviderException`, including HTTP status and whether the failure is considered transient.

### Result

The LLM layer can now fall back between both models and providers without changing application code.

For example, an unavailable OpenRouter free model can fail and allow another configured model to be attempted.

Invalid API keys and unavailable providers no longer prevent other configured providers from being attempted.

---

## Challenge: Separating Configuration from Provider Implementation

### Problem

The original configuration model stored a single model and timeout directly under each provider.

That structure worked for one model per provider but became awkward once model-level fallback was required.

It also created unnecessary duplication between `appsettings.json` and `.env`.

### Solution

The LLM configuration was changed to a provider list.

The central `LlmOptions` now contains shared generation settings and an ordered list of `LlmProviderOptions`.

Conceptually, the configuration is structured as:

`Llm -> Providers[]`

Each provider entry contains:

- Provider name.
- Ordered model list.
- Timeout.

API keys and provider-specific endpoint configuration remain environment-based.

The shared configuration is loaded through `AppConfiguration`, which loads the repository `.env` file and exposes environment variables through `IConfiguration`.

The application therefore separates:

- Non-secret LLM configuration.
- Secret API keys.
- Provider/model fallback order.

### Result

Adding or reordering models no longer requires changes to the concrete LLM clients.

The same client implementation can be instantiated for multiple models, and provider/model selection is controlled by configuration.

---

## Challenge: Making Provider Failures Observable

### Problem

A fallback system can become difficult to debug if it only returns the final successful result.

Without detailed logging, it is difficult to determine whether a failure came from an invalid API key, unavailable model, quota limitation, transient provider failure, or another HTTP error.

### Solution

The LLM clients record provider-specific HTTP timing and the fallback layer records provider/model attempts.

The logs now use a format such as:

`[LLM] Trying: Google / gemini-3.6-flash`

followed by the HTTP result and:

`[LLM] Failed: Google / gemini-3.6-flash (...) Status=400 Transient=False`

or:

`[LLM] Provider succeeded: Groq / openai/gpt-oss-120b (...)`

The fallback layer continues to the next configured model/provider when appropriate.

### Result

Fallback behavior can be inspected directly from the application logs.

During testing, invalid Google and Groq credentials were successfully skipped, while a valid OpenRouter configuration was able to complete the request.

The logs also exposed unavailable OpenRouter free-model slugs, allowing the configured model list to be corrected.

---

## Challenge: Separating Configuration from Provider Implementation

### Problem

The original configuration model stored a single model and timeout directly under each provider.

That structure worked for one model per provider but became awkward once model-level fallback was required.

### Solution

The LLM configuration was changed to a provider list.

The central `LlmOptions` now contains shared generation settings and an ordered list of `LlmProviderOptions`.

Conceptually, the configuration is structured as:

`Llm -> Providers[]`

Each provider entry contains:

- Provider name.
- Ordered model list.
- Timeout.

API keys and provider-specific endpoint configuration remain environment-based.

### Result

Adding or reordering models no longer requires changes to the concrete LLM clients.

The same client implementation can be instantiated for multiple models, and provider/model selection is controlled by configuration.

---

# Action

## Architecture

### Knowledge Source

Project documentation is stored under:

`knowledge/projects`

Each project is represented by a Markdown document containing:

- YAML frontmatter.
- Project overview.
- Context.
- Responsibilities.
- Challenges.
- Technical decisions.
- Results.
- Lessons learned.
- Interview-oriented information.

The frontmatter provides structured project metadata such as:

- Title.
- Organization.
- Role.
- Environment.
- Period.
- Status.
- Technologies.
- Concepts.
- Links.

The Markdown documents are version controlled and remain the source of truth.

---

### Frontend

The current project is intentionally focused on the backend ingestion, retrieval, and LLM integration rather than a completed candidate-facing UI.

A future frontend could provide an interface for:

- Entering job descriptions.
- Asking candidate-oriented questions.
- Reviewing retrieved project evidence.
- Generating candidate responses.
- Evaluating matching results.

A public-facing chat interface is outside the scope of the completed backend implementation.

---

### Backend

The backend is implemented with C# and ASP.NET Core.

The backend is responsible for:

- Document ingestion.
- Markdown parsing.
- Frontmatter extraction.
- Chunk generation.
- Metadata propagation.
- Embedding generation.
- PostgreSQL persistence.
- Query embedding generation.
- Vector similarity search.
- Retrieval result formatting.
- LLM provider integration.
- Model selection.
- Provider/model fallback.

The backend acts as the orchestration layer between the Markdown knowledge base, embedding model, PostgreSQL/pgvector, and LLM providers.

---

### Database

PostgreSQL is used as the generated retrieval index.

The pgvector extension provides vector storage and similarity search.

The current `document_chunks` table stores:

- Chunk ID.
- Source document.
- Section.
- Content.
- Project metadata as JSONB.
- 384-dimensional embedding vector.

The Markdown files remain authoritative.

If the chunking strategy, metadata schema, or embedding model changes, the PostgreSQL retrieval index can be regenerated from the source documents.

---

### Embedding Pipeline

The development environment uses BAAI `bge-small-en-v1.5` through Ollama.

The model produces 384-dimensional embeddings.

The same embedding model is used for both:

- Document chunks during indexing.
- User queries during semantic retrieval.

Using the same model ensures that document and query vectors occupy the same embedding space.

Changing the embedding model requires the existing document embeddings to be regenerated.

---

### Semantic Retrieval

A user question is converted into an embedding and compared against the stored document embeddings using pgvector.

The retrieval output currently exposes the top-ranked results for inspection.

Each result contains:

- Similarity score.
- Source project.
- Section.
- Retrieved content.
- Associated project metadata.

The retrieval layer is intentionally exposed for inspection so that retrieval quality can be evaluated before relying on the LLM generation stage.

The current test output retrieves the top 10 results before selecting the top 5 results as context for the LLM.

The retrieval implementation does not currently treat the similarity score as a definitive relevance decision.

---

### Retrieval Evaluation Tool

The `CandidateConsoleAssistant` is a console-based evaluation tool for inspecting the retrieval and prompt-generation stages independently.

It runs representative candidate-oriented questions against the indexed knowledge base and outputs:

- Query embedding timing.
- Vector search timing.
- Combined retrieval scores.
- Vector similarity.
- Metadata score.
- Evidence score.
- Source document.
- Section.
- Semantic type.
- Retrieved content.

After retrieval, the tool generates and displays the answer-generation prompt containing the selected evidence.

This makes it possible to manually evaluate whether the retrieved project sections are relevant and whether the generated prompt provides sufficient evidence before invoking the LLM.

The tool is intended for development and evaluation rather than as a production-facing application.

---

### Semantic Retrieval

A user question is converted into an embedding and compared against the stored document embeddings using pgvector.

The retrieval output currently exposes the top-ranked results for inspection.

Each result contains:

- Similarity score.
- Source project.
- Section.
- Retrieved content.
- Associated project metadata.

The retrieval layer is intentionally exposed for inspection so that retrieval quality can be evaluated before relying on the LLM generation stage.

The current test output retrieves the top 10 results before selecting the top 5 results as context for the LLM.

The retrieval implementation does not currently treat the similarity score as a definitive relevance decision.

---

### LLM Integration

The backend uses a provider-independent `ILLMClient` abstraction.

Configured providers are instantiated through `LlmClientFactory`.

The current provider configuration supports:

- Google
- Groq
- OpenRouter

Each provider can contain multiple models.

The fallback chain is controlled by configuration order rather than hardcoded provider logic.

For example:

`Google / Model A` -> `Google / Model B` -> `Groq / Model A` -> `Groq / Model B` -> `OpenRouter / Model A` -> `OpenRouter / Model B`

The concrete clients are responsible for provider-specific HTTP requests, authentication, request/response formats, and error handling.

`FallbackLlmClient` is responsible for executing the configured clients in order and falling back when a provider/model fails.

---

### LLM Configuration

LLM generation configuration is represented through `LlmOptions` and `LlmProviderOptions`.

Shared settings include:

- Maximum output tokens.
- Thinking level / Reasoning effort.

Provider configuration includes:

- Provider name.
- Ordered models.
- Timeout.

API keys are supplied through environment variables rather than being stored in source-controlled configuration.

The `.env` file is loaded during application configuration through `AppConfiguration`.

The environment-based configuration allows provider credentials to remain separate from the source-controlled project configuration.

---

### Data Flow

The current ingestion flow is:

`Markdown files` -> `MarkdownDocumentLoader` -> `FrontmatterParser` -> `Section chunking` -> `Metadata propagation` -> `Embedding generation` -> `PostgreSQL + pgvector`

The current query flow is:

`User question` -> `Optional English query translation (locale nb)` -> `Query embedding` -> `pgvector similarity search` -> `Top-10 chunks` -> `Top-5 prompt context`

Knowledge chunks remain English. Locale `nb` requests a fluent Norwegian Bokmål answer from the LLM; locale `us` requests American English. The answer-prompt template itself stays English.

The LLM flow is:

`Prompt/context` -> `Configured provider/model chain` -> `LLM client` -> `Provider/model fallback` -> `Generated response`

The evaluation flow is:

`Evaluation question` -> `Query embedding` -> `Vector search` -> `Ranked retrieval results` -> `Generated answer prompt` -> `Manual inspection`

The complete implemented workflow is:

`Job description/question` -> `Query embedding` -> `Semantic retrieval` -> `Top-ranked project evidence` -> `Prompt context selection` -> `LLM fallback chain` -> `Grounded candidate response + source references`

---

## Technical Decisions

### Decision: Markdown as the Source of Truth

#### Context

Project knowledge needs to remain easy to maintain, version controlled, readable, and editable without database tooling.

#### Chosen Solution

Project documentation is maintained as Markdown files in the repository.

The KnowledgeIndexer processes these documents and generates the retrieval representation in PostgreSQL.

PostgreSQL is therefore treated as a generated index rather than the authoritative knowledge store.

#### Alternatives Considered

- Database-driven knowledge management.
- JSON documents.
- Manually maintained prompt context.

#### Trade-offs

Markdown provides:

- Human-readable documentation.
- Git-based version control.
- Easy review.
- Easy editing.
- Simple rebuilding of the retrieval index.

The trade-off is that changes to the source documents require the retrieval index to be regenerated.

---

### Decision: YAML Frontmatter for Structured Metadata

#### Context

Important project information should be available independently of the prose.

Technologies, roles, status, organization, and project periods should not have to be inferred from natural-language text.

#### Chosen Solution

YAML frontmatter is used for structured project metadata.

The metadata is parsed during ingestion and propagated to every chunk originating from the project.

#### Alternatives Considered

- Extracting metadata from prose.
- Maintaining metadata exclusively in PostgreSQL.
- Separate JSON metadata files.

#### Trade-offs

The approach provides explicit, version-controlled metadata that can be used by both the ingestion and retrieval layers.

The trade-off is that frontmatter must remain syntactically valid and changes require re-indexing.

The metadata schema also needs stronger validation in a future iteration.

---

### Decision: Section-Based Chunking

#### Context

Whole-document embeddings would be too broad for precise retrieval, while arbitrary fixed-size chunks could split concepts at inappropriate boundaries.

#### Chosen Solution

Documents are initially divided according to Markdown headings.

Each section becomes a retrieval unit together with its source project and metadata.

The section heading itself is stored separately as the chunk's `Section` metadata rather than being required as part of the cleaned chunk content.

#### Alternatives Considered

- Whole-document embeddings.
- Fixed character-length chunks.
- Token-based chunks.
- Paragraph-based chunks.

#### Trade-offs

Section-based chunking preserves the structure already present in the documentation and makes retrieval results easy to understand.

The trade-off is that very large sections may still be too broad and may require secondary splitting later.

---

### Decision: PostgreSQL with pgvector

#### Context

The project needs semantic similarity search while also requiring structured project metadata.

The current knowledge base does not justify introducing a separate vector database.

#### Chosen Solution

PostgreSQL stores the chunks, metadata, and embedding vectors.

pgvector provides vector similarity search.

#### Alternatives Considered

- PostgreSQL full-text search.
- Keyword-only search.
- Dedicated vector databases.
- Hybrid search.

#### Trade-offs

Using PostgreSQL provides:

- One persistence system.
- Structured and vector data in the same database.
- Straightforward development and inspection.
- Easy access to metadata during retrieval.

The main limitation is that vector similarity alone is not sufficient for every candidate-matching query.

Exact technologies, identifiers, and highly specific requirements may be better handled using metadata or lexical search.

---

### Decision: Local Embeddings During Development

#### Context

The development environment needs repeatable embeddings without introducing external API costs or sending personal project information to a hosted embedding provider.

#### Chosen Solution

BAAI `bge-small-en-v1.5` is executed locally through Ollama.

The same model generates both document and query embeddings.

#### Alternatives Considered

- Hosted embedding APIs.
- OpenAI embeddings.
- Azure-hosted embedding services.

#### Trade-offs

Local embeddings provide:

- No embedding API cost during development.
- Local processing.
- Reproducible development.
- Control over the embedding pipeline.

The trade-offs are dependency on local model infrastructure and potentially lower inference performance depending on available hardware.

Changing the embedding model requires re-indexing the knowledge base.

---

### Decision: Keep Retrieval Separate from LLM Generation

#### Context

It is difficult to determine whether a poor AI response is caused by retrieval quality or by the generation model when both systems are introduced simultaneously.

#### Chosen Solution

The retrieval pipeline is implemented and evaluated independently from the LLM provider layer.

The `CandidateConsoleAssistant` exposes the retrieved project sections, ranking information, similarity scores, and generated answer prompt so that retrieval quality and prompt construction can be inspected directly.

The LLM generation prompt is designed to require the model to stay within the evidence retrieved for each question and to explicitly acknowledge insufficient evidence.

#### Alternatives Considered

- Implementing retrieval and LLM generation simultaneously.
- Sending large parts of the knowledge base directly into the LLM.
- Evaluating only the final generated response.

#### Trade-offs

Separating retrieval makes the system easier to debug and evaluate.

The trade-off is that the complete candidate-assistant workflow takes longer to implement because generation is deliberately treated as a separate layer.

---

### Decision: Provider-Independent LLM Client Architecture

#### Context

The project should not become tightly coupled to one commercial LLM provider.

Different providers have different pricing, model availability, rate limits, API formats, and failure modes.

#### Chosen Solution

A common `ILLMClient` interface is used by provider-specific clients.

The current implementations include:

- `GoogleClient`.
- `GroqClient`.
- `OpenRouterClient`.
- `CerebrasClient` retained as an optional client for future use.

Provider-specific HTTP and response handling remains inside each concrete client.

`LlmClientFactory` creates the configured provider/model clients.

`FallbackLlmClient` handles execution order and fallback behavior.

#### Trade-offs

The architecture makes providers interchangeable and allows multiple models to be configured without changing application-level code.

The trade-off is additional abstraction and configuration complexity.

---

### Decision: Model-Level and Provider-Level Fallback

#### Context

A provider can fail because of an invalid key, unavailable model, quota, rate limiting, or service failure.

A provider may also have multiple models that should be tried independently.

#### Chosen Solution

Providers contain ordered model lists.

The fallback chain evaluates each provider/model combination independently.

For example:

`Gemini / Model A` -> `Gemini / Model B` -> `Groq / Model A` -> `Groq / Model B` -> `OpenRouter / Model A` -> `OpenRouter / Model B`

The fallback layer records the provider and model for every attempt.

Transient failures are distinguished from non-transient failures through `LlmProviderException`.

#### Alternatives Considered

- One model per provider.
- Provider-only fallback.
- Hardcoded model fallback inside every concrete client.
- External retry infrastructure.

#### Trade-offs

The configuration is flexible and the fallback behavior is centralized.

The trade-off is that the number of possible attempts increases as more models are configured, which can increase latency when several providers/models fail sequentially.

---

### Decision: Environment-Based LLM Configuration

#### Context

API credentials must not be committed to source control, while provider/model configuration should be easy to change between development environments.

#### Chosen Solution

LLM provider and model configuration is loaded through `IConfiguration`.

API keys are supplied through environment variables and `.env` during local development.

The provider/model structure is represented through `LlmOptions` rather than separate hardcoded configuration properties for every model.

#### Alternatives Considered

- Hardcoding API keys.
- Storing secrets in source-controlled `appsettings.json`.
- One configuration class per provider/model.
- Hardcoding fallback order in `LlmClientFactory`.

#### Trade-offs

Environment-based configuration keeps secrets outside the repository and makes model fallback order configurable.

The trade-off is that local development requires correct environment configuration, and configuration errors can be harder to diagnose without explicit startup validation.

---

# Implementation

The implementation covers the Markdown ingestion and chunking foundation, PostgreSQL/pgvector persistence, semantic retrieval, provider-independent LLM integration, multi-model/provider fallback, grounded answer generation, and a console-based retrieval evaluation workflow.

The main implementation flow is:

1. Project Markdown files are stored under `knowledge/projects`.
2. The `MarkdownDocumentLoader` recursively discovers Markdown documents.
3. Template files are excluded from ingestion.
4. The `FrontmatterParser` extracts YAML metadata.
5. The document is separated into metadata and Markdown content.
6. The content is split into sections based on Markdown headings.
7. Section headings are stored as section metadata.
8. Empty sections are ignored after content cleaning.
9. Project metadata is propagated to each generated chunk.
10. Generated chunks are validated and can be inspected through the indexer.
11. Each chunk is passed through the local embedding model.
12. The resulting 384-dimensional vectors are stored in PostgreSQL.
13. pgvector provides vector similarity search over the stored embeddings.
14. A user question is converted into an embedding using the same model. For locale `nb`, the question is translated to English before this step.
15. The query vector is compared against the stored document vectors.
16. The top 10 ranked chunks are retrieved.
17. The top 5 retrieved chunks are selected as prompt context.
18. Retrieved evidence is supplied to the configured LLM.
19. Retrieval sources are returned alongside the generated answer.
20. Source URLs point directly to the corresponding project Markdown file in the GitHub repository.
21. The `CandidateConsoleAssistant` can run representative retrieval evaluation questions independently of the API.
22. The evaluation tool displays retrieval timing, scores, source sections, semantic types, and retrieved content.
23. The evaluation tool generates and displays the answer-generation prompt for manual inspection.
24. The `LlmClientFactory` creates configured provider/model clients.
25. `FallbackLlmClient` attempts each configured provider/model in order.
26. Provider/model failures are logged with provider, model, HTTP status, and timing.
27. A successful provider/model terminates the fallback chain.
28. If all configured provider/model combinations fail, the fallback client returns an aggregated failure.

The current pipeline is:

`Markdown` -> `Frontmatter parsing` -> `Document loading` -> `Section chunking` -> `Metadata propagation` -> `Embedding generation` -> `PostgreSQL + pgvector` -> `Optional English query translation` -> `Query embedding` -> `Similarity search` -> `Top-10 retrieved chunks` -> `Top-5 prompt context` -> `LLM provider/model fallback` -> `Generated answer in the requested language + source references`

The evaluation pipeline is:

`Evaluation question` -> `Query embedding` -> `Vector search` -> `Ranked retrieval results` -> `Generated answer prompt` -> `Manual inspection`

The current implementation intentionally does not treat the similarity score as a definitive relevance decision.

---

### LLM Providers

The current LLM configuration supports multiple providers and models.

Google, Groq and OpenRouter can contain multiple models.

Cerebras is retained as a provider client for possible future use but is not currently part of the active fallback configuration.

The provider/model order is configuration-driven.

A provider can therefore be changed or removed without changing the fallback architecture itself.

---

# Result

The ingestion, retrieval, evaluation, and LLM integration workflow is operational.

The system can currently:

- Discover Markdown project documentation recursively.
- Exclude template files.
- Parse YAML frontmatter.
- Extract structured project metadata.
- Split documents into semantic sections.
- Remove Markdown section headings from chunk content.
- Ignore empty sections.
- Propagate project metadata to generated chunks.
- Validate generated chunks.
- Configure PostgreSQL with pgvector.
- Store document chunks, metadata, and 384-dimensional embeddings.
- Generate 384-dimensional embeddings locally.
- Perform vector similarity search using pgvector.
- Generate embeddings for natural-language queries.
- Retrieve semantically related project sections.
- Select the highest-ranked retrieved sections as LLM context.
- Inspect retrieval results independently of LLM generation.
- Inspect similarity scores, source sections, and associated project metadata.
- Run representative retrieval evaluation questions through `CandidateConsoleAssistant`.
- Inspect retrieval timing, ranking information, and retrieved evidence.
- Inspect the generated answer-generation prompt before LLM execution.
- Connect to multiple LLM providers.
- Configure multiple models per provider.
- Fall back between models within a provider.
- Fall back between providers.
- Log provider/model attempts and failures.
- Continue past invalid API keys and unavailable models when configured as fallback candidates.
- Generate answers grounded in retrieved project evidence.
- Return retrieved source information alongside generated answers.
- Provide direct GitHub URLs to the source Markdown files used as evidence.

The current KnowledgeIndexer output can be used to verify the ingestion pipeline by inspecting discovered documents, parsed project titles, generated sections, chunk content, and index statistics.

The `CandidateConsoleAssistant` provides a separate manual evaluation path for inspecting retrieval results and the generated answer prompt using representative candidate-oriented questions.

The Markdown knowledge base remains the source of truth, while PostgreSQL/pgvector acts as generated retrieval infrastructure.

During LLM integration testing, invalid Google, OpenRouter and Groq credentials were correctly detected and skipped, while valid provider credentials allowed a later provider in the fallback chain to succeed.

The fallback implementation also exposed a practical problem with free-tier OpenRouter model identifiers: a configured model can exist in configuration while no longer being available under that free endpoint. The HTTP response and provider error therefore need to be treated as part of runtime model availability rather than assuming that a `:free` model slug will remain valid indefinitely.

Current retrieval tests show similarity scores approximately in the `0.58-0.82` range depending on the query and retrieved content.

These scores are useful for comparing retrieval results but should not be interpreted as probabilities or as a universal relevance threshold.

The retrieval workflow retrieves the top 10 results and selects the top 5 results as context for answer generation.

Retrieval evaluation has been performed manually using representative candidate-oriented questions covering areas including:

- ASP.NET Core.
- Azure authentication.
- CI/CD.
- PostgreSQL.
- pgvector.
- .NET with PostgreSQL.
- Databases.
- ERP systems.
- PostgreSQL production usage.
- Platform Engineering responsibilities involving software development, developer experience, internal developer platforms, Kubernetes, IaC, CI/CD, automation, and hybrid on-prem/cloud environments.

The answer-generation prompt has also been refined to reduce unsupported claims. In particular, it explicitly distinguishes between evidence that a technology was used and evidence that it was used in production.

The current evaluation is manual. There is not yet an automated retrieval evaluation dataset, automated top-N comparison, metadata-aware ranking implementation, hybrid search implementation, or reranking layer.

The completed backend therefore provides a working ingestion, retrieval, retrieval-evaluation, and grounded answer-generation workflow together with configurable multi-model/provider fallback. Retrieval optimization and a future candidate-facing interface remain outside the scope of the completed backend submission.

---

# Lessons Learned

## Retrieval Quality Is More Important Than the LLM Layer

A sophisticated LLM cannot compensate for consistently poor retrieval.

If the wrong project or section is supplied as context, the generation layer can produce an answer that sounds convincing while still being unsupported by the candidate's actual experience.

The retrieval layer therefore needs to be evaluated independently before relying on generated answers.

---

## Source Data Should Remain Human-Maintained

Keeping project knowledge in Markdown makes the source material easy to read, review, update, and version control.

The database can then be treated as disposable generated infrastructure.

This is preferable to making the vector database the authoritative source because the complete knowledge base can be rebuilt when the indexing strategy changes.

---

## Metadata Should Not Be Inferred When It Can Be Declared

A technology mentioned in a document does not necessarily mean that the project implemented that technology.

Explicit frontmatter makes this distinction clearer.

For example, the project's `technologies` field represents technologies declared as part of the project's implemented technology stack rather than technologies merely mentioned somewhere in the documentation.

This provides a much stronger foundation for future technology-aware retrieval.

---

## Prompt-Level Evidence Constraints Matter

Retrieval quality and answer quality are related but separate problems.

Even when the retrieved context is relevant, an LLM can make an unsupported inference if the answer prompt does not explicitly constrain it.

One example is production experience: evidence that PostgreSQL was used in a project does not by itself prove that PostgreSQL was used in a commercial production environment.

The answer prompt therefore needs to distinguish between:

- Evidence that a technology was used.
- Evidence that a technology was used in production.
- Evidence that a responsibility was actually performed.
- Evidence that a claim is only implied rather than explicitly supported.

This reduces the risk of technically plausible but unsupported candidate claims.

---

## Chunking Is a Retrieval Design Decision

The chunking strategy directly affects retrieval quality.

Whole-document embeddings provide too little precision, while overly small chunks can lose the context required to understand a technical decision or project result.

Using Markdown headings as initial boundaries provides a practical balance because the documentation structure already reflects meaningful semantic units.

The strategy should still evolve as retrieval evaluation produces more evidence.

---

## Vector Similarity Is Not a Complete Matching Strategy

Semantic search is useful for finding conceptually related project experience, but it is not sufficient for every candidate-oriented question.

Exact technology requirements, specific roles, organizations, project status, and other structured constraints are better represented through metadata.

This makes hybrid retrieval or metadata-aware ranking a natural next step.

---

## Similarity Scores Are Not Probabilities

A similarity score such as `0.8` does not mean that a result is 80% relevant.

The value depends on the embedding model, vector distribution, query, corpus, and similarity metric.

Retrieval quality should therefore be evaluated using actual questions and expected results rather than relying on an arbitrary score threshold.

---

## Manual Retrieval Testing Is Useful but Limited

Manual inspection is useful during development because it makes it possible to see which project sections are being retrieved and whether the generated answer remains grounded in those sections.

The `CandidateConsoleAssistant` makes this evaluation repeatable by running representative questions and displaying retrieval results and the generated answer prompt.

However, manual testing alone is not enough to determine whether a retrieval change consistently improves the system.

A larger evaluation dataset and automated regression tests are therefore future improvements rather than current functionality.

---

## LLM Provider Availability Is Not the Same as Model Availability

An API provider can be reachable and a valid API key can exist while a specific model is unavailable, restricted, renamed, deprecated, or no longer available through a particular tier.

Testing showed this explicitly with OpenRouter free-model identifiers.

The fallback architecture therefore needs to treat provider/model availability as a runtime concern rather than assuming that a configured model name guarantees successful generation.

---

## Fallback Order Is an Operational Decision

Adding more models and providers increases resilience but also increases potential latency when several attempts fail sequentially.

The fallback list should therefore not simply contain every available model.

The order should reflect:

- Expected availability.
- Response latency.
- Model quality.
- Cost.
- Rate limits.
- Reliability.
- Whether the model satisfies the project's generation requirements.

A large fallback chain is not automatically better than a smaller, well-ordered one.

---

# Future Improvements

## Retrieval

- Introduce metadata-aware filtering.
- Improve metadata-aware ranking by distinguishing descriptive and structured metadata.
- Introduce metadata intent detection so structured attributes such as environment and organization can be matched explicitly against query intent.
- Improve reranking for queries containing explicit metadata constraints.
- Compare vector-only retrieval against hybrid retrieval.
- Add lexical search for exact technologies and identifiers.
- Add project-level result aggregation.
- Add result deduplication.
- Evaluate reranking strategies.
- Consider query rewriting where appropriate.
- Evaluate different top-N values using a retrieval evaluation dataset rather than choosing top-k based only on manual inspection.

## Evaluation

- Create a representative retrieval evaluation dataset.
- Define expected projects and sections for each test question.
- Measure top-k retrieval precision.
- Track false-positive retrievals.
- Add automated retrieval regression tests.
- Compare retrieval strategies quantitatively rather than relying only on manual inspection.
- Evaluate whether fewer than the current top-ranked results provide sufficient evidence for specific question types.
- Add automated LLM answer-grounding evaluation.
- Test fallback behavior automatically using mocked provider failures.
- Measure end-to-end latency across different fallback chains.

## LLM Integration

- Add configurable retry policies for transient failures.
- Distinguish provider authentication failures, model availability failures, rate limits, quota failures, and server errors more explicitly.
- Add provider/model availability checks where appropriate.
- Improve source references in generated answers.
- Improve grounded candidate response generation.
- Prevent unsupported claims when the knowledge base does not contain sufficient evidence.
- Add job-description extraction.
- Implement candidate-to-job matching.
- Separate retrieval confidence from generation quality.
- Add structured LLM response formats where appropriate.

## Knowledge Management

- Add stronger frontmatter schema validation.
- Detect invalid or incomplete project metadata during indexing.
- Automatically re-index changed documents.
- Detect deleted or renamed source documents.
- Track knowledge-base versioning.
- Add index rebuild commands.

## Infrastructure

- Containerize the complete development environment.
- Add automated indexing to the development workflow.
- Replace primarily console-based logging with structured logging.
- Add observability around ingestion, retrieval, and LLM fallback.
- Add production secret management rather than relying on `.env` outside local development.
- Add production deployment when the retrieval and generation workflow is sufficiently validated.
- Develop a candidate-facing web interface, potentially using React, as a separate frontend project.
- Expose the completed backend through a public chat-oriented application in a future iteration.
- Add authentication and access control if the assistant is made publicly accessible.

---