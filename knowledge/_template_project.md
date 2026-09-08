---
title: Project Name

organization: Company Name | Personal Project | School Project | Open Source | Freelance | Interview Case

role: Fullstack Developer | Frontend Developer | Backend Developer

environment: development | staging | production | demo

period:
  from: YYYY-MM
  to: YYYY-MM | Present

status: active | completed | archived

technologies:
  - react-native
  - typescript
  - redux
  - sqlite
  - supabase
  - postgresql

concepts:
  - offline-first
  - state-management
  - synchronization
  - authentication

dependencies:
  - package/name

links:
  github: https://github.com/username/github-repo-name
  live: https://example.com

---

# Overview

<!-- DETAIL LEVEL: SHORT
Write 1?3 paragraphs.

Purpose:
Give a concise overview that allows the project to be understood quickly.

Include:
- What the project is.
- Who uses it.
- Why it exists.
- The main value it provides.
- The most important technologies or characteristics.

Do not explain implementation details here.
Do not repeat challenges or technical decisions.

This section should work as the project's executive summary.
-->

---

# Context

<!-- DETAIL LEVEL: SHORT?MEDIUM
Explain why the project existed and what situation surrounded it.

Include:
- Business or personal context.
- User needs.
- Existing system or legacy situation.
- Relevant limitations.
- Important constraints.
- Why the project needed to be built or changed.

Focus on circumstances, not your implementation.

Do not describe the solution in detail yet.
-->

---

# Task

<!-- DETAIL LEVEL: SHORT?MEDIUM
Describe YOUR responsibility.

Include:
- Your role.
- What you personally owned.
- Main goals.
- Expected outcome.
- Technical ownership.
- Collaboration with other developers or stakeholders when relevant.

Be precise about your personal contribution.

Do not claim responsibility for work performed by the wider team.
Do not turn this into a general project description.
-->

---

# Challenge

<!-- DETAIL LEVEL: DETAILED
This is one of the most important sections.

Document the technically or professionally interesting problems you actually solved.

Prefer several focused challenges rather than one large generic description.

Only include challenges that are useful for understanding your engineering ability.

Examples:
- Legacy migration.
- Performance bottleneck.
- Data consistency.
- Offline synchronization.
- Complex UI state.
- Security/privacy.
- Scalability.
- Integration problems.
- Backward compatibility.
- Difficult data transformation.
- Deployment or infrastructure problems.

Use one subsection per meaningful challenge.
-->

## Challenge: Title

### Problem

<!-- DETAIL LEVEL: MEDIUM?DETAILED

Explain:
- What was wrong or difficult?
- Why was it difficult?
- What constraints existed?
- What made the problem technically interesting?
- What would have happened if it was not solved?

Focus on the problem, not the solution.
-->

### Solution

<!-- DETAIL LEVEL: DETAILED

Explain:
- Your technical approach.
- Important implementation details.
- Why the approach worked.
- Important algorithms, patterns, workflows, or architecture.
- Relevant tools or libraries.
- Important reasoning behind the implementation.

This is where technical depth belongs.

Avoid repeating the complete architecture of the project.
Focus specifically on the solution to this challenge.
-->

### Result

<!-- DETAIL LEVEL: SHORT?MEDIUM

Explain the direct outcome of this solution.

Include:
- What improved.
- What problem was eliminated or reduced.
- Performance/reliability/usability improvements.
- Measurable results when available.

Do not repeat the entire project result.
-->

---

# Action

<!-- DETAIL LEVEL: MEDIUM
This section is the architectural map of the project.

Do NOT turn this into another list of challenges.

The purpose is to describe what you actually built and how the major parts fit together.

Keep each subsection relatively concise.

Architecture = WHAT EXISTS AND HOW IT IS CONNECTED.
Challenge = WHAT WAS DIFFICULT AND HOW IT WAS SOLVED.
Technical Decisions = WHY A PARTICULAR APPROACH WAS CHOSEN.
-->

## Architecture

### Frontend

<!-- DETAIL LEVEL: MEDIUM

Describe:
- Frameworks and major libraries.
- Component/module structure.
- State management.
- Important UI architecture.
- Main interaction patterns.
- Communication with backend.

Do not repeat individual challenge solutions unless necessary for understanding the architecture.
-->

### Backend

<!-- DETAIL LEVEL: MEDIUM

Describe:
- Runtime/framework/language.
- API architecture.
- Business logic.
- Services.
- Authentication/authorization when relevant.
- External integrations.
- Important backend responsibilities.

Keep this architectural rather than challenge-focused.
-->

### Database

<!-- DETAIL LEVEL: MEDIUM

Describe:
- Database technology.
- Main entities/data model.
- Important relationships.
- Persistence strategy.
- Important storage decisions.

Explain enough to understand how data is structured and persisted.
Do not document every table or field unless it is technically important.
-->

