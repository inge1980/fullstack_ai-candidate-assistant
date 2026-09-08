# Project Definition

## Purpose

Create a backend-focused AI knowledge assistant designed to represent a developer's projects, technical experience, decisions, and professional background.

The system should use Retrieval-Augmented Generation (RAG) to retrieve relevant evidence from a Markdown-based knowledge base before generating an answer with an LLM.

The project should combine backend development with practical use of semantic search, vector databases, embeddings, LLM integration, and resilient external API integration within an individual four-week Module 4 backend project.

The Markdown knowledge base should be the Single Source of Truth (SSoT). PostgreSQL with pgvector should be used as a generated retrieval index that can be discarded and rebuilt from the Markdown source.

The project should also include a console-based evaluation tool for manually inspecting retrieval quality, relevance scoring, retrieved evidence, and the generated LLM prompt independently of the REST API.

---

## Learning Objectives

The project should demonstrate practical understanding of:

* C# and ASP.NET Core backend development
* Docker and Docker Compose
* Application architecture
* REST API design
* PostgreSQL and pgvector
* Semantic search and Retrieval-Augmented Generation (RAG)
* Local embedding generation
* Document ingestion and processing
* Evidence-based LLM responses
* Integration with external LLM providers
* Provider and model fallback
* Error handling and failure resilience
* Observability for manual testing and evaluation
* Retrieval evaluation and evidence inspection

The project should prioritize a working end-to-end backend over implementing a large number of unrelated features.

---

## MVP Scope

The MVP should be a backend system that can:

* Expose the functionality through an ASP.NET Core REST API.
* Maintain developer knowledge as Markdown documents.
* Extract project metadata from YAML frontmatter.
* Process knowledge documents recursively.
* Split documents into semantic sections.
* Generate embeddings for knowledge sections.
* Store sections, metadata, and embeddings in PostgreSQL using pgvector.
* Convert user questions into embeddings.
* Retrieve relevant project evidence using semantic similarity.
* Apply metadata retrieval and ranking.
* Provide retrieved evidence to an LLM.
* Generate answers based on the retrieved evidence.
* Avoid unsupported claims when evidence is insufficient.
* Support multiple LLM providers and models.
* Fall back to alternative models or providers when requests fail.
* Return generated answers together with retrieved evidence and relevance information.
* Generate source references to the corresponding project documents.
* Provide a console-based retrieval evaluation workflow for manually inspecting retrieval results and generated answer prompts.

The MVP should be considered complete when the complete flow from Markdown knowledge, through indexing and retrieval, to evidence-based answer generation works locally through the backend API, with retrieval independently inspectable through the console evaluation tool.

---

## Functional Requirements

### Knowledge Base

The system must use Markdown files as the authoritative knowledge source.

The PostgreSQL index must be rebuildable from the Markdown source.

### Indexing

The indexer must:

1. Process Markdown documents recursively.
2. Exclude template documents.
3. Parse YAML frontmatter.
4. Extract project metadata.
5. Split documents into semantic sections.
6. Propagate project metadata to generated chunks.
7. Generate embeddings.
8. Store chunks, metadata, and embeddings in PostgreSQL.

### Retrieval

The system must:

1. Convert a user question into an embedding.
2. Perform semantic similarity search using pgvector.
3. Retrieve relevant project sections.
4. Apply similarity / evidence scoring.
5. Expose retrieval information for manual inspection.
6. Support inspection of the top-ranked retrieved results through the console evaluation tool.

### Retrieval Evaluation

The project should provide a console-based evaluation tool that can:

1. Accept candidate-oriented questions for evaluation.
2. Generate an embedding for the question.
3. Perform retrieval against the indexed knowledge base.
4. Calculate and display combined retrieval information, including vector, and evidence scores.
5. Display the top-ranked retrieved results.
6. Show source project, section, semantic type and retrieved content.
7. Generate and display the LLM answer prompt based on the retrieved evidence.

The evaluation tool should make it possible to inspect retrieval quality and evidence sufficiency before relying on the generated answer.

### Answer Generation

The LLM must receive retrieved project evidence rather than the complete knowledge base.

Generated answers should be based on the retrieved evidence and should avoid unsupported claims.

When the available evidence is insufficient, the system should state this instead of hallucinating an answer.

### LLM Integration

The backend should use a provider-independent LLM abstraction.

A provider should be able to contain multiple configured models.

The fallback system should evaluate configured provider/model combinations sequentially and continue when a request fails.

Provider/model failures should be logged with relevant failure information.

### REST API

The backend should expose a REST API for question answering.

The primary workflow should be:

1. Receive a question.
2. Generate a query embedding.
3. Retrieve relevant knowledge.
4. Build the LLM prompt using the retrieved evidence.
5. Execute the configured provider/model fallback chain.
6. Return the generated answer.
7. Return retrieved evidence and relevance information.

Swagger should be available for manual API testing.

### Source References

Retrieved project evidence should be associated with the corresponding source.

---

## Technical Requirements

The implementation should use:

* C#
* .NET / ASP.NET Core Web API
* PostgreSQL
* pgvector
* Embeddings model
* Docker
* Docker Compose

The embedding model should be used for both document and query embeddings.

The application should use configuration-driven provider/model selection.

LLM API keys must be supplied through environment variables rather than stored in source-controlled configuration.

The PostgreSQL database should be treated as generated infrastructure rather than SSoT.

A console-based evaluation tool should be available for retrieval and evidence inspection independently of the REST API.

---

## Acceptance Criteria

The project should be considered successful when:

* Markdown knowledge can be indexed successfully.
* The retrieval index can be rebuilt from the Markdown knowledge base.
* User questions can retrieve relevant project evidence.
* Retrieval results can be inspected manually.
* Retrieval and scoring are available.
* The console evaluation tool can display retrieved results, scores and evidence.
* The console evaluation tool can generate and display the LLM answer prompt based on retrieved evidence.
* Retrieved evidence is supplied to the LLM.
* Generated answers are constrained by available evidence.
* Multiple LLM providers can be configured.
* Provider/model fallback works when a configured candidate fails.
* Swagger can be used to test the API from question to JSON constructed answer with evidence details.
* The complete backend workflow can run locally.
* Retrieval quality can be evaluated independently of the final generated answer.

---

## Out of Scope

The following should be outside the scope of the Module 4 project:

* React or mobile frontend
* User authentication
* Public deployment
* Cloud infrastructure
* Production monitoring
* Complete candidate-to-job matching product
* Automated retrieval evaluation benchmark

These may be natural future extensions, but should not be required for the current project.

---

## Project Constraints

The project scope should be defined from the Module 4 learning objectives and the chosen technical direction.

The main constraint should be to develop a realistic backend MVP within the available project period while demonstrating meaningful understanding of backend architecture, databases, AI integration, retrieval, external service resilience, and practical evaluation of retrieval quality.

The project should intentionally focus on backend functionality rather than building a complete end-user product.

The retrieval evaluation should initially remain manual through the console evaluation tool rather than requiring a formal automated benchmark.

---

## Definition of Done

The project should be considered complete for the Module 4 scope when the end-to-end backend workflow is operational:

Markdown knowledge can be indexed, relevant evidence can be retrieved using semantic search, the evidence can be supplied to an LLM, and the generated answer can be returned through the REST API together with the retrieved sources and relevance information.

The retrieval pipeline should also be independently inspectable through the console evaluation tool, including retrieved projects and sections, relevance information, evidence content, and the generated answer prompt.

The implementation should have been manually tested using questions covering different technologies, projects, responsibilities, and distinctions such as production versus non-production experience.