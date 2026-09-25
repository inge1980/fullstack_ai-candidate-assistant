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

- API: `http://localhost:5179` ? Swagger `/swagger`, `POST /api/v1/Questions` (complete JSON), `POST /api/v1/Questions/progress` (SSE phases + one JSON `result`), smoke `GET /api/v1/llm/test`
- UI: `http://localhost:5173` ? Vite proxies `/api` to `http://localhost:5179` (no CORS on the API)
- Indexer `knowledgePath` is `cwd/knowledge/projects` ? run from repo root
- Solution: `fullstack_ai-candidate-assistant.slnx` (.NET 10)
- Shared config: root `appsettings.json` + `.env` via `AppConfiguration` (walks up to the `.slnx`). Copy `.env.example`. Never commit `.env`.

---

## Data flows

Ingestion:

`knowledge/projects/*.md` -> `MarkdownDocumentLoader` -> `FrontmatterParser` -> heading chunking -> strip section heading from content -> skip empty -> copy frontmatter onto each chunk -> Ollama embed (384) -> `VectorStore` upsert -> `document_chunks`

Query (API):

`POST /api/v1/Questions` (`question`, `locale` `us`|`nb`) -> if `nb`, LLM-translate the question to English (`query-translate-prompt-v1.md`) -> query embed (English) -> pgvector cosine search -> for catalog/broad-experience questions, merge `overview` chunks whose `organization` contains a query token -> when the question resolves named technology slugs (`Prompts/taxonomy/technology-families.md`), also merge `overview` chunks whose `technologies` jsonb contains those slugs or family members -> `MetadataEvidenceScorer` -> sort by combined score -> select prompt context (`list`/`filter-list` with N: one chunk per project, up to N; `list` without N: wider retrieve, one chunk per project; `filter-list`/`count` without N: one chunk per distinct `source` in the retrieve window; catalog and broad-experience questions that name an organization keep only matching `organization`; broad experience: one chunk per `source` unless a retrieved project is named; broad production questions keep only `environment: production`; named technologies: keep exact `technologies` matches first, do not pad with unrelated sources, and skip the LLM when no selected chunk has an exact named-technology match, returning a short grounded refusal that may name family technologies those projects did list as used) -> `answer-prompt.md` (`{{question}}` original text, `{{context}}` English chunks with Organization/Period/Environment/Technologies/Match/Links, `{{answer_language_instruction}}`, `{{tech_list_instruction}}` union vs intersection, `{{tech_match_instruction}}` exact vs related) -> `LlmClientFactory` / `FallbackLlmClient` -> `ListTableLinkRewriter` fills list-table Project cells from Links (GitHub and live on the title; portfolio stays in Article when that column is present) -> answer in the requested language + GitHub source URLs. The chat UI calls `POST /api/v1/Questions/progress` instead: SSE `phase` events (`translating` for `nb`, then `searching`, `writing`, and `trying-another-model` on LLM fallback) and one `result` event with the same JSON as `/Questions`.

Professional catalog questions (`professionally`, `professional`, `profesjonelt`, `profesjonell`, `i arbeid`, `på jobb`, `at work`) drop `Personal Project` and `School Project` before the prompt is built. When one company's selected periods merge into a single span of 2+ years, context includes `OrganizationSpans` (one line per such company, chronological). The answer puts one sentence naming each of those spans above the list table. Year counts stay separate. A company span under 2 years is listed under `ShorterOrganizations`: those projects stay in the table and stay out of that sentence. Environment `demo` does not drop a company project.

Eval (console):

question -> same intent, retrieval limit, and `PromptContextSelector` as the API -> print all ranked hits -> `AnswerPromptFormatter` fills `answer-prompt.md` (no required LLM call). English locale only (no `nb` query translation).

