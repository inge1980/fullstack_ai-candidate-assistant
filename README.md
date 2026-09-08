# M.I.N.D - My Indexed Knowledge Directory

M.I.N.D is a personal AI knowledge assistant built to represent a developer's experience, projects, technical decisions, and professional background.

The system uses Retrieval-Augmented Generation (RAG) to combine semantic search with a Large Language Model (LLM). Knowledge is stored as Markdown files, indexed using embeddings, stored in PostgreSQL with pgvector, and retrieved dynamically when answering questions.

The Markdown knowledge base remains the Single Source of Truth. PostgreSQL is used as a generated retrieval index that can be discarded and rebuilt whenever the knowledge structure, chunking strategy, metadata schema, or embedding model changes.

The goal is to create an AI assistant that can answer questions about a candidate's:

* Projects
* Technical experience
* Architecture decisions
* STAR stories
* CV and career history
* Engineering opinions and preferences

Retrieved evidence is manually evaluated to determine whether it is sufficient to support claims made in generated answers.

---

# Features

## Knowledge Management

* Markdown-based knowledge source (Single Source of Truth)
* YAML frontmatter for structured project metadata
* Structured knowledge about projects, experience, and technical decisions
* Recursive Markdown document discovery
* Template document exclusion
* Semantic section-based document chunking
* Project metadata propagation to generated chunks
* Automatic indexing from Markdown to PostgreSQL
* Rebuildable PostgreSQL retrieval index

## Semantic Search (RAG)

* Generate 384-dimensional embeddings using BAAI/bge-small-en-v1.5
* Generate embeddings locally through Ollama
* Store embeddings in PostgreSQL using pgvector
* Generate query embeddings using the same embedding model
* Perform semantic similarity search against indexed knowledge
* Retrieve top-ranked project sections
* Build relevant context before sending it to the LLM
* Return source URLs pointing directly to the corresponding GitHub project Markdown files

## AI Assistant

* Candidate-oriented question answering
* Job-description oriented knowledge retrieval
* Uses retrieved knowledge instead of sending the entire knowledge base
* Generates answers from retrieved project evidence
* Explicitly handles insufficient evidence
* Constrains answers against unsupported claims
* Provider-independent LLM abstraction
* Multiple LLM providers
* Multiple models per provider
* Model-level fallback
* Provider-level fallback
* Returns source URLs for inspecting the original project documentation

## RAG Evaluation Tool

* Standalone console tool for evaluating retrieval quality
* Generate query embeddings for candidate-oriented questions
* Display vector similarity scores
* Display metadata and evidence scores
* Display combined relevance scores
* Display top-ranked retrieved project sections
* Inspect source documents, section headings, semantic types, and metadata
* Generate and inspect the LLM answer prompt
* Manually evaluate whether retrieved evidence is sufficient to support an answer

## LLM Provider Fallback

* Multiple models per provider
* Sequential fallback across configured provider/model combinations
* Continue fallback after provider/model failures
* Provider/model failure logging
* HTTP status and transient failure information
* Support for Google
* Support for Groq
* Support for OpenRouter
* Optional Cerebras client retained for future use

## Architecture

* Markdown knowledge base as the source of truth
* ASP.NET Core backend as the application orchestration layer
* PostgreSQL + pgvector as the generated retrieval index
* Ollama for local embedding generation
* Provider-independent LLM integration layer
* Configuration-driven provider/model fallback
* Docker Compose for local infrastructure
* Standalone console tools for indexing and retrieval evaluation

---

# Technologies

## Backend

* C#
* .NET / ASP.NET Core Web API
* REST API
* Dependency Injection
* Environment-based application configuration

## AI / RAG

* Retrieval-Augmented Generation (RAG)
* BAAI/bge-small-en-v1.5 embeddings via Ollama
* Semantic search
* Document ingestion
* Section-based text chunking
* YAML metadata extraction
* Metadata-aware retrieval foundation

## LLM Providers

* Google API
* Groq API
* OpenRouter API

## Database

* PostgreSQL
* pgvector for vector storage and similarity search
* JSONB for project metadata
* 384-dimensional embedding vectors

## Infrastructure

* Docker
* Docker Compose
* Ollama

---

# Knowledge Base Structure

Each project is represented by a Markdown document containing YAML frontmatter and structured project documentation.

The frontmatter contains information such as:

* Title
* Organization
* Role
* Environment
* Period
* Status
* Technologies
* Concepts
* Links

The Markdown content contains sections such as:

