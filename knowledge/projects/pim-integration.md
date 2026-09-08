---
title: Product Information Management Integration in ERP

organization: Episteme AS

role: Fullstack Developer

environment: development

period:
  from: 2022-01
  to: 2025-01

status: completed

technologies:
  - php
  - sql-server
  - rest-api
  - csharp
  - dotnet
  - javascript
  - css
  - bootstrap
  - iis

concepts:
  - enterprise-software
  - product-information-management
  - data-modeling
  - system-integration
  - api-design
  - data-consistency
  - synchronization
  - backend-development
  - ecommerce
  - erp
  - maintainability

dependencies:

links:
  github:
  live:

---

# Overview

The project extended an existing ERP platform with Product Information Management (PIM) capabilities, creating a centralized system for managing and organizing product information.

The PIM functionality was integrated with the company's online store through a REST API, allowing product information maintained in the ERP to be exposed consistently to external systems. The goal was to reduce duplicated product data, improve data consistency, and provide better visibility and control over product information.

The work combined backend development, database development, API integration, and frontend functionality within an existing enterprise system.

---

# Context

The existing ERP system already contained core product and business information, but product data needed to be managed more systematically across the ERP and connected online store.

Product information could otherwise become difficult to maintain when the same information was used across multiple systems. Inconsistent or duplicated data increases maintenance effort and creates a risk that customers and internal users see outdated or conflicting product information.

The project therefore focused on evolving the existing ERP into a more capable product information management platform while preserving its role as an established business system.

Important requirements included:

- Centralizing product information.
- Improving the structure and overview of product data.
- Keeping product information consistent between ERP and online store.
- Supporting existing ERP workflows.
- Integrating with the existing online store.
- Extending the existing platform without introducing unnecessary architectural disruption.

The work was also part of the broader continuous development of the ERP platform, where new functionality had to coexist with established business logic and production systems.

---

# Task

As a Fullstack Developer, I was responsible for designing and implementing PIM functionality within the existing ERP platform and integrating it with the connected online store.

My responsibilities included:

- Extending the ERP with product information management functionality.
- Designing and implementing functionality for organizing product information.
- Developing database queries and data access logic.
- Implementing REST API functionality for external system integration.
- Integrating product information with the online store.
- Developing frontend functionality for managing product information.
- Maintaining compatibility with the existing ERP architecture.
- Supporting ongoing improvements to the surrounding ERP functionality.

The primary goal was to establish a more centralized and maintainable approach to product information while ensuring that connected systems received consistent product data.

---

# Challenge

## Challenge: Establishing a Central Source for Product Information

### Problem

Product information was used across the ERP and online store, creating a need for a more structured way to manage and maintain that information.

Without a clear central source, product data can become duplicated across systems and difficult to keep synchronized. Changes made in one location may not automatically be reflected elsewhere, increasing the risk of inconsistent product information.

The challenge was to improve product information management without replacing the existing ERP platform or disrupting established business processes.

### Solution

Extended the existing ERP platform with PIM functionality so that product information could be managed and organized centrally.

The PIM functionality provided a dedicated way of structuring product-related information within the existing ERP environment. Database queries and data access logic were developed to retrieve and manage the required information.

The ERP was kept as the central system for product information, while the REST API provided a controlled integration boundary for the online store.

This separated product information management from the presentation and customer-facing concerns of the online store while allowing the existing ERP to remain responsible for the underlying business data.

### Result

Product information could be managed from a more centralized location within the ERP, improving overview and reducing the need to maintain the same information independently across systems.

The approach also established a clearer integration boundary between the ERP and online store.

---

## Challenge: Keeping Product Data Consistent Across Systems

### Problem

The online store depended on product information originating from the ERP, but the two systems had different responsibilities and technical interfaces.

Directly duplicating product information in the online store would increase the risk of outdated data and create additional maintenance work.

The integration therefore needed to expose the relevant ERP information in a controlled way while keeping the ERP as the authoritative source.

### Solution

Implemented REST API integration between the ERP's PIM functionality and the online store.

The API exposed relevant product information from the ERP to the online store rather than requiring the storefront to manage an independent copy of the underlying ERP data.