Intended retrieval (knowledge doc + console): top **10** from the store, top **5** as LLM context. API default is retrieve **25** then **10** chunks. Detail questions may then append up to 15 overview chunks for named technologies missing from those 10. For `list` or `filter-list` with a requested N, retrieve `max(50, n*8)` (cap 100), one chunk per project (prefer `overview`), take up to N. For `list` without N (`what projects`, `which projects`, `hvilke/hva slags prosjekter`), retrieve `max(50, 10*8)` (cap 100) and send one chunk per `source`. For `filter-list` or `count` without N, retrieve 25 and send one chunk per distinct `source` in that window. If the question names an organization that appears in retrieved frontmatter (suffixes like `AS` stripped), keep only those projects. Broad experience questions (`have you used`, `what experience`) retrieve `max(50, 10*8)` (cap 100), then send one chunk per `source` (up to 10) unless the question names a retrieved project, in which case the default 10 chunks are kept. Broad production questions (`in production`, `production experience`, `i produksjon`, `produksjonserfaring`) then keep only chunks whose frontmatter `environment` is `production` (including when that question is classified as `filter-list`). When named technologies resolve against `Prompts/taxonomy/technology-families.md`, keep only projects whose `technologies` field matches (union: any named slug; intersection: every named slug). Do not pad with non-matching sources. If no selected chunk has an exact named-technology match, do not call the LLM: return a short grounded refusal. Family members that those projects did list (for example Docker when the question named Kubernetes) may be named as used, without claiming the missing technology. If some named slugs match and others do not, the prompt may still include a small related-family set labeled `related`, not as used. Similarity scores are for ranking only, not probabilities or a cutoff (manual tests often land around 0.58?0.82).

---

## Design rules

