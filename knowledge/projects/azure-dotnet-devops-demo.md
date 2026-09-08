---
title: Azure .NET DevOps Platform

organization: School Project

role: Backend Developer

environment: development

period:
  from: 2026-07
  to: 2026-07

status: completed

technologies:
  - aspnet-core
  - csharp
  - dotnet
  - azure
  - container-registry
  - docker
  - github-actions
  - linux
  - terraform

concepts:
  - api-design
  - cloud-infrastructure
  - infrastructure-as-code
  - ci-cd
  - containerization
  - deployment-automation
  - devops
  - security
  - managed-identity

dependencies:

links:
  github:
  live:

---

# Overview

An ASP.NET Core Web API deployed to Microsoft Azure through an automated infrastructure and CI/CD workflow.

The project combines Terraform, Docker, Azure Container Registry, Azure Virtual Machines, and GitHub Actions to demonstrate how application infrastructure and deployments can be defined, version controlled, and reproduced through code.

The main objective was to gain practical experience with cloud infrastructure, containerized deployments, infrastructure as code, CI/CD automation, and secure cloud authentication.

---

# Context

The project was created as a school project to explore the operational side of backend development beyond writing and running an API locally.

The application needed to be deployed to Azure using reproducible infrastructure rather than manually configuring resources through the Azure Portal. The deployment process also needed to minimize manual steps and avoid storing long-lived Azure client secrets in GitHub.

Important requirements included:

- Reproducible Azure infrastructure.
- Version-controlled infrastructure configuration.
- Containerized application deployment.
- Automated application updates.
- Secure CI/CD authentication.
- No long-lived Azure client secret stored in GitHub.
- A deployment workflow that could be reused for future application changes.

The project was intentionally kept relatively small so the focus could remain on the deployment architecture and DevOps workflow rather than application complexity.

---

# Task

My responsibility was implementing the backend application and the complete cloud deployment workflow.

I owned:

- The ASP.NET Core Web API.
- Docker containerization.
- Azure infrastructure definition using Terraform.
- Azure Container Registry configuration.
- Azure Virtual Machine deployment.
- GitHub Actions CI/CD workflows.
- GitHub Actions authentication to Azure using OpenID Connect.
- SSH-based access from GitHub Actions to the Azure Virtual Machine.
- User Assigned Managed Identity configuration for Azure Container Registry access.
- Automated container deployment and updates on the Azure Virtual Machine.

The expected outcome was a reproducible deployment process where both the infrastructure and application deployment could be managed primarily through code and automation.

---

# Challenge

## Challenge: Building a Secure Automated Deployment Pipeline

### Problem

Deploying a containerized application to Azure involves several independent steps that must work together correctly:

- Infrastructure must exist before the application can be deployed.
- Container images must be built and made available to the target environment.
- The CI/CD workflow needs authenticated access to Azure.
- The GitHub Actions runner needs secure access to the deployment Virtual Machine.
- The target Virtual Machine needs authenticated access to the container registry.
- Application updates need to replace the running container reliably.

Doing these steps manually would make deployments harder to reproduce and increase the likelihood of configuration mistakes.

There was also a security requirement: the CI/CD pipeline should not depend on long-lived Azure client secrets stored in the GitHub repository.

### Solution

The deployment workflow was divided into infrastructure provisioning, image distribution, and application deployment.

Terraform defines the Azure infrastructure, including the resource group, networking, Linux Virtual Machine, public IP, Azure Container Registry, User Assigned Managed Identity, and the required role assignment.

Docker packages the ASP.NET Core API into a reproducible application image.

GitHub Actions handles the deployment workflow. The workflow:

1. Authenticates to Azure using OpenID Connect.
2. Builds the Docker image.
3. Pushes the image to Azure Container Registry.
4. Connects to the Azure Virtual Machine through SSH using a repository secret containing the VM's private SSH key.
5. Authenticates the VM to Azure using its User Assigned Managed Identity.
6. Authenticates Docker to Azure Container Registry using the VM's managed identity and `AcrPull` permissions.
7. Retrieves the new container image.
8. Updates the running application through Docker Compose.

OpenID Connect was used for the GitHub-to-Azure trust relationship so the workflow did not require a long-lived Azure client secret stored in the repository.

The SSH private key was used separately for GitHub Actions to establish the connection to the Azure Virtual Machine.

The Virtual Machine uses a User Assigned Managed Identity with the `AcrPull` role, allowing the VM to authenticate to Azure Container Registry without storing registry credentials on the VM.

### Result

The deployment process became repeatable and largely automated. Application changes can be built, published, and deployed through the CI/CD workflow without manually provisioning or configuring the Azure environment for each deployment.

---

# Action

## Architecture

### Backend

The backend is an ASP.NET Core Web API written in C# and packaged as a Docker container.

The API is responsible for:

- HTTP API endpoints.
- Application and business logic.
- Dependency injection.
- Configuration.
- Containerized application execution.

The application is intentionally small because the primary purpose of the project is demonstrating the deployment and infrastructure workflow.