* Overview
* Context
* Task
* Challenges
* Results
* Technical Decisions
* Implementation
* Lessons Learned
* Future Improvements

The Markdown files are version controlled and remain the authoritative source.

---

# How It Works

The system follows a Retrieval-Augmented Generation (RAG) pipeline with three distinct stages.

## 1. Knowledge Indexing

1. Project knowledge is maintained as Markdown documents with YAML frontmatter.
2. The KnowledgeIndexer discovers and parses the documents.
3. The content is split into semantic sections and enriched with project metadata.
4. A 384-dimensional embedding is generated for each document chunk.
5. The chunks, metadata, and embeddings are stored in PostgreSQL using pgvector.

## 2. Semantic Retrieval

1. A user question is converted into an embedding using the same embedding model.
2. pgvector performs semantic similarity search.
3. The most relevant project sections and their metadata are retrieved as evidence.

## 3. Answer Generation

1. The retrieved evidence is supplied to the LLM.
2. The configured provider/model fallback chain attempts the request until a provider succeeds or all configured options fail.
3. The LLM generates a candidate-oriented response based on the retrieved evidence.
4. The API returns the generated answer together with the retrieved sources, including a GitHub URL to the corresponding project Markdown file.

---

# How to Run

## 1. Start infrastructure

Make sure a Docker runtime is installed and running.

Then start PostgreSQL with pgvector:

    docker compose up -d

---

## 2. Install Ollama and embedding model

Install Ollama and make sure the embedding model is available:

    ollama pull bge-small-en-v1.5

The project uses `bge-small-en-v1.5` for both document and query embeddings.

The model generates 384-dimensional vectors.

---

## 3. Build knowledge index

Import the Markdown knowledge base and generate the retrieval index:

    dotnet run --project src/tools/KnowledgeIndexer

The indexer will:

1. Discover Markdown files recursively.
2. Exclude template documents.
3. Parse YAML frontmatter.
4. Extract project metadata.
5. Split documents into semantic sections.
6. Clean generated section content.
7. Ignore empty sections.
8. Propagate project metadata to each chunk.
9. Generate embeddings.
10. Store chunks, metadata, and vectors in PostgreSQL.
11. Output the generated index structure for manual inspection.

The generated PostgreSQL index can be rebuilt from the Markdown knowledge base.

---

## 4. Evaluate RAG Retrieval and Evidence

The `CandidateConsoleAssistant` is a standalone console tool for manually evaluating RAG retrieval and generated answer prompts.

Run:

    dotnet run --project src/tools/CandidateConsoleAssistant

The evaluation tool:

1. Takes a candidate-oriented question as input.
2. Generates an embedding for the question.
3. Performs vector similarity search against the indexed knowledge base.
4. Calculates and displays combined relevance information, including vector, metadata, and evidence scores.
5. Displays the top 10 retrieved results for manual inspection.
6. Shows the source project, section, semantic type, retrieved content, and project metadata for each result.
7. Generates the LLM answer prompt using the retrieved evidence.
8. Displays the generated prompt for inspection before LLM execution.

The tool is used to evaluate whether the retrieved project evidence is relevant and sufficient to support an answer before relying on the generated response.

Example questions can cover:

* Relevant projects for a specific job role
* Technologies and technical experience
* Production versus non-production experience
* Specific responsibilities
* Infrastructure, CI/CD, automation, and cloud experience
* Experience spanning multiple projects

The evaluation is intentionally manual. The output makes it possible to inspect retrieval quality, identify false-positive results, compare relevance scoring, and verify that the generated prompt contains appropriate evidence without requiring a separate frontend.

---

## 5. Start backend API

Run the ASP.NET Core API:

    dotnet run --project src/backend/Api

---

## 6. Use API with Swagger

The API can be tested manually using Swagger UI:

    http://localhost:5179/swagger/index.html

Endpoint:

    POST /api/v1/Questions

Example request:

    {
      "question": "What experience do I have with PostgreSQL and pgvector?"
    }

The API workflow is:

1. Receive the question.
2. Generate an embedding for the question.
3. Search relevant knowledge using pgvector.
4. Retrieve the highest-ranked project sections.
5. Build the LLM prompt based on a set of instructions, the question and the retrieved evidence.
6. Send the prompt to the configured LLM.
7. Use the configured fallback chain if the selected provider/model fails.
8. Return the generated answer, together with evidence chunks, similarity scores, and source URLs.

---

# Semantic Retrieval

