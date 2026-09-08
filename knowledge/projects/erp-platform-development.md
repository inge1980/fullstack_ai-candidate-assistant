---
title: ERP Platform Development and Modernization

organization: Episteme AS

role: Fullstack Developer

environment: production

period:
  from: 2023-02
  to: 2025-06

status: completed

technologies:
  - php
  - sql-server
  - rest-api
  - csharp
  - dotnet
  - javascript
  - iis

concepts:
  - enterprise-software
  - legacy-modernization
  - backend-development
  - api-design
  - system-integration
  - e-commerce
  - scalability
  - maintainability
  - debugging
  - production-systems
  - server-administration

dependencies:

links:
  github:
  live:

---

# Overview

Continuous development and modernization of a business-critical ERP platform and its connected e-commerce solution.

The project combined backend development, ERP customization, REST API integration, frontend modernization, SQL Server development, and IIS-based production operations. The systems supported daily business operations including warehouse management, retail, ordering, invoicing, and online sales.

A key part of the work was replacing direct database communication between the online store and ERP with a dedicated REST API, reducing system coupling while preserving established business workflows.

---

# Context

The solution consisted of an established ERP platform and an online store that were both central to daily business operations.

The existing online store was tightly coupled to the ERP database through direct database access. This made the storefront dependent on internal database structures and increased the impact of changes between the two systems.

At the same time, the ERP contained established business logic that could not simply be replaced without significant operational risk. New functionality therefore needed to be introduced incrementally while maintaining compatibility with existing workflows.

The work included:

- Modernizing the customer-facing online store.
- Introducing REST API communication between the online store and ERP.
- Extending ERP functionality based on business requirements.
- Developing SQL queries and data access logic.
- Maintaining IIS-hosted production environments.
- Investigating and resolving production issues.
- Supporting customer-specific business workflows.

Important constraints included:

- Business-critical systems had to remain operational.
- Existing ERP workflows needed to remain compatible.
- New functionality needed to work with established data structures.
- System coupling needed to be reduced without requiring a complete ERP replacement.
- Production changes needed to be introduced carefully.

---

# Task

My responsibility was continuous development, modernization, and operational maintenance of the ERP platform and connected systems.

I was responsible for:

- Extending ERP functionality.
- Redesigning the online store.
- Implementing REST API communication between systems.
- Replacing direct database access from the storefront.
- Developing SQL queries and data access logic.
- Implementing customer-specific functionality.
- Maintaining IIS server configuration.
- Investigating and resolving production issues.
- Supporting system stability and reliability.
- Developing dedicated interfaces for selected business processes.

The goal was to improve maintainability and system integration while keeping business-critical functionality stable and operational.

---

# Challenge

## Challenge: Modernizing a Tightly Coupled ERP Integration

### Problem

The existing online store communicated directly with the ERP database.

This created a tightly coupled architecture where the customer-facing application depended directly on internal ERP database structures. Changes to the ERP database could therefore affect the storefront, while the storefront also had direct knowledge of internal data structures.

The architecture made future development harder and increased the risk associated with changing either system.

At the same time, the ERP handled established business workflows that the company relied on for daily operations, so the integration could not simply be replaced without considering operational compatibility.

### Solution

A REST API layer was introduced between the online store and ERP.

The responsibilities were separated so that:

- The online store handled customer-facing functionality and ordering.
- The REST API provided controlled communication with ERP functionality.
- The ERP remained responsible for established business workflows, order processing, and invoicing.
- SQL queries and data access logic provided the API with the ERP data required by the storefront and other integrations.

The online store was also redesigned with a more modern responsive experience and updated authentication functionality.

This allowed the storefront to consume business functionality through defined API interfaces rather than accessing the ERP database directly.

### Result

The new integration reduced coupling between the online store and ERP and provided a cleaner boundary between the customer-facing application and internal business systems.

The architecture also created a more flexible foundation for additional applications and integrations without exposing the ERP database directly.

---

## Challenge: Extending a Mature Business-Critical ERP

### Problem

The ERP platform already contained established business logic and was deeply integrated into daily operations.

New requirements therefore had to be implemented without unnecessarily disrupting existing workflows or introducing regressions into production systems.

This required understanding existing business rules, database structures, and dependencies before modifying the system.

### Solution

New functionality was introduced incrementally through:

- ERP functionality extensions.
- Custom business workflows.
- SQL queries supporting new functionality.
- Data access logic.
- REST API extensions.
- Customer-specific functionality.
- Production debugging and troubleshooting.
- IIS configuration and operational maintenance.