### Database

The project does not use a persistent database.

The deployment architecture does not depend on a database-specific service, leaving the infrastructure workflow open for a future persistent storage component.

### Infrastructure

Azure hosts the deployed application and supporting infrastructure.

Terraform provisions:

- Resource Group.
- Virtual Network.
- Subnet.
- Network Security Group.
- Public IP.
- Linux Virtual Machine.
- Azure Container Registry.
- User Assigned Managed Identity.
- `AcrPull` role assignment.

The application runs as a Docker container on the Linux Virtual Machine.

GitHub Actions provides the CI/CD layer. It authenticates to Azure using OpenID Connect, builds and publishes the container image, connects to the Virtual Machine through SSH, and triggers the application update.

The Virtual Machine uses its User Assigned Managed Identity to authenticate to Azure Container Registry and retrieve the container image.

---

## Technical Decisions

### Decision: Infrastructure as Code with Terraform

#### Context

The Azure environment needed to be reproducible and version controlled.

Manually creating resources through the Azure Portal would make the infrastructure dependent on manual configuration and increase the risk of configuration drift.

#### Chosen Solution

Terraform was used to define the Azure infrastructure as code.

The infrastructure configuration describes the resources, networking, identity, and permissions required to run the application.

This allows the environment to be recreated from the repository rather than relying on undocumented manual configuration.

#### Alternatives Considered

- Azure Portal.
- Azure CLI scripts.

The Azure Portal was not chosen because manual provisioning is less reproducible. CLI scripting could automate provisioning, but Terraform provides a declarative infrastructure model and explicit resource state.

#### Trade-offs

**Advantages:**

- Reproducible infrastructure.
- Version-controlled configuration.
- Reduced manual provisioning.
- Easier environment recreation.
- Clear infrastructure dependencies.

**Disadvantages:**

- Additional tooling and configuration.
- Terraform state must be managed correctly.
- Infrastructure changes require understanding both Azure and Terraform.

---

### Decision: OpenID Connect for CI/CD Authentication

#### Context

GitHub Actions needs permission to interact with Azure during deployment.

Storing a long-lived Azure client secret in GitHub would create an unnecessary credential-management and security risk.

#### Chosen Solution

GitHub Actions authenticates to Azure using OpenID Connect.

The workflow grants the GitHub Actions job permission to request an OIDC token and uses the Azure Login action with the Azure client ID, tenant ID, and subscription ID.

This establishes a federated trust relationship between the GitHub repository and the Azure identity without requiring long-lived Azure client secrets to be stored in GitHub.

#### Alternatives Considered

- Azure client secret authentication.
- Publish profiles.

These approaches were not chosen because they rely on longer-lived credentials that require additional secret storage and rotation.

#### Trade-offs

**Advantages:**

- No long-lived Azure client secret in GitHub.
- Reduced credential-management overhead.
- Federated authentication between GitHub and Azure.
- Better separation between GitHub and Azure identity management.

**Disadvantages:**

- More initial identity configuration.
- Requires understanding of workload identity federation.
- Azure and GitHub workflow configuration must match correctly.

---

### Decision: User Assigned Managed Identity for Container Registry Access

#### Context

The Azure Virtual Machine needs permission to retrieve container images from Azure Container Registry.

Storing registry credentials directly on the Virtual Machine would introduce another credential-management problem.

#### Chosen Solution

A User Assigned Managed Identity was provisioned with Terraform and associated with the Virtual Machine.

The identity receives the `AcrPull` role on the Azure Container Registry.

During deployment, the VM authenticates to Azure using:

`az login --identity`

It then uses:

`az acr login`

to authenticate Docker to the Azure Container Registry.

This allows the VM to access the registry using Azure-managed identity rather than manually stored registry credentials.

#### Alternatives Considered

- Static container registry credentials.
- Access tokens stored on the Virtual Machine.

These alternatives would introduce credentials that need to be stored, protected, rotated, and potentially revoked manually.

#### Trade-offs

**Advantages:**

- No manually managed registry password on the VM.
- Azure-managed identity lifecycle.
- Explicit role-based access.
- Clear separation between application deployment and registry credentials.

**Disadvantages:**

- Additional Azure identity configuration.
- Requires correct role assignment and identity configuration.
- Registry authentication from the VM still needs to be integrated correctly into the deployment workflow.

---

### Decision: Docker-Based Application Deployment

#### Context

The application needed a consistent runtime environment between development and the Azure deployment target.

#### Chosen Solution

The ASP.NET Core API is packaged as a Docker image and deployed to the Linux Virtual Machine.

Docker Compose is used on the VM to manage the running application container and simplify application updates.

#### Alternatives Considered

- Deploying the ASP.NET Core application directly to the VM.
- Using Azure App Service or another managed container platform.

Direct deployment was avoided because it would couple the VM more closely to the application runtime environment. Managed Azure container services were considered outside the scope of this project, which focused on understanding the underlying infrastructure and deployment process.

