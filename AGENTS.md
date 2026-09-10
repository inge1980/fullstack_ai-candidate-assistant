# Project map (M.I.N.D)

Working index for agents. Long-form product description: `knowledge/projects/ai-candidate-assistant-rag.md`. Formal brief: `PROJECT.md`. Human README: `README.md`.

**M.I.N.D** (My Indexed Knowledge Directory) is a backend RAG assistant for candidate-oriented questions and job-description matching. Markdown project docs are the source of truth. PostgreSQL/pgvector is a generated retrieval index. The LLM only sees retrieved evidence, not the whole knowledge base.

Four concerns stay separate:

1. Human-maintained Markdown knowledge
2. Embedding and retrieval infrastructure
3. Semantic retrieval and evidence selection
4. LLM generation with provider/model fallback

Do not evaluate only the final answer. Retrieval quality is inspected independently (`CandidateConsoleAssistant`). A plausible answer is not a supported answer: technology used is not the same as used in production.

---

## Run locally

From the repo root:

```text
docker compose up -d
ollama pull qllama/bge-small-en-v1.5
dotnet run --project src/tools/KnowledgeIndexer
dotnet run --project src/tools/CandidateConsoleAssistant
dotnet run --project src/backend/Api
npm install --prefix src/frontend
npm run dev --prefix src/frontend
```

- API: `http://localhost:5179` ? Swagger `/swagger`, `POST /api/v1/Questions`, smoke `GET /api/v1/llm/test`
- UI: `http://localhost:5173` ? Vite proxies `/api` to `http://localhost:5179` (no CORS on the API)
- Indexer `knowledgePath` is `cwd/knowledge/projects` ? run from repo root
- Solution: `fullstack_ai-candidate-assistant.slnx` (.NET 10)
- Shared config: root `appsettings.json` + `.env` via `AppConfiguration` (walks up to the `.slnx`). Copy `.env.example`. Never commit `.env`.

---

## Data flows

Ingestion:

`knowledge/projects/*.md` -> `MarkdownDocumentLoader` -> `FrontmatterParser` -> heading chunking -> strip section heading from content -> skip empty -> copy frontmatter onto each chunk -> Ollama embed (384) -> `VectorStore` upsert -> `document_chunks`

Query (API):

`POST /api/v1/Questions` (`question`, `locale` `us`|`nb`) -> if `nb`, LLM-translate the question to English (`query-translate-prompt-v1.md`) -> query embed (English) -> pgvector cosine search -> `MetadataEvidenceScorer` -> sort by combined score -> select prompt context (`filter-list` with N: one chunk per project, up to N) -> `answer-prompt-v8.md` (`{{question}}` original text, `{{context}}` English chunks, `{{answer_language_instruction}}`) -> `LlmClientFactory` / `FallbackLlmClient` -> answer in the requested language + GitHub source URLs

Eval (console):

question -> same retrieval service -> print scores, sections, semantic types, content -> build and print the answer prompt (no required LLM call for inspection)

Intended retrieval (knowledge doc + console): top **10** from the store, top **5** as LLM context. API default is retrieve **25** then **10** chunks. For `filter-list` with a requested N, retrieve `max(50, n*8)` (cap 100), keep one chunk per project (prefer `overview`), take up to N. Similarity scores are for ranking only, not probabilities or a cutoff (manual tests often land around 0.58?0.82).

---

## Design rules

- Same embedding model for documents and queries. Changing the model or dimensions requires a full re-index.
- Frontmatter `technologies` is the declared stack, not every technology mentioned in prose.
- Answer prompt must refuse unsupported claims (invented tech, responsibilities, projects, production use).
- Knowledge, embeddings, retrieval, and the answer-prompt template stay English. `locale: nb` translates the user question to English before embedding and asks the LLM for fluent Norwegian Bokmål, not a literal translation. `locale: us` skips translation and answers in American English.
- LLM providers are replaceable. Fallback order is `Llm.Providers[]` then each provider's `Models[]`.
- Secrets stay in env (`Google__ApiKey`, `Groq__ApiKey`, `OpenRouter__ApiKey`). Non-secret provider/model lists live in `appsettings.json`.
- Frontend is a Vite client of the API only. No RAG, embeddings, or LLM calls in the browser. No auth or public deploy in this stage.
- Frontend visual QA is manual for now: do not screenshot or browser-click to confirm layout; the user inspects the UI.

---

## Solution layout

