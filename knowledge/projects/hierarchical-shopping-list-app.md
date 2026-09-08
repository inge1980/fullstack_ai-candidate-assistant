---
title: Offline-First Hierarchical Shopping List App

organization: Personal Project

role: Fullstack Developer

environment: development

period:
  from: 2024-10
  to: Present

status: active

technologies:
  - react-native
  - typescript
  - redux
  - sqlite
  - supabase
  - postgresql
  - github-actions
  - vercel
  - dotenv
  - rclone
  - react-native-gesture-handler
  - react-native-reanimated

concepts:
  - architecture
  - authentication
  - automation
  - ci-cd
  - database-design
  - database-migrations
  - deployment-automation
  - devops
  - environment-management
  - local-persistence
  - mobile-development
  - mobile-performance
  - offline-first
  - qr-authentication
  - single-source-of-truth
  - state-management
  - synchronization
  - web-authentication

dependencies:
  - react-native-gesture-handler
  - react-native-reanimated
  - "@reduxjs/toolkit"
  - "@supabase/supabase-js"
  - react-native-sqlite-storage

links:
  github:
  live:

---

# Overview

An offline-first React Native application for managing hierarchical shopping lists and shared family shopping data.

The application allows family members to create nested shopping lists, reorder items through gesture-based interactions, and share lists across a household. Parents authenticate through email, while children can join an existing family through a QR-based device linking flow.

The application is designed around Redux as the runtime Single Source of Truth, with SQLite providing local persistence and Supabase PostgreSQL providing remote synchronization. The architecture is intended to keep the application usable without network connectivity while supporting reliable synchronization when connectivity becomes available.

---

# Context

The project was created as a personal exploration of how to build a production-oriented mobile application that remains useful when network connectivity is unavailable.

The application needed to support more than a traditional single-user shopping list. Family members need to share lists, while child onboarding should be simpler than conventional account registration. At the same time, the application should avoid making network connectivity a requirement for basic interactions.

The main constraints were:

- The application needed to work offline.
- User interactions needed to feel immediate.
- Local data needed to survive application restarts.
- Local and remote data needed to synchronize.
- Application state needed a clear owner.
- Family membership needed controlled authorization.
- Child onboarding needed to minimize unnecessary personal information.
- Database schema changes needed to be handled safely on mobile devices.
- Development and production environments needed separate configuration.
- Database backups and deployment needed to be automated.

The project therefore combines mobile UI architecture, local persistence, synchronization, authentication, authorization, deployment automation, and operational infrastructure.

---

# Task

I was responsible for designing and implementing the application's architecture and technical solution.

My responsibilities included:

- Designing the React Native application architecture.
- Defining Redux as the runtime state owner.
- Implementing hierarchical shopping-list interactions.
- Implementing drag-and-drop and swipe interactions.
- Designing the offline-first persistence flow.
- Implementing SQLite persistence.
- Implementing the synchronization layer with Supabase.
- Designing the family and group authorization model.
- Implementing QR-based child/device linking.
- Designing and implementing the local SQLite migration system.
- Configuring development and production environments.
- Implementing CI/CD automation.
- Implementing automated PostgreSQL backup workflows.
- Making performance decisions for gesture-heavy interactions.

The goal was to create a maintainable application where user interactions remained immediate and predictable regardless of network availability.

---

# Challenge

## Challenge: Designing an Offline-First State and Synchronization Model

### Problem

The application had three potentially competing sources of data:

- Redux application state.
- Local SQLite persistence.
- Remote Supabase PostgreSQL data.

Allowing UI components to read and write directly against multiple persistence layers would make ownership unclear and increase the risk of inconsistent state.

The application also needed to remain usable when the network was unavailable. User actions therefore could not depend on a successful remote request before being reflected in the UI.

### Solution

I defined Redux as the runtime Single Source of Truth.

The data flow is:

1. The user performs an action.
2. Redux updates immediately.
3. The updated state is persisted to SQLite.
4. Synchronization detects local changes that need to be sent to Supabase.
5. Remote data is synchronized when connectivity is available.
6. Resulting state is fed back into Redux.

The UI therefore consumes application state from Redux rather than directly querying SQLite or Supabase.

SQLite provides durable local persistence, while Supabase PostgreSQL acts as the remote synchronization target.

This separates three responsibilities:

- Redux owns active application state.
- SQLite owns durable local state.
- Supabase owns centralized synchronized state.

