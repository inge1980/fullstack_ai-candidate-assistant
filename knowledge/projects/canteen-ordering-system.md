---
title: Canteen Management System with ERP

organization: Episteme AS

role: Fullstack Developer

environment: production

period:
  from: 2023-08
  to: 2024-06

status: completed

technologies:
  - php
  - csharp
  - dotnet
  - swagger
  - sql-server
  - rest-api
  - javascript
  - css
  - bootstrap
  - iis

concepts:
  - enterprise-software
  - erp
  - system-integration
  - inventory-management
  - food-ordering
  - multi-department
  - role-based-access
  - backend-development
  - api-design
  - data-consistency

dependencies:

links:
  github:
  live:

---

# Overview

A fullstack canteen management system built to support food ordering across workplace and school canteens, with integration to an ERP system for inventory management.

The platform supported multiple departments, allowing department managers to manage users and place orders for their departments, while individual users could place personal food orders through a streamlined interface.

---

# Context

The project was designed for canteens in both workplaces and schools, where food ordering needed to support different organizational structures and user roles.

The system addressed the need for a single canteen ordering solution that could serve multiple departments while connecting orders to an ERP system for more efficient inventory management.

The system operated against the live database, meaning testing had to account for real users, recipes, and production data. Test orders were explicitly marked to distinguish them from actual orders.

Different user roles required separate ordering workflows, making role-specific functionality a key requirement.

---

# Task

I developed the fullstack solution, including multi-department support and ERP integration.

My responsibilities included implementing user management, department ordering, individual food ordering, and functionality required to safely test ordering workflows against the live environment.

---

# Challenge

## Challenge: Supporting Different Ordering Workflows

### Problem

The system needed to support multiple departments with different ordering requirements. Department managers needed administrative capabilities for managing users and placing orders for their entire department, while individual users needed a simpler ordering experience.

### Solution

I implemented separate ordering workflows tailored to each role. Department managers could add users and manage department-wide orders, while individual users interacted with a streamlined interface focused on personal orders.

### Result

The system supported both department-wide and individual ordering within a single application.

---

## Challenge: ERP Integration

### Problem

Canteen orders needed to integrate with an ERP system to support inventory management.

### Solution

I integrated the ordering workflow with the ERP system so department orders became part of the inventory management process.

### Result

The integration connected food ordering with ERP-driven inventory management, creating a more efficient workflow.

---

## Challenge: Testing Against Live Data

### Problem

The application was tested against the live database rather than an isolated test database. This meant testing had to work with real usernames and recipes while avoiding confusion between test orders and actual orders.

### Solution

Test orders were explicitly marked with a test marker so they could be distinguished from real orders while using the same live users, recipes, and underlying data.

### Result

The ordering workflows could be tested using realistic production data and real user scenarios while maintaining a clear distinction between test orders and actual orders.

---

# Action

## Architecture

### Frontend

The application provided separate interfaces and ordering workflows for department managers and individual users.

Department managers could manage users and place orders for their departments, while individual users used a streamlined interface for personal food orders.

### Backend

The web application backend was built in PHP, while the REST API was developed in C# and .NET with Swagger for API documentation. The backend handled departments, users, orders, and the ERP integration.

### Database

SQL Server stored data related to departments, users, and food orders.

### File Storage

### Infrastructure

The application was hosted on IIS and operated in a production environment.

Testing was performed against the live environment using explicitly marked test orders to distinguish them from actual orders.

---

## Technical Decisions

### Decision: Separate Ordering Workflows

#### Context

The application needed to support department managers placing orders for entire departments while keeping personal ordering simple for individual users.

#### Chosen Solution

I implemented separate workflows tailored to each user role.

#### Alternatives Considered

Not documented.

#### Trade-offs

Separate workflows improved usability for each role but introduced additional complexity in handling different ordering processes.

---

### Decision: ERP Integration for Inventory Management

#### Context

Inventory management needed to reflect orders placed through the canteen system.

#### Chosen Solution

I integrated the ordering workflow with the ERP system so department orders became part of the inventory management process.

#### Alternatives Considered

Not documented.

#### Trade-offs

The integration improved inventory management but introduced a dependency on the ERP integration.

---

### Decision: Test Marking in the Live Environment

#### Context

The application needed to be tested with real users, recipes, and live database data rather than isolated test data.

#### Chosen Solution

Orders created during testing were explicitly marked as test orders, allowing them to be distinguished from actual orders while using the same live data.

#### Alternatives Considered

Not documented.

#### Trade-offs

Testing against live data provided realistic validation of the complete ordering workflow but required explicit identification of test orders to prevent them from being confused with real orders.

---

### Decision: Use a Shared Domain Model Across Different Canteen Types

#### Context

The system needed to support both workplace and school canteens without maintaining separate implementations for each environment.

#### Chosen Solution

I structured the application around departments, users, and role-based ordering workflows, allowing the same domain model to support different organizational structures.

#### Alternatives Considered

Not documented.

#### Trade-offs

Using a shared model reduced duplication and kept the application consistent across different canteen environments, but required the domain model to accommodate different organizational structures within a single implementation.

---

## Implementation

### Features

- Support for workplace and school canteens
- Multi-department support
- User management for department managers
- Department-wide food ordering
- Individual food ordering
- Streamlined ordering interface for individual users
- ERP integration for inventory management
- Test order marking for live-environment testing

### APIs

REST API built with C# and .NET, documented with Swagger, supporting the canteen application and ERP integration.

### Data and Persistence

The application managed data for departments, users, and food orders using SQL Server.

### Automation

### Testing

Testing was performed against the live environment using real usernames, recipes, and database data.

Test orders were explicitly marked to distinguish them from actual orders.

---

# Result

The project delivered a fullstack canteen management system that supported both workplace and school canteens, with multiple departments, department-wide ordering, and individual food ordering within a single platform.

The ERP integration connected food ordering with inventory management, while live-environment testing allowed the complete ordering workflow to be validated using real users, recipes, and production data.

---

# Lessons Learned

## Lesson: Designing Around User Roles

This project reinforced the importance of designing workflows around user responsibilities. Department managers and individual users had fundamentally different needs, making role-specific experiences essential without adding unnecessary complexity for everyday users.

## Lesson: Testing With Production Data Requires Explicit Separation

Testing directly against live data provided realistic validation of the complete ordering workflow, but also required a clear mechanism for distinguishing test activity from real activity.

Using explicit test markers made it possible to test with real users and recipes while maintaining a clear distinction between test orders and actual orders.

---

# Future Improvements

- Introduce a dedicated test environment and test database to reduce reliance on live data.
- Improve observability around ERP-integrated order flows.
- Extend department administration and order management functionality.

---