### File Storage

<!-- DETAIL LEVEL: SHORT?MEDIUM
Include this section only when the project uses dedicated file/object storage.

Describe:
- Where files are stored.
- How metadata relates to files.
- How the application accesses them.
- Important lifecycle considerations.
-->

### Infrastructure

<!-- DETAIL LEVEL: SHORT?MEDIUM
Include when infrastructure is relevant.

Describe:
- Hosting.
- Containers.
- Cloud services.
- Deployment.
- CI/CD.
- Environment/configuration management.
- Important operational components.

Do not duplicate detailed infrastructure decisions from Technical Decisions.
-->

---

## Technical Decisions

<!-- DETAIL LEVEL: DETAILED
Document decisions where the WHY matters.

Only include meaningful decisions.

Good candidates:
- Architecture choices.
- Technology choices.
- Data storage choices.
- API strategy.
- State management.
- Migration strategy.
- Security approach.
- Performance strategy.
- Infrastructure choices.

Do not create a decision section for trivial implementation details.
-->

### Decision: Title

#### Context

<!-- DETAIL LEVEL: SHORT?MEDIUM

Why did this decision need to be made?

Describe the specific requirement, constraint, or problem that led to the decision.
-->

#### Chosen Solution

<!-- DETAIL LEVEL: MEDIUM?DETAILED

Describe what you chose and how it was implemented.

Focus on the decision itself.
Detailed implementation belongs primarily in Challenge ? Solution or Implementation.
-->

#### Alternatives Considered

<!-- DETAIL LEVEL: SHORT?MEDIUM

List realistic alternatives you actually considered.

Do not invent plausible alternatives for completeness.

Explain briefly why they were not chosen.
-->

#### Trade-offs

<!-- DETAIL LEVEL: MEDIUM

Describe:
- Advantages.
- Disadvantages.
- New complexity introduced.
- Limitations.
- Situations where another approach might have been better.

Be honest about weaknesses.
-->

---

## Implementation

<!-- DETAIL LEVEL: MEDIUM
Describe the concrete functionality you built.

This section should answer:

"What did I actually implement?"

Use concise grouped descriptions or bullet lists.

Include only implementation facts that are useful and not already adequately covered elsewhere.
-->

### Features

<!-- DETAIL LEVEL: SHORT?MEDIUM

List important user-facing or system-level functionality.
-->

- Feature
- Feature
- Feature

### APIs

<!-- DETAIL LEVEL: SHORT?MEDIUM
Include when the project contains APIs.

Describe:
- Important endpoints or API capabilities.
- Main data exchanged.
- Important integrations.

Do not document every endpoint.
-->

### Data and Persistence

<!-- DETAIL LEVEL: SHORT?MEDIUM
Include important database/persistence implementation details not already covered by Architecture.
-->

### Automation

<!-- DETAIL LEVEL: SHORT?MEDIUM
Include:
- Scheduled jobs.
- Background processing.
- Automated cleanup.
- CI/CD automation.
- Other significant automation.
-->

### Testing

<!-- DETAIL LEVEL: SHORT?MEDIUM
Describe testing that actually existed.

Include:
- Unit tests.
- Integration tests.
- End-to-end tests.
- Manual testing.
- Browser/device testing.
- Pilot/customer testing.

Do not claim tests that did not exist.
-->

---

# Result

<!-- DETAIL LEVEL: SHORT?MEDIUM
Describe the overall outcome of the project.

This is the final answer to:

"What did the project achieve?"

Include:
- Measurable improvements.
- User impact.
- Business value.
- Technical improvements.
- Performance.
- Reliability.
- Adoption.
- Scale.

Use numbers whenever they are factual and available.

Do not repeat the technical implementation in detail.
Focus on outcomes.
-->

---

# Lessons Learned

<!-- DETAIL LEVEL: MEDIUM
Describe what YOU learned from the project.

Focus on:
- Technical lessons.
- Architectural lessons.
- Engineering practices.
- Product/process lessons.
- Things that changed how you work.
- Important mistakes or discoveries.
- What you would approach differently today.

Prefer concrete lessons over generic statements.
-->

## Lesson: Title

<!-- DETAIL LEVEL: SHORT?MEDIUM

Explain:
- What you learned.
- What caused the lesson.
- How it changed your approach.
-->

---

# Future Improvements

<!-- DETAIL LEVEL: SHORT?MEDIUM
Describe realistic improvements you would make if the project continued.

Good examples:
- Architecture modernization.
- Better testing.
- Better observability.
- Scalability improvements.
- Security improvements.
- Performance improvements.
- UX improvements.
- Automation.
- Migration away from legacy technology.

These should be technically credible and based on the actual project.

Avoid generic wishlist features.
-->

- Improvement
- Improvement
- Improvement

---