The integration provided a defined communication boundary between the systems and allowed product information maintained in the ERP to be consumed by the customer-facing application.

This approach built on the existing ERP and online-store architecture while reducing unnecessary coupling between the systems.

### Result

The online store could consume product information from the centralized PIM functionality, improving consistency between the internal product data and customer-facing product information.

Changes to product information could be managed from the central system instead of requiring equivalent changes to be maintained independently in the storefront.

---

## Challenge: Extending a Mature ERP Platform

### Problem

The PIM functionality was introduced into an existing ERP platform with established business logic, database structures, and production workflows.

Adding a substantial new capability to such a system requires understanding the existing architecture and extending it without unnecessarily disrupting existing functionality.

The solution also needed to fit the technical conventions of the existing platform rather than introducing an isolated system that would increase operational complexity.

### Solution

Implemented the PIM functionality incrementally within the existing ERP architecture.

The work combined:

- Backend development.
- SQL-based data access.
- Frontend functionality.
- REST API integration.
- Integration with the existing online store.

Existing ERP data structures and business logic were reused where appropriate, while new functionality was introduced around the specific requirements of product information management.

The PIM functionality therefore became an extension of the existing ERP platform rather than a separate product information system requiring an additional operational platform.

### Result

The ERP platform gained dedicated PIM capabilities without requiring a separate product management system.

The approach reduced architectural duplication and allowed the new functionality to benefit from the existing ERP infrastructure and business processes.

---

# Action

## Architecture

### Frontend

The PIM functionality was integrated into the existing ERP frontend and provided interfaces for managing and organizing product information.

The frontend was built using the existing web technology stack and provided functionality for working with centrally managed product data.

The frontend communicated with backend functionality responsible for accessing and updating the underlying ERP data.

---

### Backend

The backend extended the existing ERP application with PIM-related business functionality and REST API capabilities.

Responsibilities included:

- Product information management.
- Product data access.
- Business logic integration.
- REST API communication.
- Integration with the online store.

The backend remained part of the existing ERP platform, allowing PIM functionality to reuse established application and business logic where appropriate.

---

### Database

SQL Server was used for persistent ERP data.

The PIM functionality extended the existing data model and data access layer to support additional product information and related functionality.

Database work included:

- SQL query development.
- Product data retrieval.
- Data access logic.
- Integration between existing ERP data and new PIM functionality.

The database remained part of the existing ERP architecture rather than introducing a separate product database.

---

### Infrastructure

The PIM functionality operated as part of the existing ERP environment.

The surrounding production environment used IIS for hosting, with the ERP and connected applications maintained as part of the existing infrastructure.

The integration therefore extended the existing production platform rather than introducing separate infrastructure dedicated to PIM.

---

## Technical Decisions

### Decision: Use the ERP as the Central Product Information Source

#### Context

Product information was required by both internal ERP functionality and the online store.

Maintaining separate product information in each system would create unnecessary duplication and increase the risk of inconsistent data.

#### Chosen Solution

The ERP was extended with PIM capabilities and used as the central source for product information.

The online store consumed the relevant product information through the integration layer rather than becoming an independent source of product data.

#### Alternatives Considered

Maintaining product information independently in the online store was a possible alternative, but this would have introduced duplicated data and additional synchronization requirements.

#### Trade-offs

Advantages:

- Centralized product information.
- Reduced duplication.
- Clear ownership of product data.
- Easier maintenance.
- Better consistency between connected systems.

Disadvantages:

- The ERP became more important to product management workflows.
- Changes to the ERP data model could affect integrated systems.
- The API integration introduced an additional dependency between the systems.

---

### Decision: REST API as the Integration Boundary

#### Context

The online store needed access to ERP product information without tightly coupling its implementation directly to the ERP database.

A defined integration boundary was needed to separate internal ERP data management from the customer-facing application.

#### Chosen Solution

A REST API was introduced as the communication layer between the ERP/PIM functionality and the online store.

The API exposed the product information required by the storefront while keeping database access inside the ERP/backend layer.

#### Alternatives Considered

Direct database access from the online store was an existing integration approach, but it created tighter coupling between the systems and exposed internal database structures to the external application.

