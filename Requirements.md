# PlanWise AI — Requirements Document

---

## Elevator Pitch

PlanWise AI is an AI-powered event planning web application that helps small groups of friends and families generate creative ideas for gatherings. Users describe their event in natural language, and the system uses vector-based semantic search to recommend personalized event plans. It exists to eliminate the stress of “we don’t know what to do” and turn vague ideas into structured, actionable plans.

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
- As a user, I can expand an event plan to generate a schedule and shopping checklist using AI.
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

- AI must be part of the core functionality (semantic event recommendation).
- Vector search implemented using Azure AI Search.
- Embeddings generated using Azure OpenAI.
- Clean backend architecture using Controllers, Services, DTOs, and dependency injection.
- Frontend built using Vue components and Vuetify UI framework.
- Canonical event data stored in Azure SQL and indexed in Azure AI Search with embedding vectors.
- Unit tests covering core backend services and frontend components.
- Secure configuration using environment variables and Azure settings.
- Full deployment to Azure with a working CI/CD pipeline.
- Polished and user-friendly UI/UX.