### Result

The application remains usable without network connectivity and can synchronize changes when connectivity returns.

The architecture also provides a clear ownership model, reducing the risk of different UI components independently treating SQLite or Supabase as the application's source of truth.

---

## Challenge: Maintaining Smooth Gesture-Based Interactions

### Problem

The application relies heavily on drag-and-drop interactions for hierarchical shopping lists.

Gesture updates can occur many times per second. If every movement causes JavaScript-thread execution and React state updates, gesture-heavy interfaces can become visibly laggy.

The interaction therefore needed to remain responsive while the user moved and reordered items.

### Solution

I implemented the gesture system using React Native Gesture Handler and Reanimated.

Gesture processing and animation work are moved to the UI thread where possible rather than requiring every frame to pass through the JavaScript thread.

The interaction model supports:

- Press-and-hold dragging.
- Vertical item movement.
- Hierarchical item reordering.
- Swipe-to-delete interactions.
- Immediate visual feedback during interaction.

The architecture separates high-frequency animation state from the application's persistent business state so that dragging does not unnecessarily trigger broad Redux or React updates.

### Result

Drag-and-drop interactions remain responsive while users reorder items, with reduced dependence on JavaScript-thread execution during high-frequency gesture updates.

---

## Challenge: Secure Parent and Child Family Linking

### Problem

The application needed to allow children to join an existing family without requiring a traditional account-registration flow.

At the same time, simply placing account information in a QR code would create unnecessary security and privacy risks.

The system needed to ensure that:

- Parents retain ownership of the family.
- Child devices can be linked without exposing account credentials.
- Invitations expire.
- Invitations cannot be reused.
- Family membership determines access to shared lists.
- Child-related personal information is minimized.

### Solution

I implemented a temporary invitation and device-linking workflow.

A parent creates an invitation for a child device. The invitation contains a unique QR token and temporary verification code rather than exposing account credentials or sensitive account information.

The invitation contains lifecycle information such as:

- Unique `qr_token`.
- Temporary verification code.
- `expires_at`.
- `used` state.

The child device scans the QR code and completes the linking process. Once linked, the child can use a simplified authentication mechanism based on a PIN rather than going through the parent's email-based authentication flow.

The authorization model separates authentication from family membership. Devices and users are associated with family groups, while group membership and roles determine access to shared lists.

The relevant data model includes entities such as:

- `device_invitations`
- `devices`
- `users`
- `usergroupmembership`
- `sharedlists`

The device model references the parent-controlled account structure rather than requiring unnecessary personal information about the child device owner.

### Result

Children can join an existing family with a significantly simpler onboarding flow while the system retains explicit ownership, invitation expiration, authorization, and lifecycle controls.

The QR code acts as a temporary linking mechanism rather than an alternative authentication credential.

---

## Challenge: Evolving the Local SQLite Schema Safely

### Problem

An offline-first mobile application cannot assume that every installed version starts with the latest database schema.

Users may upgrade from an older application version while retaining existing local data.

Manually changing the schema would create inconsistent database states between application versions and could prevent the application from starting correctly.

### Solution

I implemented a custom SQLite migration runner.

Migrations execute sequentially and maintain a record of completed versions.

The migration process:

1. Determines the current local schema version.
2. Identifies migrations that have not yet been executed.
3. Runs migrations in deterministic order.
4. Records successfully completed migrations.
5. Prevents completed migrations from running again.
6. Stops initialization if a migration fails.

Migration folders and steps provide an explicit ordering model for schema changes.

The system intentionally focuses on controlled forward migrations. Automatic rollback is not currently implemented.

### Result

Local database changes can be introduced incrementally without requiring users to recreate their local data.

A failed migration prevents the application from continuing with an uncertain database state rather than silently running against a partially upgraded schema.

---

## Challenge: Separating Development and Production Environments

### Problem

The application uses multiple external services, including Supabase and Vercel.

Hardcoding service endpoints or manually changing configuration before deployments creates a significant risk of accidentally connecting development builds to production infrastructure or deploying incorrect configuration.

### Solution

I implemented environment-specific configuration using local `.env` files during development and Vercel Environment Variables for deployed environments.

The application selects the appropriate service configuration based on the environment rather than embedding environment-specific endpoints directly into application code.

This keeps development and production configuration separate while allowing the same application codebase to be deployed to different environments.

### Result