```text
appsettings.json          Shared JSON (GitHub URLs, Knowledge, Embeddings notes, Llm providers)
.env.example              Ollama, Postgres, LLM API keys
docker-compose.yml        pgvector/pg17 :5432, db candidate_ai
docker/postgres/init.sql  vector extension, document_chunks, HNSW cosine
knowledge/_template_project.md   Excluded from indexing
knowledge/skills/skills.md       Not indexed (indexer only loads knowledge/projects)
knowledge/projects/*.md          Indexed SSoT documents
src/backend/Api                  ASP.NET Core host
src/backend/Application          Retrieval, questions, prompt templates
src/backend/Infrastructure       Documents, embeddings, pgvector, LLM, config
src/tools/KnowledgeIndexer       Ingest -> embed -> upsert
src/tools/CandidateConsoleAssistant  Manual retrieval + prompt inspection
src/frontend                     Vite + React + TypeScript + Tailwind chat UI
```

### Api (`src/backend/Api`)

| File | Role |
|---|---|
| `Program.cs` | DI, Swagger, controllers, static files, 404 fallback |
| `Controllers/QuestionsController.cs` | `POST /api/v1/Questions` (`includeDebug` adds scores and raw source). `POST /api/v1/Questions/intent` is keyword intent only (no LLM). |
| `Controllers/LlmController.cs` | `GET /api/v1/llm/test` |
| `Properties/launchSettings.json` | http `5179`, https `7277` |
| `wwwroot/404.html` | Fallback page |
| `api.http` | Manual HTTP samples |

`QuestionService` is registered in Api DI but lives under `Application/Questions` with namespace `Api.Services`.

### Application (`src/backend/Application`)

| File | Role |
|---|---|
| `Knowledge/KnowledgeRetrievalService.cs` | Query embed -> vector search -> score -> rank |
| `Knowledge/IKnowledgeRetrievalService.cs` | Retrieval contract |
| `Knowledge/KnowledgeRetrievalResult.cs` | Ranked items (source, heading, semantic type, content, scores) |
| `Questions/QuestionService.cs` | Orchestrates retrieve / prompt / LLM / source URLs. Rule-based `QuestionIntentDetector` runs on the original question. `filter-list` with N uses a wider retrieve window and one chunk per project. |
| `Questions/QuestionIntentDetector.cs` | Keyword intent: `detail`, `list`, `count`, `filter-list`, plus optional `requestedCount` |
| `Questions/PromptContextSelector.cs` | Default top-10 chunks; `filter-list`+N: retrieve `max(50, n*8)` cap 100, one overview (else best) chunk per `source`, take N |
| `Questions/IQuestionService.cs` | Ask contract |
| `Questions/AskQuestionRequest.cs` / `AskQuestionResponse.cs` | API DTOs (`Locale` is `us` or `nb`; response includes `intent`) |
| `Questions/QuestionLocale.cs` | Locale normalize, query-translation flag, answer-language instruction |
| `Questions/QuestionSource.cs` / `QuestionRelevance.cs` | Evidence payload |
| `Questions/QuestionItem.cs` / `QuestionItemStatus.cs` / `QuestionDebugInfo.cs` | Extra question types |
| `Prompts/answer/answer-prompt-v1.md` ? `v7.md` | Prompt history; **runtime is v7** (copied to output) |
| `Prompts/translate/query-translate-prompt-v1.md` | English retrieval query for `nb` |

### Infrastructure (`src/backend/Infrastructure`)

Documents: `MarkdownDocumentLoader` (recursive `*.md`, skip `_template_project.md`), `FrontmatterParser`, `ParsedMarkdown`, `MarkdownDocument`, `DocumentChunker` (ATX headings, nested `HeadingPath`, heading stripped from content), `SemanticTypeResolver`, `DocumentChunk`.

Embeddings: `EmbeddingService` (Ollama `/api/embed` from **env vars**, not the JSON `Embeddings` section), `VectorStore` (upsert on `id`; cosine `1 - (embedding <=> q)`), `SearchResult`.

Scoring: `MetadataEvidenceScorer` (term overlap on metadata + content -> combined score). `IReranker` / `RerankResult` exist; retrieval uses the scorer, not a cross-encoder. Stronger metadata filtering and hybrid/lexical search are still future work.

LLM: `ILLMClient`, `LlmClientFactory` (flatten providers x models), `FallbackLlmClient` (log try/fail/success, HTTP status, transient flag via `LlmProviderException`). Google class is `GoogleClient` in file `GeminiClient.cs`. Also `GroqClient`, `OpenRouterClient`. `CerebrasClient` exists but is not in the active `appsettings` provider list.

Config: `Configuration/AppConfiguration.cs`.

### Tools

- **KnowledgeIndexer:** load -> chunk -> embed -> `InsertAsync`. Prints document/chunk stats. Does not delete rows for removed or renamed files.
- **CandidateConsoleAssistant:** hardcoded eval questions; top 10 retrieval, top 5 prompt context; prints timing, combined/vector/metadata/evidence scores, heading, semantic type, content, and the built prompt.