Changes were implemented within the existing architecture where appropriate rather than attempting to replace established business functionality unnecessarily.

### Result

The ERP platform continued to support evolving business requirements while preserving the existing operational foundation.

The incremental approach reduced the risk associated with modifying a mature production system.

---

## Challenge: Extracting Business Functionality Into Dedicated Applications

### Problem

Some business processes were dependent on the original ERP user interface even though they represented focused workflows that could benefit from more specialized interfaces.

Continuing to handle every workflow directly through the ERP interface limited flexibility and made it harder to create user-focused tools around specific business processes.

### Solution

Selected ERP functionality was exposed through dedicated interfaces using the REST API.

This included functionality for:

- Product information management.
- Order office workflows and dashboards.
- Administrative interfaces.

These applications consumed ERP functionality through the API instead of communicating directly with the database.

The approach allowed selected workflows to be separated from the ERP user interface while keeping the ERP as the underlying source of business functionality.

### Result

Specific business processes could be handled through more focused interfaces without requiring a complete replacement of the ERP platform.

This established a gradual path for extracting functionality from the larger enterprise system.

---

# Action

## Architecture

### Frontend

The project included a redesigned customer-facing online store as well as dedicated interfaces for selected ERP-related workflows.

The online store was redesigned to provide:

- Responsive desktop and mobile layouts.
- Improved ordering workflows.
- Authentication functionality.
- API-based communication with backend systems.

Additional interfaces were developed for focused business processes such as product information management and order office workflows.

These interfaces consumed functionality through the REST API rather than accessing ERP database structures directly.

---

### Backend

The backend consisted primarily of the existing ERP platform extended with new business functionality and a REST API integration layer.

Responsibilities included:

- ERP business logic.
- REST API development.
- Order integration.
- Customer-specific functionality.
- Business workflow adjustments.
- SQL query development.
- Data access implementation.
- Production debugging.

The REST API provided a controlled interface between external applications and ERP functionality.

---

### Database

SQL Server was used as the primary database platform for the ERP system.

Database work focused on extending the existing data model and providing the data required by new functionality and API endpoints.

Responsibilities included:

- Developing SQL queries.
- Working with existing ERP data structures.
- Implementing data access logic.
- Supporting new ERP functionality.
- Providing API-accessible data.

The database remained part of the ERP's internal architecture rather than being exposed directly to customer-facing applications.

---

### Infrastructure

The production environment was hosted on Windows Server using IIS.

Operational responsibilities included:

- IIS administration.
- Server configuration.
- Deployment support.
- Production troubleshooting.
- System stability improvements.

The environment required ongoing maintenance because the ERP and connected applications supported business-critical operations.

---

## Technical Decisions

### Decision: Introduce a REST API Between Online Store and ERP

#### Context

The existing online store accessed the ERP database directly.

This created unnecessary coupling between the customer-facing application and internal database structures and made future development more difficult.

#### Chosen Solution

A REST API was introduced as the integration boundary between the online store and ERP.

The API provided controlled access to required ERP functionality and data while keeping the ERP database behind the backend boundary.

#### Alternatives Considered

- Continuing direct database communication between the online store and ERP.

This approach was functional but maintained the existing tight coupling and dependency on internal database structures.

#### Trade-offs

Advantages:

- Reduced coupling.
- Clearer separation of responsibilities.
- Improved maintainability.
- Easier development of additional integrations.
- Internal database structures remained behind the API boundary.

Disadvantages:

- Additional API development and maintenance.
- More components to operate and troubleshoot.
- API contracts need to remain compatible with consuming applications.

---

### Decision: Incremental Modernization Instead of ERP Replacement

#### Context

The ERP contained established business logic and was actively used for daily operations.

A complete replacement would introduce significant operational risk and require recreating existing business workflows.

#### Chosen Solution

The system was modernized incrementally by:

- Extending existing ERP functionality.
- Introducing API-based integrations.
- Redesigning external applications.
- Extracting selected workflows into dedicated interfaces.
- Maintaining compatibility with established ERP functionality.

#### Alternatives Considered

- Replacing the ERP platform entirely.
- Continuing to develop exclusively within the existing architecture.

A complete replacement was not practical for the operational context, while continuing without modernization would preserve the existing architectural limitations.

#### Trade-offs

Advantages:

- Lower operational risk.
- Existing business workflows remain available.
- Improvements can be delivered incrementally.
- Existing business logic can continue to be reused.

Disadvantages:

- Legacy constraints remain in parts of the system.
- Modernization takes longer than a clean-slate implementation.
- New components must coexist with existing architecture.