- Same embedding model for documents and queries. Changing the model or dimensions requires a full re-index.
- Frontmatter `technologies` is the declared stack, not every technology mentioned in prose.
- Frontmatter `organization` and `environment` decide school/personal/company and production vs development. Prose such as live data does not override those fields.
- Frontmatter `links` keys are `github` (code), `live` (demo), and `portfolio` (article). Naming a project in an answer should include every present link. Environment is classification, not a URL.
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
src/backend/Application          Retrieval, questions, prompt templates, technology taxonomy
src/backend/Infrastructure       Documents, embeddings, pgvector, LLM, config
src/tools/KnowledgeIndexer       Ingest -> embed -> upsert
src/tools/CandidateConsoleAssistant  Manual retrieval + prompt inspection
src/frontend                     Vite + React + TypeScript + Tailwind chat UI
```

### Api (`src/backend/Api`)

| File | Role |
|---|---|
| `Program.cs` | DI, Swagger, controllers, static files, 404 fallback |
| `Controllers/QuestionsController.cs` | `POST /api/v1/Questions` (`includeDebug` adds scores and raw source). `POST /api/v1/Questions/progress` is the same ask with SSE status phases, then one JSON result. `POST /api/v1/Questions/intent` is keyword intent only (no LLM). |
| `Controllers/LlmController.cs` | `GET /api/v1/llm/test` |
| `Properties/launchSettings.json` | http `5179`, https `7277` |
| `wwwroot/404.html` | Fallback page |
| `api.http` | Manual HTTP samples |

`QuestionService` is registered in Api DI but lives under `Application/Questions` with namespace `Api.Services`.

### Application (`src/backend/Application`)

| File | Role |
|---|---|
| `Knowledge/KnowledgeRetrievalService.cs` | Query embed -> vector search -> score -> rank. Catalog/broad-experience questions merge overview chunks whose `organization` matches a query token so named companies are not dropped by the ANN limit. Named technologies also merge overview chunks whose `technologies` jsonb contains resolved slugs or family members. |
| `Knowledge/IKnowledgeRetrievalService.cs` | Retrieval contract |
| `Knowledge/KnowledgeRetrievalResult.cs` | Ranked items (source, heading, semantic type, content, scores) |
| `Knowledge/TechnologyCatalog.cs` | Alias/family lookup from `Prompts/taxonomy/technology-families.md` (not indexed, not sent as an LLM template) |
| `Questions/QuestionService.cs` | Orchestrates retrieve / prompt / LLM / source URLs. Optional `onProgress` reports search, then each LLM attempt with provider/model (`Asking Groq (gpt-oss-120b)`). Skips the LLM when named technologies have no exact `technologies` hit and returns `UnsupportedNamedTechnologyAnswer`. After a generated answer, `ListTableLinkRewriter` puts GitHub and live into list-table Project cells from each chunk's Links. Rule-based `QuestionIntentDetector` runs on the original question. `list`/`filter-list` with N uses a wider retrieve window; `list` without N also uses that wider window; `filter-list`/`count` without N keep the 25-hit window; catalog intents send one chunk per project. |
| `Questions/UnsupportedNamedTechnologyAnswer.cs` | Locale-aware grounded refusal when named technologies have no exact `technologies` match. Short human sentence. May name family technologies that retrieved projects did list as used, without claiming the missing technology. |
| `Questions/QuestionPhase.cs` | Pipeline status for SSE (`translating`, `searching`, `writing`) plus optional provider/model on LLM attempts |
| `Questions/QuestionIntentDetector.cs` | Keyword intent: `detail`, `list` (`what`/`which` projects), `count`, `filter-list`, plus optional `requestedCount`. Broad "have you used" / "what experience" questions that resolve named technologies (aliases such as C# -> csharp) are `filter-list`, not `detail`. |
| `Questions/ListTableLinkRewriter.cs` | After the LLM answer, rewrites list-table Project cells with `AnswerPromptFormatter.FormatLinkHeading` (same Links metadata as the prompt). GitHub, then live, on the title. When the table has an Article column, portfolio stays out of the title and an empty Article cell gets `[Read](url)` / `[Les](url)`. |
| `Questions/AnswerPromptFormatter.cs` | Shared LLM context/prompt fill used by API and CandidateConsoleAssistant. Context includes Organization, Period (`from` to `to`, plus whole years when the span is at least 12 months), Status, Environment, Technologies, Match (exact vs related), and http(s) Links as ready Markdown (GitHub/live/portfolio). When every selected chunk shares one organization and overlapping Periods merge to a single span of 2+ years, context also includes `OverlappingPeriod`, unless the question is a professional catalog: then context includes `OrganizationSpans` instead (one merged 2+ year line per company organization). On named-technology / "have you used" questions, the answer prompt requires listing every exact-match project in the table form and mentioning the longest exact-match Period of 2+ years once, attached to that technology. Count questions must list every matching project; they may use `OverlappingPeriod` in the opening sentence. |
| `Questions/PromptContextSelector.cs` | Default retrieve 25 / top-10 chunks for focused detail. Detail questions then append at most 15 overview chunks from that retrieved set, one per named technology the top 10 do not already contain, including when the question text merely contains a project slug. Broad "have you used / what experience" retrieves 80 (cap 100), then one chunk per `source` (up to 10) unless it is also `filter-list` from named technologies, in which case every exact-match source in that window is sent. `list` without N: same 80 retrieve, one chunk per `source`. `list`/`filter-list`+N: wider retrieve, one chunk per `source`, take N; `filter-list`/`count` without N: one chunk per `source` in the retrieve window. Production language on catalog paths keeps `environment: production`. Professional catalog questions on those paths keep company organizations and drop `Personal Project` and `School Project`. Named organization in the question keeps matching `organization` metadata. Questions that say completed/`ferdig` keep `status: completed` when that field is present. Named technologies keep exact `technologies` matches and do not pad. Related-family chunks are not sent to the LLM when every named slug is missing. Broad "what experience" + `and`/`og` is a tech union in the answer prompt; `both`/`både`/`have you used A and B` is an intersection. |
| `Questions/IQuestionService.cs` | Ask contract |
| `Questions/AskQuestionRequest.cs` / `AskQuestionResponse.cs` | API DTOs (`Locale` is `us` or `nb`; response includes `intent`) |
| `Questions/QuestionLocale.cs` | Locale normalize, query-translation flag, answer-language instruction |
| `Questions/QuestionSource.cs` / `QuestionRelevance.cs` | Evidence payload |
| `Questions/QuestionItem.cs` / `QuestionItemStatus.cs` / `QuestionDebugInfo.cs` | Extra question types |
| `Prompts/answer/answer-prompt.md` | Runtime answer template (copied to output). Includes each present GitHub, live, and portfolio link when a project is named. List tables use Project \| Summary unless more than two listed rows have a portfolio article, in which case they use Project \| Summary \| Article, omit portfolio from the Project cell, and put [Read](url) in Article (empty when that project has no portfolio URL). `ListTableLinkRewriter` applies that Project-cell heading after generation so GitHub and live are not dropped on rows that have them. |
| `Prompts/taxonomy/technology-families.md` | Runtime alias and family table for named technologies (copied to output, not indexed) |
| `Prompts/translate/query-translate-prompt-v1.md` | English retrieval query for `nb` |

### Infrastructure (`src/backend/Infrastructure`)

Documents: `MarkdownDocumentLoader` (recursive `*.md`, skip `_template_project.md`), `FrontmatterParser`, `ParsedMarkdown`, `MarkdownDocument`, `DocumentChunker` (ATX headings, nested `HeadingPath`, heading stripped from content), `SemanticTypeResolver`, `DocumentChunk`.

Embeddings: `EmbeddingService` (Ollama `/api/embed` from **env vars**, not the JSON `Embeddings` section), `VectorStore` (upsert on `id`; cosine `1 - (embedding <=> q)`; catalog questions also load `overview` chunks whose `organization` contains a query token; named technologies load `overview` chunks whose `technologies` jsonb contains resolved slugs), `SearchResult`.

Scoring: `MetadataEvidenceScorer` (term overlap on metadata + content -> combined score). `IReranker` / `RerankResult` exist; retrieval uses the scorer, not a cross-encoder. Stronger metadata filtering and hybrid/lexical search are still future work.

LLM: `ILLMClient`, `LlmClientFactory` (flatten providers x models), `FallbackLlmClient` (log try/fail/success, HTTP status, transient flag via `LlmProviderException`; reports each attempt to ask progress). Google class is `GoogleClient` in file `GeminiClient.cs`. Also `GroqClient`, `OpenRouterClient`. `CerebrasClient` exists but is not in the active `appsettings` provider list.

Config: `Configuration/AppConfiguration.cs`.

### Tools

- **KnowledgeIndexer:** load -> chunk -> embed -> `InsertAsync`. Prints document/chunk stats. Does not delete rows for removed or renamed files.
- **CandidateConsoleAssistant:** hardcoded eval questions; same intent, retrieval limit, context selection, and answer-prompt fill as `QuestionService` (prints all ranked hits, then the prompt the API would send). English locale only.

### Frontend (`src/frontend`)

Vite + React + TypeScript + Tailwind. Chat UI posts `{ question, locale }` (`us` | `nb`) to `POST /api/v1/Questions/progress` via a Vite proxy (`/api` ? `http://localhost:5179`) and shows SSE phase copy (search, then `Asking Groq (gpt-oss-120b)` per LLM attempt) until the final JSON `result`. While typing, the form previews intent via `POST /api/v1/Questions/intent` (same keyword detector, no LLM). The complete JSON `POST /api/v1/Questions` stays for Swagger. Chrome copy uses `i18next` / `react-i18next`. The header language menu uses `country-flag-icons` (US / NO) plus `sr-only` / `aria-label` names (`English (US)`, `Norsk bokmål`). Locale is stored in `localStorage`. Successful Q&A pairs are stored in `localStorage` (`mind-chat-history`) and listed in a left sidebar (`ChatHistory`) where the question text opens the saved answer and a re-ask icon button asks it again, plus a confirmed clear-history action. Types live in `src/frontend/client/types.ts`; fetch lives in `src/frontend/client/questions.ts`. Assistant answers are rendered with `react-markdown` plus `remark-gfm` (tables, strikethrough, thematic breaks; no raw HTML). Flattened one-line GFM tables are split into rows before parse. Markdown links open in a new tab and show an external-link icon. No CORS on the API. Swagger on `:5179` is unchanged.

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
- `osedalen-org-news-feed.md`
- `pim-integration.md`
- `react-hotel-booking-case.md`
- `sms-joke-archive.md`

