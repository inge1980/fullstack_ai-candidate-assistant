---
title: GUI for the AI-Assisted Candidate Assistant

organization: School Project

role: Fullstack Developer

environment: development

period:
  from: 2026-09
  to: Present

status: active

technologies:
  - react
  - typescript
  - vite
  - tailwindcss
  - css
  - i18next
  - react-markdown
  - cursor

concepts:
  - chat-interface
  - prompt-engineering
  - manual-testing
  - localization
  - markdown-rendering
  - ai-assisted-development
  - evidence-based-answers

dependencies:
  - react
  - react-dom
  - vite
  - tailwindcss
  - typescript
  - i18next
  - react-i18next
  - react-markdown
  - remark-gfm
  - country-flag-icons

links:
  github: https://github.com/inge1980/fullstack_ai-candidate-assistant

---

# Overview

A local chat interface for an existing candidate-assistant API, built so questions can be asked, re-asked, and judged as a person would read them. The backend Retrieval-Augmented Generation (RAG) system was already finished: Markdown knowledge, embeddings, PostgreSQL with pgvector, and an ASP.NET Core API that builds an evidence prompt and calls an LLM. That system is documented in `ai-candidate-assistant-rag.md`. This project is the continuation. It does not rebuild ingestion, the vector index, or the provider clients.

The chat UI is a Vite, React, and TypeScript client. It posts a question and a locale to the API and renders the answer as Markdown, including tables and links. American English and Norwegian Bokmål are both supported in the chrome and in the answer language. The interface was implemented in cooperation with Cursor, using Grok as the coding model in the editor. That was paired implementation in the IDE: I directed the behavior, reviewed the diff, and decided when an answer was sane.

Once the chat existed, the work shifted to the answer prompt and the question routing in front of it. Test questions were run in the GUI against the configured LLMs, and the prompt was tightened when the answer was plausible but wrong.

---

# Context

The backend Retrieval-Augmented Generation (RAG) system documented in `ai-candidate-assistant-rag.md` was imported into this repo. In collaboration with Cursor, I added a Vite chat that calls the existing API. After that chat existed, test questions, prompt fixes, intent rules, and the way selected chunks are described to the model, was changed when an answer failed.

---

# Task

I owned the product judgment and the test loop.

- Decide which candidate questions the assistant must answer sanely.
- Keep the browser a client of the API, with no retrieval or LLM calls of its own.
- Use the chat to compare the answer with the prompt that was actually sent.
- Reject answers that invent technologies, drop projects, confuse school work with professional work, or hide links.
- Direct the prompt and routing changes, and accept them only when a re-ask in the GUI improved the answer.

Cursor, with Grok, wrote most of the React UI and applied the prompt edits in the same working tree. I stayed in the loop for every change: what the screen should do, which question failed, and whether the new answer was acceptable.

I did not re-implement document ingestion, embeddings, or pgvector. Those remain the background API.

---

# Challenge

## Challenge: A chat that shows the real answer, not another debug dump

### Problem

The API already returned an answer string plus evidence. A thin page that printed that string as raw Markdown still failed as a reading surface. Models emitted one-line tables, non-breaking spaces that would not wrap, bare URLs, and sometimes chain-of-thought in the content. A generic spinner also hid whether the wait was translation, search, or a fallback to another model.

Without edit, re-ask, and history, every prompt experiment meant retyping the question and losing the previous answer.

### Solution

The UI is a single-question chat. The user message can be edited and sent again, or re-asked as-is. Successful pairs are kept in `localStorage` and listed in a sidebar.

The client calls `POST /api/v1/Questions/progress` and reads server-sent events. The status line follows the pipeline: translating the question when the locale is Norwegian, searching the index, then writing, including the provider and model on each attempt (`Asking Groq (gpt-oss-120b)` and the same pattern when another model is tried). While the draft is being typed, `POST /api/v1/Questions/intent` previews the keyword intent. That preview does not call an LLM.

Answers are rendered with `react-markdown` and `remark-gfm`. Before parse, non-breaking spaces are turned back into normal spaces, one-line tables are split into rows, and bare `http` URLs become links. Links open in a new tab. In development the request asks for debug data, and the answer bubble can expand the prompt the model received.

Groq is configured with reasoning excluded from the response, and OpenRouter is configured with reasoning disabled, so the chat shows the answer rather than a thinking trace. Those provider settings belong to the existing LLM clients. They were changed because the GUI made the leaked reasoning obvious.

### Result

A question can be asked, read as a formatted answer, compared with the prompt, and asked again without leaving the page. Loading text matches the step the API is actually on.