#### Trade-offs

**Advantages:**

- Consistent application runtime.
- Reproducible deployment artifact.
- Clear separation between application and host environment.
- Simple container replacement workflow.

**Disadvantages:**

- The VM and Docker runtime remain operational responsibilities.
- Container orchestration is limited compared with managed container platforms.
- High availability and automatic scaling are not provided by this architecture.

---

## Implementation

### Features

- ASP.NET Core Web API.
- Dockerized application runtime.
- Automated Azure infrastructure provisioning.
- Azure Container Registry integration.
- Linux Virtual Machine deployment.
- Automated container image publishing.
- Automated application deployment.
- OIDC-based GitHub-to-Azure authentication.
- SSH-based GitHub Actions access to the deployment VM.
- Managed Identity-based registry access.
- Docker Compose-based container updates.

### APIs

The project contains an ASP.NET Core Web API.

The API provides the application runtime that is packaged into the Docker image and deployed to the Azure Virtual Machine.

API functionality was intentionally kept secondary to the infrastructure and deployment objectives of the project.

### Data and Persistence

No persistent database was implemented.

The API therefore does not require a database service as part of the deployment pipeline.

### Automation

GitHub Actions automates the application deployment workflow.

The automation includes:

- Azure authentication through OpenID Connect.
- Docker image building.
- Container image publishing to Azure Container Registry.
- SSH-based connection to the deployment VM using a GitHub repository secret.
- VM authentication to Azure using User Assigned Managed Identity.
- Container registry authentication from the VM.
- Container image retrieval.
- Docker Compose application update.

Terraform provides separate infrastructure automation for provisioning the Azure environment.

### Testing

Testing focused primarily on validating the deployment workflow and infrastructure configuration.

The important validation areas were:

- Successful OIDC-based Azure authentication from GitHub Actions.
- Successful Docker image creation.
- Successful image publication to Azure Container Registry.
- Successful SSH connection from GitHub Actions to the Virtual Machine.
- Successful Managed Identity authentication from the Virtual Machine.
- Successful registry access from the Virtual Machine.
- Successful container deployment.
- Successful application startup after deployment.

No automated end-to-end test suite was implemented as part of this project.

---

# Result

The project resulted in a complete demonstration of an automated ASP.NET Core deployment workflow on Azure.

The final solution provides:

- Infrastructure defined through Terraform.
- Containerized application deployment.
- Azure Container Registry-based image distribution.
- GitHub Actions CI/CD automation.
- OpenID Connect authentication for CI/CD between GitHub Actions and Azure.
- SSH-based deployment access to the Azure Virtual Machine.
- Managed Identity-based registry access from the Virtual Machine.
- Automated application updates on an Azure Linux Virtual Machine.

The project achieved its primary goal of replacing a manually configured deployment process with a reproducible, version-controlled workflow.

---

# Lessons Learned

## Lesson: Infrastructure Is Part of the Application

The project reinforced that deploying a backend application is not separate from engineering the application itself.

Networking, identity, permissions, compute resources, containers, and deployment workflows all affect whether the application can actually run reliably.

This changed my approach toward treating infrastructure configuration as part of the application's source-controlled engineering process rather than as an external operational task.

## Lesson: Authentication Architecture Matters in CI/CD

Using OpenID Connect instead of long-lived Azure client secrets demonstrated the difference between simply making a deployment work and designing a deployment workflow with a better security model.

The main lesson was to avoid introducing persistent Azure credentials when the platform already provides federated workload identity mechanisms.

The project also showed that different parts of a deployment can require different authentication mechanisms. GitHub Actions used OIDC to authenticate to Azure, SSH was used to access the VM, and the VM used Managed Identity to access Azure Container Registry.

## Lesson: Managed Identity Reduces Credential Management

Using a User Assigned Managed Identity for registry access showed how Azure-native identity mechanisms can remove registry credentials from the VM deployment process.

This makes permissions more explicit and reduces the number of secrets that need to be created, stored, rotated, and protected.

## Lesson: Managed Services Would Be Preferable at Larger Scale

The VM-based deployment was useful for learning how infrastructure, containers, networking, identity, and deployment automation fit together.

However, I would not automatically choose this architecture for a production application.

A managed container platform such as Azure Container Apps or Azure App Service would remove significant operational responsibility around VM maintenance, scaling, availability, and deployment management.

The project therefore clarified an important distinction between **learning the underlying infrastructure** and **choosing the most operationally efficient architecture for production**.

---

# Future Improvements

- Introduce automated integration tests that must pass before deployment.
- Add separate development, staging, and production environments.
- Add deployment health checks and automated rollback.
- Add Azure Monitor and Application Insights.
- Add centralized logging and deployment diagnostics.
- Introduce infrastructure validation and Terraform plan checks in CI.
- Move the application to Azure Container Apps or App Service to reduce VM operational overhead.
- Add persistent storage if application functionality requires it.
- Add deployment approval gates for production environments.
- Add stronger post-deployment verification before marking a deployment successful.

---