The retrieval system uses the same embedding model for both indexed documents and user queries, ensuring they occupy the same embedding space.

The query flow is:

1. A user question is converted into an embedding.
2. pgvector performs semantic similarity search against the indexed document embeddings.
3. The top-ranked project sections and their metadata are retrieved.
4. The retrieved context is supplied to the LLM.

Each retrieved result contains:

- Similarity score
- Source document
- Project
- Section
- Retrieved content
- Project metadata
- Source URL

The current test workflow exposes the top 10 retrieved results for manual inspection.

Retrieval evaluation also exposes:

- Vector similarity score
- Metadata score
- Evidence score
- Combined relevance score

Similarity scores are used to compare retrieval results, but they are not treated as probabilities or as a universal relevance threshold.

---

# Metadata-Aware Retrieval

Project metadata is stored in YAML frontmatter and propagated to every generated chunk.

This provides a foundation for future metadata-aware retrieval.

Metadata can be used for:

* Technology filtering
* Role filtering
* Organization filtering
* Environment filtering
* Project status filtering
* Project-level ranking
* Retrieval explanations
* Candidate-to-job matching

The system intentionally does not rely exclusively on semantic interpretation of the project documentation.

For example, evidence that PostgreSQL was mentioned in a project does not automatically prove that it was used in production.

---

# LLM Integration

The backend uses a provider-independent `ILLMClient` abstraction.

Provider-specific clients are responsible for:

* HTTP requests
* Authentication
* Request formatting
* Response parsing
* Provider-specific error handling

The current clients include:

* `GoogleClient`
* `GroqClient`
* `OpenRouterClient`
* `CerebrasClient` as an optional client for future use

`LlmClientFactory` creates provider/model clients based on configuration.

`FallbackLlmClient` controls the execution order and fallback behavior.

---

# LLM Configuration

LLM configuration is represented through:

* `LlmOptions`
* `LlmProviderOptions`

The provider and model configuration is defined in `/appsettings.json`.

Shared configuration includes:

* Maximum output tokens
* Thinking level
* Reasoning effort

Each configured provider defines:

* Provider name
* Ordered model list
* Request timeout

The active provider configuration includes:

* OpenRouter
* Google
* Groq

Cerebras is configured as a supported client but is not currently included in the active provider list.

API keys are supplied through environment variables rather than stored in source-controlled configuration.

The provider and model configuration is structured as:

* `Llm`
  * `Providers[]`
    * `Name`
    * `Models[]`
    * `TimeoutSeconds`

Providers are evaluated in the order defined in `Providers[]`.

Models are evaluated in the order defined in each provider's `Models[]` collection.

This allows providers and models to be added, removed, or reordered through configuration without changing the provider implementations.

---

# LLM Fallback

The fallback layer evaluates the provider/model combinations defined in `/appsettings.json` sequentially.

The fallback order is determined entirely by the configured order:

1. Providers are evaluated in the order they appear in `Llm.Providers[]`.
2. Models are evaluated in the order they appear in each provider's `Models[]`.
3. A failed provider/model combination causes the fallback layer to continue with the next configured combination.
4. The first successful request ends the fallback process.

For example, the current configuration is evaluated in this general order:

1. OpenRouter / configured model 1
2. OpenRouter / configured model 2
3. Additional OpenRouter models
4. Google / configured model
5. Groq / configured model

The fallback layer logs each provider/model attempt.

Example:

    [LLM] Trying: Google / gemini-3.6-flash

If a request fails:

    [LLM] Failed: Google / gemini-3.6-flash (...) Status=400 Transient=False

If a later provider succeeds:

    [LLM] Provider succeeded: Groq / openai/gpt-oss-120b (...)

The fallback implementation provides:

* Sequential model fallback within a provider
* Sequential provider fallback
* Provider/model failure logging
* HTTP status and transient failure information

---

# Evidence-Based Answer Generation

The system treats retrieval quality and answer quality as separate problems.

Semantic similarity alone does not guarantee that the retrieved evidence is sufficient to support an answer.

The LLM instructions are designed to:

* Use retrieved context as evidence
* Avoid inventing technologies
* Avoid inventing responsibilities
* Avoid inventing project experience
* Avoid inferring production experience without explicit evidence
* Distinguish technology usage from production usage
* State clearly when the available evidence is insufficient

For example:

    Technology used
            !=
    Technology used in production

The system therefore attempts to prevent technically plausible but unsupported candidate claims.

The API also exposes the source URL associated with each retrieved evidence chunk, allowing the underlying Markdown source to be inspected directly.