## Challenge: Prompt rules that only showed up as bad answers

### Problem

Retrieval could rank the right chunks and the model could still answer badly. Broad "have you used" questions collapsed to one example. Count questions returned only a number. Professional questions included school or personal projects. A named technology that no project lists was still described as used. A pasted job listing was treated like a short question: technologies named later in the ad fell out of the first chunks, and words such as "and" or "both" were read as if one project had to use every technology in the listing. Openings used phrases such as "Based on the context" or padded a plain fact with words like "extensive". List answers dropped GitHub or live links, or put every link in the wrong place.

The console could show the prompt. It could not show whether the finished paragraph was something I would stand behind.

### Solution

The answer prompt and the selector that fills it were adjusted from those failures. The retrieval design itself stays in `ai-candidate-assistant-rag.md`. What this phase changed is the contract the model is held to, checked by re-asking in the GUI:

- List, filter, and "have you used" answers name every matching project, in a Markdown table the chat renders, with a short summary on each row.
- Count answers state the number and still list the projects, as bullets.
- Professional catalog questions drop personal and school projects. When a company's selected periods merge into one span of at least two years, that span is one sentence above the table. Shorter company work stays in the table and out of that sentence.
- Organization and environment come from frontmatter. Prose that sounds like production does not override those fields.
- If no selected project lists the named technology, the API does not call the LLM. It returns a short refusal. Related family technologies that were listed may be named as used, without claiming the missing one.
- "And" on a broad experience question is a union. "Both", "både", and "have you used A and B" are an intersection, except on a pasted job listing, which stays a union.
- A pasted job listing of at least 1500 characters is routed as large input. Named technologies in that listing are a union, including when the ad says and, both, or både. Every project that used any of those technologies is sent, rather than only the first few chunks. A technology the listing names and no project lists is left as not in the record. When three or more projects match, the answer is a table with one row per project. This tunes the answer to a pasted listing. It is not a candidate-to-job matching product.
- Project links in an answer come from that project's Links metadata. GitHub and live sit on the title. When enough rows have a portfolio article, that URL goes in an Article column as Read or Les. A rewriter fills those cells from the chunks after generation, so a model that drops a link does not win.
- The voice is first person, concise, and free of prompt wording. The answer opens from the developer's experience, not from a description of the context.

The same intent, retrieval window, and prompt fill are used by the API and by `CandidateConsoleAssistant`, so a GUI failure can still be inspected as a prompt.

### Result

Re-asking the failing questions produced lists, refusals, and professional summaries that match the project files more closely than the first chat answers did. The check is still manual. There is no automated answer benchmark.

## Challenge: Norwegian in the UI and in the answer, without breaking the English index

### Problem

The knowledge base and the embeddings are English. The chat also needs Norwegian Bokmål: button labels, history, and the generated answer. A Norwegian question that was embedded as-is retrieved poorly. Separately, saving Norwegian copy on Windows through the wrong encoding replaced æ, ø, and å, including words the prompt itself must use, such as både and portefølje.

### Solution

The header language menu stores `us` or `nb` in `localStorage`. Chrome strings live in locale files. The API still translates an `nb` question to English before embedding, then instructs the model to answer in fluent Norwegian Bokmål. Project titles stay as written. Link labels switch (Les, Artikkel, artikkel i portefølje).

Text files stay UTF-8 without a BOM, with real æ, ø, and å. That rule was added because Cursor sessions on this machine had already damaged those letters when a file was rewritten carelessly.

Changing language confirms before it clears the current question, because the answer language and the stored draft would otherwise disagree.

### Result

The same chat can be used in English or Norwegian against one English index. Norwegian prompt labels and UI strings keep their letters.

---

# Action

## Architecture

### Frontend

The app lives in `src/frontend`. Vite serves it at `http://localhost:5173` and proxies `/api` to the API at `http://localhost:5179`. The API does not enable CORS. The browser never embeds a query, never searches pgvector, and never calls Groq, OpenRouter, or Google.

Main pieces:

- `App` holds the draft, the current exchange, loading progress, and history.
- `QuestionForm` submits the question and shows the intent preview.
- `MessageList` renders the user bubble (edit, resend, re-ask) and the assistant bubble.
- `AssistantMarkdown` prepares and renders the answer.
- `ChatHistory` lists saved questions, opens a saved answer, re-asks, and clears history after confirmation.
- `LanguageMenu` switches `us` and `nb` with US and Norwegian flags, plus accessible names.
- `client/questions.ts` reads the progress stream and the intent endpoint.
- `i18n` holds the English and Norwegian chrome copy.

