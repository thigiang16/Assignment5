# PlanWise AI — Requirements Document

---

## Elevator Pitch

PlanWise AI is an AI-powered event planning web application that helps small groups of friends and families generate creative ideas for gatherings. Users describe their event in natural language, and the system uses vector-based semantic search to recommend personalized event plans. The goal is to eliminate the stress of “we don’t know what to do” and turn vague ideas into structured, actionable plans.

---

## Target Audience

- Families planning birthdays or small celebrations  
- Friends organizing casual get-togethers  
- College students planning small events  
- Anyone who struggles to come up with creative event ideas  

---

## Use Cases (User Stories)

- As a user, I can register, log in, and log out.
- As a user, I can describe my event in natural language (e.g., “Small indoor birthday for 6 people, low budget.”).
- As a user, I receive a ranked list of event plans based on my description.
- As a user, I can view full event plan details (theme, food ideas, activities, decorations, budget level).
- As a user, I can save event ideas to favorites.
- As a user, I can expand an event plan to generate a detailed schedule and shopping checklist using AI.
- As an admin, I can create, edit, and delete event templates.

---
## Tech Stack

### Frontend
- Vue 3
- TypeScript
- Vuetify
- Axios

### Backend
- ASP.NET Core Web API
- Controllers, Services, DTOs
- Entity Framework Core

### Database
- Azure SQL Database

### AI & Search
- Azure OpenAI (Embeddings + Text Generation)
- Azure AI Search (Vector Search enabled)

### Cloud & Deployment
- Azure App Service
- Azure Static Web Apps
- GitHub Actions CI/CD

---
## Technical Requirements

### Functional Requirements
- Users must be able to create accounts and securely log in.
- Users must be able to describe an event using natural language.
- The system must return event recommendations ranked by relevance.
- Users must be able to view full event details including activities, food ideas, decorations, and budget level.
- Users must be able to save event plans to a favorites list.
- The system must allow users to expand an event plan into a detailed schedule and shopping checklist using AI-generated suggestions.
- Administrators must be able to manage event templates.

### System Requirements
- AI must be part of the core functionality, providing semantic event recommendations based on user descriptions.
- Vector search must be implemented using Azure AI Search to enable similarity-based event plan retrieval.
- Embeddings for event descriptions and templates must be generated using Azure OpenAI.
- The backend must follow a clean architecture using Controllers, Services, DTOs, and dependency injection.
- The frontend must be built using Vue components with the Vuetify UI framework.
- Canonical event template data must be stored in Azure SQL Database and indexed in Azure AI Search with embedding vectors.
- Unit tests must be implemented to cover core backend services and important frontend components.
- Application configuration must be secured using environment variables and Azure configuration settings.
- The system must be fully deployed to Azure with a functioning CI/CD pipeline.
- The application must provide a polished and user-friendly UI/UX.