Frontmatter (`_template_project.md`): `title`, `organization`, `role`, `environment`, `period`, `status`, `technologies`, `concepts`, `dependencies`, `links`.

Typical sections: Overview, Context, Task, Challenge, Result, Technical Decisions, Implementation, Lessons Learned, Future Improvements. Nested headings become `HeadingPath` (e.g. `Action > Technical Decisions > ?`). Semantic types come from `SemanticTypeResolver`. Very large sections are a known limitation (no secondary split yet).

Chunk id: `{filename}-{index:D3}-{headingSlug}`. Upsert updates the same id; heading edits can orphan old rows.

Postgres `document_chunks`: `id`, `source`, `heading_path`, `semantic_type`, `content`, `metadata` jsonb, `embedding vector(384)`.

---

## Config and LLM fallback

`AppConfiguration` loads `.env` then `appsettings.json` then environment variables.

Embeddings (required env): `OLLAMA_EMBED_URL`, `OLLAMA_EMBEDDING_MODEL`. Postgres: `ConnectionStrings__Postgres`.

`Llm` in `appsettings.json`: `MaxOutputTokens` (completion cap sent to every provider; Groq gpt-oss reasoning tokens count against it), `ThinkingLevel` / `ReasoningEffort`, ordered `Providers[]` with `Name`, `Models[]`, `TimeoutSeconds`. Current active order is Groq, then OpenRouter models, then Google. A configured `:free` OpenRouter slug can be invalid at runtime; treat availability as a runtime concern. Groq sends `include_reasoning: false`. OpenRouter sends `reasoning.enabled: false` so Nemotron and similar models do not dump chain-of-thought into `content` or spend the timeout thinking.