Layout follows the browser color scheme. There is no separate theme toggle. Visual placement is checked by eye; agents were told not to click through the layout unless a runtime failure could not be diagnosed from code.

### Backend

The host is unchanged in role. `POST /api/v1/Questions` remains the Swagger call. The chat uses the progress route, which emits phase events and then one JSON result of the same shape. `POST /api/v1/Questions/intent` is the keyword detector only.

Prompt work in this phase sits in the answer template, the intent detector, the context selector, and the list-table link rewriter. Indexing and embedding code were not the task.

### Database

This phase does not add a database.

The chat keeps locale and past questions in the browser. Project knowledge, chunk metadata, and embeddings stay in the existing PostgreSQL index. The browser never opens that database. How the index is built is documented in `ai-candidate-assistant-rag.md`.

### Infrastructure

The chat runs locally. Vite serves the UI and proxies `/api` to the ASP.NET Core process on the same machine.

Docker Compose and PostgreSQL belong to the backend project. This phase did not add hosting, a frontend pipeline, or a public URL. There is no separate file or object storage.

## Technical Decisions

### Decision: Build the GUI with Cursor and Grok, in the editor

#### Context

The backend submission was done. The next need was a chat and a fast way to revise the prompt after each bad answer. Doing that by hand in Swagger was too slow for the number of question shapes that had to be tried.

#### Chosen Solution

I worked in Cursor with Grok as the assistant model. I described the screen, the failed answer, and the rule the prompt should follow. Cursor edited the repo. I read the result and re-tested in the GUI.

There is no Grok bot in this project: no separate autonomous agent, no bot user, and no unattended rewrite of the knowledge base. Commits from this period are the UI and the prompt fixes, not a bot pipeline.

#### Alternatives Considered

Staying on Swagger and the console would have kept the Module 4 workflow, and would have left table layout, Norwegian chrome, and answer voice untested. Handing the repo to an unattended agent was not the way this was built.

#### Trade-offs

Advantages:

- The chat and the prompt edits landed quickly enough to test many question shapes in the same month.
- The git history shows that the work was done with Cursor, using Grok as the editor model.
- I could reject an edit by re-asking the same question.

Disadvantages:

- A generated edit is not evidence that the answer is supported. Each one still needed a manual re-ask.
- Windows encoding had to be checked on every file that contains æ, ø, or å.
- Leaving the editor to run unattended was a worse fit, and was not how this was built.

### Decision: The GUI is a client, and the prompt is the product being tuned

#### Context

`PROJECT.md` had finished the backend. `README.md` already described indexing, retrieval, and provider fallback. Re-documenting or re-building that stack would not fix a bad paragraph in the chat.

#### Chosen Solution

New behavior in the browser is presentation and session state: Markdown, locale, history, progress text, intent preview, and the debug prompt. Changes that affect what the model is allowed to say stay on the server, in the English answer prompt and the code that selects chunks. The console tool fills that same prompt, so GUI tests and console inspection stay on one path.

#### Alternatives Considered

A second prompt assembled in the browser would have drifted from Swagger and the console. Copying retrieval into the client would have broken the layering the backend project was built around.

#### Trade-offs

Advantages:

- Swagger, the console, and the chat stay on one prompt.
- The browser stays free of retrieval and LLM calls.

Disadvantages:

- A bad paragraph in the chat is fixed on the server, which is slower than rewriting the string in React.
- Prompt rules and chunk selection grew as new question shapes failed.

### Decision: Show the prompt in the chat while testing

#### Context

A fluent answer can still be unsupported. The Module 4 console existed to separate retrieval from the final paragraph. The GUI needed the same split without a second tool for every click.

#### Chosen Solution

Development requests send `includeDebug=true`. The assistant bubble keeps the answer visible and tucks the prompt under a disclosure. Production builds of the client omit that flag. History stores the prompt with the pair so a saved answer can be opened with the evidence prompt that produced it.

#### Alternatives Considered

Leaving the prompt only in `CandidateConsoleAssistant` would have kept the Module 4 tool, and would have forced a second app open for every chat failure. Always printing the full prompt above the answer would have buried the paragraph that had to be judged. Hiding it entirely would have made a fluent but unsupported answer harder to catch.

#### Trade-offs

Advantages:

- The answer and the prompt that produced it sit on the same bubble during testing.
- A saved history item can be reopened with that prompt.

Disadvantages:

- The disclosure is a testing aid. It does not replace score inspection in the console or in Swagger.
- Debug requests are tied to the development build. A production build of the client does not ask for the prompt.

## Implementation

### Features

