CREATE EXTENSION IF NOT EXISTS vector;

CREATE TABLE IF NOT EXISTS document_chunks
(
    id text PRIMARY KEY,

    source text NOT NULL,

    heading_path text NOT NULL,

    semantic_type text NOT NULL,

    content text NOT NULL,

    metadata jsonb,

    embedding vector(384)
);

CREATE INDEX document_chunks_embedding_idx
ON document_chunks
USING hnsw (embedding vector_cosine_ops);