### Frontend (`src/frontend`)

Vite + React + TypeScript + Tailwind. Chat UI posts `{ question, locale }` (`us` | `nb`) to `POST /api/v1/Questions` via a Vite proxy (`/api` ? `http://localhost:5179`). While typing, the form previews intent via `POST /api/v1/Questions/intent` (same keyword detector, no LLM). Chrome copy uses `i18next` / `react-i18next`. The header language menu uses `country-flag-icons` (US / NO) plus `sr-only` / `aria-label` names (`English (US)`, `Norsk bokmål`). Locale is stored in `localStorage`. Types live in `src/frontend/client/types.ts`; fetch lives in `src/frontend/client/questions.ts`. Assistant answers are rendered with `react-markdown` (no raw HTML). No CORS on the API. Swagger on `:5179` is unchanged.

---

## Knowledge base

Indexed files in `knowledge/projects/`:

- `ai-candidate-assistant-rag.md` (this product)
- `azure-dotnet-devops-demo.md`
- `bootstrap-migration.md`
- `canteen-ordering-system.md`
- `developer-portfolio.md`
- `erp-platform-development.md`
- `gdpr-compliant-form-builder.md`
- `hierarchical-shopping-list-app.md`
- `lost-and-found-api.md`
- `n8n-social-content-generator.md`
- `pim-integration.md`
- `react-hotel-booking-case.md`

Frontmatter (`_template_project.md`): `title`, `organization`, `role`, `environment`, `period`, `status`, `technologies`, `concepts`, `dependencies`, `links`.

Typical sections: Overview, Context, Task, Challenge, Result, Technical Decisions, Implementation, Lessons Learned, Future Improvements. Nested headings become `HeadingPath` (e.g. `Action > Technical Decisions > ?`). Semantic types come from `SemanticTypeResolver`. Very large sections are a known limitation (no secondary split yet).

Chunk id: `{filename}-{index:D3}-{headingSlug}`. Upsert updates the same id; heading edits can orphan old rows.

Postgres `document_chunks`: `id`, `source`, `heading_path`, `semantic_type`, `content`, `metadata` jsonb, `embedding vector(384)`.

---

## Config and LLM fallback

`AppConfiguration` loads `.env` then `appsettings.json` then environment variables.

Embeddings (required env): `OLLAMA_EMBED_URL`, `OLLAMA_EMBEDDING_MODEL`. Postgres: `ConnectionStrings__Postgres`.

`Llm` in `appsettings.json`: `MaxOutputTokens` (completion cap sent to every provider; Groq gpt-oss reasoning tokens count against it), `ThinkingLevel` / `ReasoningEffort`, ordered `Providers[]` with `Name`, `Models[]`, `TimeoutSeconds`. Current active order is Groq, then OpenRouter models, then Google. A configured `:free` OpenRouter slug can be invalid at runtime; treat availability as a runtime concern.

Logs look like `[LLM] Trying: Groq / ?`, `[LLM] Failed: ? Status=400 Transient=False`, `[LLM] Provider succeeded: ?`. First success stops the chain; all failures aggregate.

GitHub source URLs: `GitHub:Owner`, `Repository`, `Branch`, `ProjectsFolder`. `QuestionService.GetProjectUrl` currently inserts an extra `/` before `{id}.md`.

---

## Pitfalls and gaps

- Layers: Api host, Application orchestration, Infrastructure I/O. Console tools are not the REST host.
- API default retrieve-25 / prompt-10 vs console retrieve-10. `filter-list` with N diversifies by `source` and does not yet filter on period or technologies.
- `locale: nb` adds an extra LLM call before retrieval. Chunks stay English. UI code `us` is not ISO 639 (`en`).
- Console eval still uses English questions and the English answer-language instruction.
- Indexer upserts only; wipe the table or Docker volume for a true rebuild after deletes/renames.
- `EmbeddingService` ignores `appsettings.json` `Embeddings` / `Ollama` sections.
- `EmbeddingService` reads `OLLAMA_*` from process env at type init. In console tools, call `AppConfiguration.Build()` before `new EmbeddingService()` so `.env` is loaded. The API already loads config first.
- No automated tests, no retrieval eval dataset, no frontmatter schema validation.
- Frontend visual QA is manual; agents should not use the browser to confirm placement or styling unless asked or a runtime failure cannot be diagnosed from code or logs.
- Out of scope: auth, production deploy, candidate-to-job matching product, hybrid search.

When changing RAG behavior, update this file and, if the product story changed, `knowledge/projects/ai-candidate-assistant-rag.md` (then re-index).