- One question on screen, with edit, resend, and re-ask.
- Sidebar history in `localStorage` (`mind-chat-history`), capped, with a confirmed clear.
- Locale menu for English (US) and Norwegian Bokmål, stored in `localStorage`.
- Progress text from the live pipeline, including provider and model.
- Intent preview while typing.
- GFM tables, links with an external-link icon, and repair for flattened tables and non-breaking spaces.
- Dark colors when the browser asks for them.
- Autofocus on the question field.
- Debug prompt on the answer in development.

### APIs

The client uses only:

- `POST /api/v1/Questions/progress` for the answer stream.
- `POST /api/v1/Questions/intent` for the keyword preview.

The JSON `POST /api/v1/Questions` endpoint is unchanged for Swagger.

### Data and Persistence

The chat does not persist questions on the server.

Two `localStorage` keys hold client state:

- `mind-locale` stores `us` or `nb`.
- `mind-chat-history` stores up to 50 question, answer, and prompt triples.

Clearing history removes that key after a confirmation dialog. Changing locale can clear the current draft and the on-screen exchange so the answer language does not disagree with the chrome.

### Automation

There is no scheduled job, background worker, or frontend build pipeline in this phase.

Indexing the Markdown knowledge base remains a manual run of the existing indexer. Answer checks are manual re-asks in the GUI. No test runner covers the chat or the prompt.

### Testing

Testing in this phase is manual, in the GUI, against the configured LLMs. The console question list covers the earlier retrieval set: ASP.NET Core, Azure, CI/CD, PostgreSQL, pgvector, ERP, Docker, React and TypeScript, GDPR, and questions whose technology is not in any project. The chat added the shapes that the prompt was getting wrong:

- Which projects, how many, and top N.
- Have you used a technology, including C# as an alias for the `csharp` slug.
- Production versus school or personal, taken from frontmatter.
- Professional experience across companies, including spans of at least two years.
- Broad experience with "and" versus "both" / "både", and the same words inside a pasted job listing, where they stay a union.
- Pasted job listings of at least 1500 characters, checked so every project that used any named technology is kept.
- Norwegian questions and Norwegian chrome.
- Answers that should refuse, including soft or unsupported claims.

A failure was kept if the answer invented a project, a URL, a technology, or a workplace, or if it omitted a matching project the prompt had been given. The fix was then re-asked in the same GUI.

There is no automated UI suite and no retrieval benchmark. Layout was not signed off by browser automation.

---

# Result

The local chat is the way this assistant is tried. A question in English or Norwegian returns a formatted answer grounded in the retrieved projects, with GitHub, live, and portfolio links when those URLs exist on the project.

The Module 4 backend remains the knowledge and retrieval system. What this continuation added is a client I can re-ask from, and a prompt that has been corrected against those re-asks: full project lists, professional tenure only where the periods support it, a short refusal when the named technology is absent, and job-listing answers that keep every project that used any named technology in the ad.

The work is still active. Prompt wording and question routing are still adjusted when a new question reads wrong. Nothing here is deployed, and there is still no authentication.

---

# Lessons Learned

## Lesson: The answer you read is a different test from the prompt you print

The console and Swagger were the right tools for chunks and scores. They did not catch a one-line table, a dropped live demo link, or an opening that talks about "context". Those failures showed up only when the answer was rendered and read in the chat. After the GUI existed, prompt changes were accepted by re-asking, not by inspecting the template alone.

## Lesson: Cursor sped up the edits; it did not decide that an answer was sane

Grok in Cursor implemented the screen and applied prompt patches quickly. The useful constraint was the opposite of leaving it to run: I named the bad answer, kept the knowledge files as the source of truth, and rejected patches that sounded helpful but were not supported. Calling that cooperation a Grok bot would misstate how the repo was written.

## Lesson: Norwegian letters are part of the prompt, not a font problem

Words such as både and portefølje are instructions the model sees. Replacing them with `?` or with a mis-decoded byte sequence changes the Norwegian answer. UTF-8 without a BOM, and real æ, ø, and å, had to be enforced on UI strings and on the answer prompt after Cursor rewrites on Windows.

## Lesson: Reasoning output is not an answer

With the chat on screen, chain-of-thought from a provider was simply a wrong answer. Turning reasoning off for Groq and OpenRouter mattered more than giving the model more output tokens.

---

# Future Improvements

- Keep tuning the answer prompt from questions that still read wrong in the GUI.
- Add an automated check that a known question still names the expected projects, so a prompt edit cannot silently drop one.
- Public deployment, authentication, and a full candidate-to-job product stay out of scope, as they were for the backend submission.
- The chat is a local development client. It is not yet a published interface.

---