#### Trade-offs

Advantages:

- Reduced database coupling.
- Clearer separation of responsibilities.
- Controlled access to ERP data.
- Easier evolution of the online store.
- Reusable integration interface.

Disadvantages:

- Additional API development and maintenance.
- More components involved in the data flow.
- API failures can affect communication between ERP and online store.

---

### Decision: Incremental Extension of the Existing ERP

#### Context

The ERP was already an established business-critical system with existing data, functionality, and workflows.

Replacing the ERP or introducing a completely separate PIM platform would increase migration complexity and operational risk.

#### Chosen Solution

PIM functionality was implemented as an extension of the existing ERP platform.

The existing application, database, infrastructure, and business logic were reused where appropriate.

#### Alternatives Considered

- Introducing a separate dedicated PIM platform.
- Rebuilding the product management functionality as a standalone application.
- Continuing without centralized product information management.

#### Trade-offs

Advantages:

- Lower architectural disruption.
- Reuse of existing ERP data and infrastructure.
- No separate PIM platform to operate.
- Faster integration with existing business workflows.

Disadvantages:

- PIM remains coupled to the ERP platform.
- Legacy constraints remain relevant.
- Future migration to a dedicated PIM platform would require additional work.

---

## Implementation

### Features

Implemented functionality included:

- Centralized product information management.
- ERP-based product information organization.
- Product data management within the existing ERP.
- REST API integration with the online store.
- Consumption of ERP product information by the storefront.
- Extended ERP functionality supporting product management.

### APIs

A REST API was implemented to provide product information from the ERP/PIM functionality to the connected online store.

The API acted as the integration boundary between the internal ERP system and the customer-facing storefront.

It allowed the online store to consume relevant product information without directly accessing the ERP database.

### Data and Persistence

Product information was stored within the existing ERP database infrastructure using SQL Server.

Database development included SQL queries and data access logic required to support the PIM functionality and API integration.

The ERP remained the authoritative source for the centrally managed product information.

### Automation

No dedicated automation beyond the application integration and existing system workflows is documented for this project.

### Testing

The available project information does not document a dedicated automated test suite for the PIM functionality.

Validation was performed as part of development and integration work within the existing ERP and online-store environment.

---

# Result

The ERP system was extended with PIM capabilities that provided a more centralized approach to product information management.

The project achieved:

- Centralized management of product information.
- Improved overview of product data.
- REST API integration between ERP/PIM and the online store.
- More consistent product information across connected systems.
- Reduced dependency on duplicated product data.
- Extended ERP functionality without introducing a separate PIM platform.

The solution also created a clearer separation between internal product data management and the customer-facing online store.

---

# Lessons Learned

## Lesson: Centralized Data Ownership Reduces Synchronization Complexity

Managing product information across multiple systems makes data ownership an important architectural concern.

By establishing the ERP/PIM functionality as the central source, the number of places responsible for maintaining product information was reduced.

This reinforced the importance of deciding where data ownership belongs before designing synchronization between systems.

---

## Lesson: Integration Boundaries Matter in Enterprise Systems

The project reinforced the value of using an API boundary between an internal business system and a customer-facing application.

Direct database integration can be convenient initially, but it creates tight coupling between implementation details that should ideally remain independent.

Using an API makes the relationship between systems more explicit and gives each system clearer responsibilities.

---

## Lesson: Incremental Modernization Is Often More Practical Than Replacement

Working within an established ERP demonstrated that modernization does not always require rebuilding the underlying system.

Adding focused capabilities and improving integration boundaries can provide significant improvements while preserving existing business functionality.

If rebuilding the solution today, I would continue to separate new functionality from legacy ERP concerns where practical and consider whether a dedicated PIM service would eventually provide better long-term independence.

---

# Future Improvements

- Introduce stronger automated integration testing between PIM and the online store.
- Add structured API documentation and contract validation.
- Introduce more explicit versioning of the product API.
- Improve observability around synchronization and integration failures.
- Add validation and auditing for changes to product information.
- Consider extracting PIM functionality into a more independent service if product complexity and system scale justify it.
- Introduce more explicit synchronization status and error handling for product data consumed by external systems.

---