---

# Technical Decisions

## Markdown as the Source of Truth

Project knowledge is maintained as Markdown files in the repository.

This provides:

* Human-readable documentation
* Git-based version control
* Easy editing
* Easy review
* Simple rebuilding of the retrieval index

PostgreSQL is treated as generated retrieval infrastructure.

If the indexing strategy changes, the database representation can be discarded and rebuilt from the Markdown source.

---

## YAML Frontmatter

YAML frontmatter is used for structured project metadata.

Important information does not have to be inferred from natural-language content.

The metadata is parsed during ingestion and propagated to every generated chunk.

This provides a foundation for metadata-aware retrieval and candidate-to-job matching.

---

## Section-Based Chunking

Documents are initially split using Markdown headings.

Each Markdown section becomes an independent retrieval unit while preserving its position within the source document.

Sections such as:

* Overview
* Context
* Task
* Challenge
* Result
* Technical Decisions
* Lessons Learned
* Future Improvements

become retrieval units.

Nested headings are preserved as section paths. For example:

* `Overview`
* `Lessons Learned > Lesson: Infrastructure Is Part of the Application`
* `Action > Technical Decisions > Decision: PostgreSQL with Docker Compose > Context`

Each generated chunk retains:

* Source document
* Project metadata
* Section heading
* Semantic type
* Section content

This metadata is preserved through indexing and retrieval and is exposed in the API response together with the retrieved content and relevance scores.

Section-based chunking provides more meaningful retrieval boundaries than arbitrary character-based splitting while preserving the structure of the original knowledge document.

Very large sections are currently a known limitation and may require secondary splitting in a future iteration.

---

## PostgreSQL with pgvector

PostgreSQL is used as the retrieval index for the generated knowledge chunks.

The current knowledge base does not justify introducing a separate vector database.

Each stored chunk contains:

* Chunk content
* Source document
* Section and heading
* Project metadata
* Embedding vector

pgvector provides vector similarity search over the stored embeddings.

Keeping the vector and structured project metadata in PostgreSQL allows retrieval results to include both the matched content and the metadata required for candidate-oriented ranking and inspection.

The index is rebuildable from the Markdown knowledge base, making PostgreSQL a generated retrieval layer rather than the source of truth.

---

## Local Embeddings

The development environment generates embeddings locally through Ollama using BAAI `bge-small-en-v1.5`.

The embedding model produces 384-dimensional vectors.

Advantages include:

* No embedding API cost during development
* Local processing without sending knowledge content to an external embedding service
* Reproducible development and testing
* Full control over the embedding pipeline

The same model is used for both document and query embeddings.

Changing the embedding model requires the knowledge base to be re-indexed because the existing vectors are tied to the model and embedding dimensions.

---

## Provider-Independent LLM Architecture

The LLM layer is separated from the application through `ILLMClient`.

This prevents application-level code from becoming tightly coupled to a specific LLM provider.

Provider-specific request handling remains inside concrete clients, while application-level code works against the common interface.

The trade-off is additional abstraction and configuration complexity.

---

## Model and Provider Fallback

Providers contain ordered model lists.

Each provider/model combination is evaluated independently according to the order defined in `appsettings.json`.

This provides resilience against:

* Invalid API keys
* Unavailable models
* Rate limits
* Quota limitations
* Provider failures
* Temporary service failures

The trade-off is increased latency when several fallback candidates fail sequentially.

The fallback order should therefore be based on availability, latency, model quality, cost, rate limits, and reliability rather than simply adding every available model.

---

# Testing and Evaluation

The retrieval system is currently evaluated primarily through the standalone `CandidateConsoleAssistant` console tool.

The evaluation has covered questions involving:

* ASP.NET Core experience
* Azure authentication
* CI/CD experience
* PostgreSQL
* pgvector
* PostgreSQL with .NET
* Database experience
* ERP systems
* Production experience
* Personal and school projects
* Broader backend and API development
* Docker
* Azure
* Terraform
* React and TypeScript
* GDPR and form-builder development
* API and system integrations
* Job-oriented project matching
* Platform Engineering responsibilities involving software development, developer experience, internal developer platforms, Kubernetes, IaC, CI/CD, automation, and hybrid on-prem/cloud environments

The test questions are also used to investigate retrieval and ranking behavior, including:

* Broad technology questions
* Specific implementation details
* Production versus non-production experience
* Multiple projects matching the same technology
* Potential false-positive retrievals
* Job-description-oriented retrieval across multiple requirements