Environment-specific deployments can be configured without modifying application source code.

This reduces configuration mistakes and makes deployment behavior more predictable.

---

# Action

## Architecture

### Frontend

The mobile application is built with React Native and TypeScript.

Redux provides centralized runtime application state, while the UI is structured into reusable components for shopping lists, items, authentication, family management, and interaction controls.

Important frontend technologies include:

- React Native.
- TypeScript.
- Redux.
- React Native Gesture Handler.
- React Native Reanimated.

The application supports hierarchical shopping-list structures and gesture-based interactions such as:

- Drag-and-drop sorting.
- Nested item movement.
- Swipe-to-delete.
- Quick item creation.

The UI does not directly treat SQLite or Supabase as its state source. Instead, application state flows through Redux, with persistence and synchronization handled separately.

---

### Backend

Supabase provides the backend services used by the application.

The backend architecture includes:

- Authentication.
- PostgreSQL persistence.
- User profiles.
- Device registration.
- Family groups.
- Group membership.
- Role-based authorization.
- QR invitation management.
- Shared list ownership.
- Synchronization.

A Vercel-hosted web application provides browser-based authentication flows such as email verification callbacks.

The backend uses group membership and roles to determine which users and devices can access shared family data.

---

### Database

The application uses a hybrid local/cloud persistence architecture.

SQLite is used on the mobile device for durable local storage and offline functionality.

Supabase PostgreSQL provides centralized remote storage and synchronization.

The data model contains relationships for:

- Users.
- Devices.
- Family groups.
- Group membership.
- Shared lists.
- Shopping-list items.
- Device invitations.

Hierarchical shopping-list structures are represented through self-referencing relationships so that lists and items can contain nested children.

Timestamps and synchronization metadata are used to track local and remote state.

The local SQLite schema is managed through a custom migration system.

---

### Infrastructure

The project combines mobile application infrastructure with managed cloud services and automation.

Infrastructure includes:

- React Native mobile application.
- Vercel-hosted authentication web application.
- Supabase services.
- PostgreSQL.
- GitHub Actions.
- Environment-specific configuration.
- Automated database backups.

Development and production environments use separate configuration.

Local development uses `.env` files, while deployed environments use Vercel Environment Variables.

---

## Technical Decisions

### Decision: Redux as the Runtime Single Source of Truth

#### Context

The application has local persistence, remote persistence, and active UI state.

Without a clear ownership model, components could start reading directly from different data sources and produce inconsistent behavior.

#### Chosen Solution

Redux was selected as the central runtime state layer.

Components consume Redux state, while SQLite and Supabase act as persistence and synchronization layers around that state.

This provides a predictable unidirectional state flow.

#### Alternatives Considered

Direct database-driven UI was considered, where components would read directly from SQLite or Supabase.

This was rejected because it would couple presentation directly to persistence and make offline synchronization harder to reason about.

#### Trade-offs

Advantages:

- Clear state ownership.
- Predictable data flow.
- Easier debugging.
- Separation between UI and persistence.
- Better control over synchronization.

Disadvantages:

- Additional application architecture.
- Synchronization logic becomes an explicit responsibility.
- State and persistence models must remain consistent.

---

### Decision: React Native Reanimated for Gesture-Driven Animation

#### Context

Drag-and-drop interactions require frequent updates and need to remain responsive.

#### Chosen Solution

Reanimated 3 is used together with React Native Gesture Handler so that animation and gesture work can execute on the UI thread where appropriate.

#### Alternatives Considered

React Native's traditional Animated API was an alternative.

The chosen approach provided better control over high-frequency gesture interactions and reduced JavaScript-thread dependency.

#### Trade-offs

Advantages:

- Smoother gesture interactions.
- Reduced JavaScript-thread workload.
- Better suitability for complex drag interactions.

Disadvantages:

- More complex programming model.
- Worklet/UI-thread execution introduces additional debugging considerations.

---

### Decision: SQLite for Local Persistence

#### Context

Offline functionality requires data to remain available when the device has no network connection.

The application also needs persistence across application restarts.

#### Chosen Solution

SQLite is used as the mobile persistence layer.

Local state is persisted independently from the remote PostgreSQL database so that the application can continue operating offline.

#### Alternatives Considered

A purely remote data model was unsuitable because network connectivity would become a prerequisite for normal operation.

A simpler key-value storage approach would also be insufficient for the relational and hierarchical data model.

#### Trade-offs