---

### Decision: Keep ERP as the Core Business System

#### Context

The ERP already contained established business rules for operational workflows such as ordering and invoicing.

Duplicating this logic in external applications would create additional consistency and maintenance problems.

#### Chosen Solution

The ERP remained responsible for core business functionality while external applications consumed selected capabilities through the REST API.

This allowed new interfaces to be developed without duplicating the underlying business logic.

#### Alternatives Considered

- Moving business logic into each consuming application.
- Reimplementing ERP functionality in a separate backend.

These approaches would have introduced duplicated business rules and increased the risk of inconsistent behavior between systems.

#### Trade-offs

Advantages:

- Centralized business logic.
- Reduced duplication.
- Existing workflows remain authoritative.
- New interfaces can be developed independently.

Disadvantages:

- External applications remain dependent on ERP capabilities.
- Legacy ERP constraints can limit API functionality.
- The ERP remains an important architectural dependency.

---

## Implementation

### Features

Implemented functionality and modernization work included:

- ERP functionality extensions.
- Customer-specific business workflows.
- Complete redesign of the online store frontend.
- Responsive desktop and mobile storefront.
- Online store authentication.
- REST API integration between ERP and external applications.
- Product information management interfaces.
- Order office dashboards and workflows.
- Administrative interfaces built on top of the REST API.
- SQL-backed data access for new functionality.
- IIS-based production support.

### APIs

The REST API provided controlled communication between the ERP platform and external applications.

Important API capabilities included:

- Access to ERP data required by external applications.
- Online order integration.
- Product information access.
- Administrative functionality.
- Communication with dedicated business interfaces.

The API replaced direct database communication from the online store and established a defined integration boundary.

### Data and Persistence

SQL Server remained the primary persistence layer for the ERP platform.

Implementation work included:

- SQL query development.
- Data access logic.
- Integration with existing ERP data structures.
- Retrieval of data required by API functionality.
- Support for new business workflows.

External applications accessed required data through the API rather than connecting directly to the ERP database.

### Automation

No significant CI/CD or scheduled automation is documented as part of this project.

Deployment and operational maintenance were primarily handled through the IIS-hosted production environment.

### Testing

Testing was primarily performed through development, integration, and production troubleshooting of the existing business systems.

The work required validating new functionality against established ERP workflows and investigating production issues when they occurred.

---

# Result

The ERP platform and connected applications became more maintainable and easier to extend while preserving the existing business-critical workflows.

Key outcomes included:

- Modernized customer-facing online store.
- REST API integration replacing direct storefront database access.
- Reduced coupling between external applications and the ERP database.
- Dedicated interfaces for selected business processes.
- Continued support for evolving business requirements.
- Improved separation between customer-facing applications and internal ERP functionality.
- Continued operational support of the production environment.

The modernization approach allowed the platform to evolve without requiring a complete replacement of the established ERP system.

---

# Lessons Learned

## Lesson: Modernize Around Stable Business Boundaries

Legacy systems do not always need to be replaced to become more maintainable.

Introducing clear interfaces around existing business functionality can provide significant architectural improvements while preserving established workflows.

This reinforced the value of identifying stable boundaries before deciding that a system needs to be rewritten.

---

## Lesson: Avoid Exposing Internal Database Structures

Direct database access from external applications creates a strong dependency on internal implementation details.

Introducing an API boundary makes the integration contract explicit and gives the backend more control over how internal data and business logic are exposed.

This changed how I approach integrations between systems: external consumers should depend on stable capabilities rather than internal storage structures whenever practical.

---

## Lesson: Incremental Modernization Requires Architectural Discipline

Working with a mature ERP demonstrated that modernization is not simply about introducing newer technologies.

Changes need to account for existing business rules, dependencies, production stability, and operational risk.

A technically cleaner solution is not automatically the better solution if it introduces unacceptable business risk.

---

## Lesson: Specialized Interfaces Can Extend the Lifetime of Legacy Systems

Extracting focused workflows into dedicated applications can provide many of the benefits of modernization without requiring the underlying ERP to be replaced.

This demonstrated the value of keeping established business logic in place while gradually improving how users and external systems interact with it.

---

# Future Improvements

- Introduce automated integration tests around critical ERP and API workflows.
- Add structured API documentation and contract validation.
- Introduce centralized logging and production observability.
- Improve deployment automation for API and frontend applications.
- Add automated monitoring and alerting for critical production services.
- Continue extracting suitable ERP workflows into dedicated applications and API-based integrations.
- Gradually reduce remaining direct dependencies on legacy ERP interfaces.

---