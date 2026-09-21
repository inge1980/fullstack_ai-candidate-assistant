# Technology aliases and families

Not indexed. Runtime lookup for question routing, copied with other Prompt files.
The LLM should not receive this file as a template; the formatter injects a short Match instruction.

Canonical slugs must match `technologies` frontmatter (lowercase, hyphenated).
Phrases in a question map only through this list. Do not add bare tokens such as
`sql` or `server`.

Related fallback uses **families**. A missing named slug may pull other slugs in
the same family into retrieval. Prompt context still requires an exact
`technologies` match before the LLM is called. Related-only hits become a
short grounded refusal. That refusal may name family technologies that projects
did list as used, without claiming the named missing slug. Do not put `react`
and `react-native` in one family.

## Aliases

Each bullet is `canonical-slug: phrase, phrase`.

- sql-server: sql server, mssql, microsoft sql server, ms sql, ms sql server, sql-server
- mysql: mysql, my sql, my-sql
- postgresql: postgresql, postgres, postgres sql, postgre sql, pg
- sqlite: sqlite, sqlite3
- oracle: oracle, oracle db, oracle database
- mariadb: mariadb, maria db
- php: php
- csharp: csharp, c#, c sharp, c-sharp
- dotnet: dotnet, .net, dot net
- aspnet-core: aspnet-core, asp.net core, aspnet core, asp net core, asp.net
- entity-framework-core: entity-framework-core, entity framework core, entity framework, ef core, efcore
- rest-api: rest-api, rest api, rest apis, restful, rest
- javascript: javascript, java script, js
- typescript: typescript, type script, ts
- react: react
- react-native: react-native, react native
- next.js: next.js, nextjs, next js
- redux: redux
- html: html
- css: css
- bootstrap: bootstrap
- jquery: jquery, j query
- swagger: swagger, openapi, open api
- docker: docker
- docker-compose: docker-compose, docker compose
- kubernetes: kubernetes, k8s, kube
- terraform: terraform
- azure: azure
- github-actions: github-actions, github actions
- subversion: subversion, svn
- iis: iis
- n8n: n8n
- pgvector: pgvector, pg vector
- material-ui: material-ui, material ui, mui
- json: json

## Families

Each bullet is `family-id: slug, slug`. Skip related fallback when a family
would be too broad to stay honest.

- relational-sql: sql-server, mysql, mariadb, postgresql, sqlite, oracle
- containers: docker, docker-compose, kubernetes, container-registry