Advantages:

- Reliable local persistence.
- Offline support.
- Relational data model.
- Efficient local queries.
- Data survives application restarts.

Disadvantages:

- Requires schema migration management.
- Local and remote schemas must remain compatible.
- Synchronization becomes more complex.

---

### Decision: Custom SQLite Migration System

#### Context

The application needs controlled schema evolution across installed versions.

#### Chosen Solution

A custom migration runner executes migrations sequentially and records completed migration versions.

The current approach provides deterministic forward migration without automatic rollback.

#### Alternatives Considered

An external migration framework could provide more functionality, but a custom implementation was chosen to keep the local database lifecycle explicit and lightweight.

#### Trade-offs

Advantages:

- Full control over migration behavior.
- Small implementation footprint.
- No additional migration dependency.
- Explicit execution order.

Disadvantages:

- Migration testing is the application's responsibility.
- Rollback and recovery mechanisms require additional implementation.
- Long-term migration management becomes an internal maintenance responsibility.

---

### Decision: Temporary QR Invitations for Child Device Linking

#### Context

Children need a simple way to join an existing family without exposing parent credentials or requiring full account registration.

#### Chosen Solution

The application uses temporary invitations containing unique QR tokens and verification information.

Invitations have expiration and single-use semantics.

Authorization is then handled through family membership and roles rather than treating the QR token as a permanent credential.

#### Alternatives Considered

Embedding account information directly in the QR code was rejected because it could expose sensitive information.

Requiring children to create independent email-based accounts would also make the onboarding flow unnecessarily complex for the intended use case.

#### Trade-offs

Advantages:

- Simple onboarding.
- Temporary invitation lifecycle.
- Reduced exposure of account information.
- Explicit family authorization.
- Parent-controlled ownership.

Disadvantages:

- Requires invitation lifecycle management.
- QR linking introduces an additional authentication/linking flow.
- PIN-based child access requires careful handling and rate limiting as the system matures.

---

### Decision: Environment-Based Configuration

#### Context

Development and production use different backend resources and deployment environments.

#### Chosen Solution

Local `.env` files are used during development, while deployed environments use Vercel Environment Variables.

Application configuration is resolved from the active environment.

#### Alternatives Considered

Hardcoded service endpoints and manually changing configuration before deployments were rejected because they increase deployment risk.

#### Trade-offs

Advantages:

- Clear environment separation.
- Lower risk of accidentally targeting production.
- Same codebase can serve multiple environments.
- Easier automated deployment.

Disadvantages:

- Environment configuration must be maintained correctly.
- Missing or incorrect variables can prevent deployment or runtime initialization.

---

### Decision: Progressive Synchronization Architecture

#### Context

The application must provide immediate local interaction while eventually synchronizing data with a remote database.

#### Chosen Solution

Local changes are applied to the application state immediately and persisted locally. Synchronization with Supabase occurs separately when connectivity is available.

#### Alternatives Considered

A remote-first approach would make user interaction dependent on network availability.

A local-only architecture would not provide family-wide synchronization.

#### Trade-offs

Advantages:

- Immediate user feedback.
- Offline functionality.
- Centralized remote data.
- Supports multiple devices.

Disadvantages:

- Conflict resolution is not yet fully sophisticated.
- Synchronization failures require explicit handling.
- Local and remote state can temporarily diverge.

---

## Implementation

### Features

The application currently includes:

- Hierarchical shopping lists.
- Nested list and item structures.
- Drag-and-drop sorting.
- Swipe-to-delete interactions.
- Offline-first operation.
- Local SQLite persistence.
- Redux-based application state.
- Supabase synchronization.
- Parent email authentication.
- QR-based child/device linking.
- Temporary invitations.
- Family and group membership.
- Role-based access control.
- Development and production environments.

### APIs

The backend capabilities include:

- User authentication.
- User profile management.
- Device registration.
- Family/group management.
- QR invitation creation and validation.
- Invitation expiration and single-use handling.
- Group membership management.
- Shared list access.
- Role-based authorization.
- PostgreSQL persistence.
- Synchronization between mobile clients and remote storage.

The web authentication application handles browser-based authentication flows such as email verification callbacks.

### Data and Persistence

SQLite provides the local persistence layer for:

- Shopping lists.
- Hierarchical items.
- Local user/application state.
- Synchronization metadata.
- Local configuration.