Logs look like `[LLM] Trying: Groq / ?`, `[LLM] Failed: ? Status=400 Transient=False`, `[LLM] Provider succeeded: ?`. First success stops the chain; all failures aggregate.

GitHub source URLs: `GitHub:Owner`, `Repository`, `Branch`, `ProjectsFolder`. `QuestionService.GetProjectUrl` currently inserts an extra `/` before `{id}.md`.

---

## Pitfalls and gaps

- Layers: Api host, Application orchestration, Infrastructure I/O. Console tools are not the REST host.
- API default retrieve-25 / prompt-10. Detail questions may append up to 15 extra overview chunks, one per named technology missing from those 10. `list` without N, `list`/`filter-list` with N, and `filter-list`/`count` without N diversify by `source`. Catalog and broad-experience questions that name an organization keep matching `organization`. Broad production questions filter on `environment`. Named technologies (aliases/families in `Prompts/taxonomy/technology-families.md`) filter on `technologies`. "Have you used" plus a named technology is `filter-list` and must list every exact-match project (answer-prompt table), not one example under `detail`. Zero exact matches skip the LLM (short grounded refusal; may name family technologies that were listed as used). They do not yet filter on period. Professional catalog questions drop school and personal organizations. `OrganizationSpans` names each company whose selected periods merge to one span of 2+ years; the answer states those spans in one sentence above the table and does not add the year counts. `ShorterOrganizations` names the other company organizations, and every numbered project at those organizations stays in the table.
- `locale: nb` adds an extra LLM call before retrieval. Chunks stay English. UI code `us` is not ISO 639 (`en`). Chat loading text tracks SSE phases from `/Questions/progress`.
- Progress SSE is status only. The API sets `X-Accel-Buffering: no`; the Vite `/api` proxy should not add extra buffering.
- Console eval still uses English questions and the English answer-language instruction.
- Indexer upserts only; wipe the table or Docker volume for a true rebuild after deletes/renames.
- `EmbeddingService` ignores `appsettings.json` `Embeddings` / `Ollama` sections.
- `EmbeddingService` reads `OLLAMA_*` from process env at type init. In console tools, call `AppConfiguration.Build()` before `new EmbeddingService()` so `.env` is loaded. The API already loads config first.
- No automated tests, no retrieval eval dataset, no frontmatter schema validation.
- Text files are UTF-8 without BOM with literal æ/ø/å (especially `src/frontend/i18n/locales/nb.ts`). Do not use PowerShell `-Encoding utf8NoBOM` or the console default; those produce U+FFFD. Prefer editor file tools or Python `encoding="utf-8"`.
- Frontend visual QA is manual; agents should not use the browser to confirm placement or styling unless asked or a runtime failure cannot be diagnosed from code or logs.
- Out of scope: auth, production deploy, candidate-to-job matching product, hybrid search.

When changing RAG behavior, update this file and, if the product story changed, `knowledge/projects/ai-candidate-assistant-rag.md` (then re-index).