The retrieval output is inspected together with:

* Combined relevance score
* Vector similarity score
* Metadata score
* Evidence score
* Source project
* Section heading
* Semantic type
* Retrieved content
* Project metadata
* Source URL

The current evaluation workflow exposes the top 10 retrieved results for manual inspection and generates the LLM answer prompt using the retrieved evidence.

Metadata-aware scoring and evidence-aware reranking are already implemented and exposed as part of the retrieval evaluation output.

The evaluation is currently manual and does not yet contain a formal automated retrieval benchmark.

---

# LLM Provider Testing

The provider fallback implementation has been tested with provider failures.

Testing has included:

* Invalid API keys
* Unavailable models
* Provider failures
* Successful fallback execution

Invalid Google and Groq credentials were skipped successfully when later fallback candidates were available.

OpenRouter testing also exposed an issue with free model identifiers.

A model can remain configured while no longer being available through a particular endpoint or free tier.

Provider/model availability therefore needs to be treated as a runtime concern rather than assuming that a configured model identifier will always remain valid.

---

# Development Goals

The project demonstrates:

* Retrieval-Augmented Generation (RAG)
* Semantic search
* Local embeddings
* Vector search with PostgreSQL and pgvector
* Document ingestion
* Markdown-based knowledge management
* YAML metadata extraction
* Semantic section-based chunking
* Metadata-aware retrieval
* Evidence-aware reranking
* Evidence-based LLM responses
* Multi-provider LLM architecture
* Model-level fallback
* Provider-level fallback
* Configurable GitHub source references
* Clean backend architecture
* Practical use of LLMs in developer tooling

---

# Future Improvements

## Retrieval

* Introduce stronger metadata-aware filtering and search
* Improve metadata-aware ranking
* Add project-level result deduplication
* Evaluate additional reranking strategies
* Consider query rewriting and model-specific prompt optimization
* Evaluate different top-N values using an evaluation dataset

## Evaluation

* Define expected projects and sections for each test question
* Measure top-k retrieval precision and recall
* Add automated evaluation of generated answers against retrieved evidence
* Track false-positive retrievals
* Test fallback behavior using mocked provider failures
* Measure end-to-end latency across different fallback chains

## LLM Integration

* Add provider/model availability checks where appropriate
* Distinguish authentication failures from model availability failures
* Distinguish rate limits and quota failures
* Include source references directly in generated answer text where appropriate
* Implement candidate-to-job matching

## Knowledge Management

* Detect deleted or renamed source documents
* Add explicit index rebuild commands
* Automatically re-index changed documents

## Infrastructure

* Automate indexing as part of the development workflow
* Replace primarily console-based logging with structured logging
* Add observability around ingestion, retrieval, and LLM fallback
* Add production secret management
* Add production deployment when the retrieval and generation workflow is sufficiently validated

## User Interface and Productization

The current submission is intentionally backend-focused. A future version could extend the API into a publicly accessible candidate assistant with:

* React-based chat interface
* Candidate-oriented user experience
* Job-description input and analysis
* Candidate-to-job matching
* Source references in generated answers
* Authentication and access control
* Production deployment and monitoring

These features are outside the scope of the current Module 4 submission.

---

# Status

The backend project is complete for the scope of the Module 4 submission.

The following components are operational:

* Markdown knowledge ingestion
* YAML frontmatter and project metadata extraction
* Semantic section-based chunking
* Local embedding generation through Ollama
* PostgreSQL and pgvector retrieval
* Metadata-aware retrieval and reranking
* Standalone console-based RAG retrieval evaluation
* Manual retrieval evaluation and inspection
* Evidence-based LLM answer generation
* Configurable multi-provider LLM fallback
* REST API for question answering and retrieval results
* Configurable GitHub source URLs for retrieved project evidence

The `CandidateConsoleAssistant` can be used to inspect retrieved project evidence, relevance scoring, and the generated LLM answer prompt independently of the API.

The API can answer questions using retrieved project evidence and exposes the retrieved sources, source URLs, and relevance information alongside the generated answer.

The GitHub source URL is constructed from configuration values for the repository owner, repository, branch, and projects folder, allowing the source repository location to be changed without modifying the question service implementation.

The current implementation is intentionally focused on the backend ingestion, retrieval, evaluation, and LLM integration required for the project scope.

A future iteration could extend the system into a publicly accessible candidate assistant with a React-based user interface and additional production infrastructure.

---