Supabase PostgreSQL provides centralized cloud persistence.

The local database uses sequential migrations to evolve its schema between application versions.

Hierarchical relationships are represented through self-referencing database relationships.

### Automation

GitHub Actions is used for automation workflows including:

- PostgreSQL backup scheduling.
- Database dump generation.
- Backup compression.
- Cloud backup upload using rclone.
- Retry handling.
- Failure notifications.
- Backup retention cleanup.
- Deployment automation.
- Environment-aware deployment validation.

### Testing

Development and functionality are validated through application testing and deployment workflows.

Testing focuses on:

- Offline application behavior.
- Local persistence.
- Database migrations.
- Authentication flows.
- QR linking.
- Gesture-based interactions.
- Synchronization behavior.
- Development and production configuration.

Automated end-to-end synchronization testing and comprehensive conflict-resolution testing are not yet implemented.

---

# Result

The project has evolved into a production-oriented React Native application with a clear offline-first architecture.

The application can operate without network connectivity while maintaining durable local state and synchronizing with Supabase PostgreSQL when connectivity is available.

The architecture provides:

- Immediate local interactions.
- Centralized Redux application state.
- Durable SQLite persistence.
- Cloud synchronization.
- Hierarchical shopping-list management.
- Gesture-based interactions.
- Family-based authorization.
- QR-based child/device onboarding.
- Environment-separated deployments.
- Automated PostgreSQL backups.
- Automated CI/CD workflows.

The project also provides a practical foundation for further work on synchronization conflict resolution, observability, automated testing, and production deployment.

---

# Lessons Learned

## Lesson: Offline-First Requires Explicit State Ownership

Offline-first systems become difficult when multiple persistence layers are treated as competing sources of truth.

Defining Redux as the runtime state owner made the architecture easier to reason about. SQLite and Supabase have different responsibilities rather than both attempting to control application state.

This changed the way I approach offline architecture: persistence and synchronization should support application state rather than become accidental state-management mechanisms themselves.

---

## Lesson: Gesture Performance Must Be Designed Early

Drag-and-drop interactions expose JavaScript-thread bottlenecks quickly.

Trying to optimize gesture performance after building the entire UI would make the problem harder to solve because rendering, state updates, and animation would already be tightly coupled.

Using Gesture Handler and Reanimated from the beginning allowed high-frequency interaction work to remain separate from broader application state updates.

---

## Lesson: Local Database Migrations Are Part of the Application Lifecycle

A mobile database cannot be treated like a disposable development database.

Users may have years of local data when a new application version is installed.

The custom migration system reinforced the importance of deterministic schema evolution, explicit versioning, and failing safely when a migration cannot be completed.

---

## Lesson: Simple Onboarding Still Requires Strong Authorization

QR-based onboarding can look simple from a UX perspective, but the underlying security model cannot be simplified to "scan a QR code and grant access."

The QR code should identify a temporary linking operation rather than expose account credentials.

Separating invitation, authentication, device registration, and authorization resulted in a clearer model and reduced unnecessary exposure of personal information.

---

## Lesson: Offline Synchronization Needs Conflict Strategy

The current architecture handles local persistence and synchronization, but simultaneous updates from multiple devices introduce a deeper problem: what happens when two devices modify the same data before synchronizing?

This is an area where the current implementation can be improved.

If rebuilding the synchronization layer today, I would define conflict semantics explicitly before increasing the number of synchronized entities or clients.

---

## Lesson: Operational Automation Is Part of Application Architecture

Backups, deployment configuration, and environment management are not separate concerns once an application depends on cloud infrastructure.

Automating PostgreSQL backups and deployment workflows reduces operational mistakes and makes the development environment more representative of how the application is actually operated.

---

# Future Improvements

- Implement explicit conflict-resolution strategies for simultaneous updates.
- Add automated end-to-end synchronization tests across multiple simulated devices.
- Add telemetry for synchronization failures and retry behavior.
- Improve observability around local/remote state divergence.
- Add automated migration tests covering upgrade paths from previous schema versions.
- Add comprehensive end-to-end testing using a tool such as Detox.
- Improve database indexing for larger shopping-list datasets.
- Add background synchronization where supported by the platform.
- Add push notifications for relevant family/list changes.
- Strengthen PIN security with rate limiting and additional abuse protection.
- Add automated backup restore testing rather than validating backups only through successful dump generation.
- Improve deployment validation and rollback procedures.

---