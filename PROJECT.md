# Project Definition

## Purpose

Continue the finished Retrieval-Augmented Generation (RAG) backend with a local chat interface, and use that interface to fine-tune the answer prompt and test answers against configured LLMs.

The backend already accepts a question, retrieves project evidence, builds a prompt from those chunks, and returns an answer. That system stays the host. This phase does not rebuild ingestion, the vector index, or the provider clients.

The chat is a Vite and React client. It posts a question and a locale to the existing API and renders the answer as Markdown. American English and Norwegian Bokmål are both supported. The usual test is to read the answer, compare it with the prompt that was sent, and change the prompt or the question routing when the answer is plausible but wrong.

The chat and the prompt edits in this phase were made in cooperation with Cursor, using Grok as the coding model in the editor. That was paired work in the IDE.

---

## Learning Objectives

This phase should demonstrate practical understanding of:

* Get experience with Cursor development workflow
* A frontend that is only a client of an existing API
* Chat interaction for asking, editing, and re-asking a question
* Rendering model output as Markdown, including tables and links
* Locale handling for English and Norwegian Bokmål, while retrieval stays on the English knowledge base
* Progress feedback from the real ask pipeline
* Prompt engineering judged by the answer a person reads
* Evidence limits: no invented technologies, projects, responsibilities, or production use
* Manual testing of many question shapes against live LLMs


---

## MVP Scope

The continuation should:

* Serve a local Vite, React, and TypeScript chat against the existing API.
* Send `{ question, locale }` with locale `us` or `nb`.
* Show pipeline progress while an answer is generated, including the provider and model on each attempt.
* Render the answer with tables and links, and allow the question to be edited or asked again.
* Keep recent questions in the browser.
* Preview keyword intent while typing, without an extra LLM call.
* In development, show the prompt that was sent, so a fluent answer can be checked against the evidence.
* Adjust the answer prompt and the chunk selection in front of it when GUI tests show a bad answer.
* Keep Swagger and the console tool on the same prompt path as the chat.

The phase is in good shape when a local question in the chat returns a grounded answer, and a failed answer can be re-asked after a prompt change without leaving the UI.

---

## Functional Requirements

### Existing backend

The API and embeddings already exist. The chat calls them.

The primary API workflow remains:

1. Receive a question.
2. For locale `nb`, translate the question to English before embedding.
3. Retrieve relevant knowledge.
4. Build the LLM prompt from the retrieved evidence.
5. Run the configured provider and model fallback chain.
6. Return the answer and the retrieved evidence.

### Chat

The chat must:

1. Submit a question to the progress endpoint and show status phases until the result arrives.
2. Render the answer as Markdown. Links open in a new tab.
3. Support editing the question and sending it again, and re-asking it unchanged.
4. Store the locale and successful question-answer pairs in the browser.
5. Offer English (US) and Norwegian Bokmål for the chrome and for the answer language.
6. Leave retrieval, embeddings, and LLM calls on the server.

### Answer quality

Prompt and routing changes in this phase are driven by answers read in the chat. A sane answer:

* Uses only retrieved evidence.
* Names every matching project when the question asks for a list, a count, or whether a named technology was used.
* Treats frontmatter organization and environment as the source of truth for school, personal, company, and production.
* Refuses a named technology that no selected project lists, instead of inventing use.
* Includes GitHub, live, and portfolio links only when that project's metadata contains them.

### Retrieval inspection

Swagger and `CandidateConsoleAssistant` stay available for chunks, scores, and the prompt text. The chat is the place where the finished answer is judged.

---

## Technical Requirements

The continuation uses:

* React
* TypeScript
* Vite
* Tailwind CSS
* The existing ASP.NET Core API

The browser talks to the API through the Vite dev proxy. The API does not enable CORS.

LLM API keys stay in environment variables. The chat does not hold provider keys and does not call a model itself.

Cursor with Grok may be used as the editor assistant for UI and prompt edits.

There is no new database. Locale and chat history stay in browser `localStorage`. PostgreSQL remains the backend retrieval index.

---

## Acceptance Criteria

This phase is successful when:

* The local chat can ask a question and show a formatted answer from the API.
* Loading text follows the real pipeline, including provider and model when a model is being called.
* English and Norwegian Bokmål both work against the English knowledge base.
* A question can be edited, re-asked, and found again in local history.
* Development builds can show the prompt behind an answer.
* List, count, professional, and named-technology questions have been tried in the chat, and prompt or routing changes from those tests are what the API sends.
* An unsupported technology produces a short grounded refusal rather than a fabricated project.
* The console tool and the API still fill the same answer prompt.
* The system still runs locally, without authentication or a public deployment.

---

## Out of Scope

The following stay outside this phase:

* Rebuilding Markdown ingestion, chunking, or the embedding index
* User authentication
* Public deployment and production monitoring
* A complete candidate-to-job matching product
* An automated retrieval or answer benchmark
* A mobile client

---

## Project Constraints

The constraint is to improve the answers people actually read, using the local chat as the test loop.

Prompt and routing changes belong on the server, so Swagger, the console, and the chat stay on one path. The browser remains a client.

Answer quality is judged manually. A generated edit is not accepted until the same question is asked again in the chat.

The backend phase is background. Its retrieval and fallback design should not be replaced in order to tune a paragraph.

---

## Definition of Done

This continuation is in working order when the local chat can take a candidate question in English or Norwegian Bokmål, show progress from the API, and render a grounded answer with the links the project metadata actually contains.

The answer prompt and the question routing in front of it have been adjusted from failed answers in that chat: full project lists where a list is required, professional and school work taken from frontmatter, and a refusal when the named technology is absent.

Retrieval can still be inspected in the console and in Swagger. Nothing in this phase requires a public deployment.
