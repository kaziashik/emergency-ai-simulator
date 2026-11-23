# SYSTEM ARCHITECTURE

## Style

Modular monolith:
- One Python FastAPI backend
- One Unity frontend
- Feature modules with clear responsibilities
- No microservices and no database for the MVP

## Main Components

### 1. Unity Simulation (`unity/`)
Responsible for:
- Building environment
- Fire hazards
- Simulated people / evacuation
- Operator dashboard
- HTTP client to FastAPI

Script layout:
```
Assets/Scripts/
  Core/                 # GameManager, SimulationManager
  Modules/
    Environment/        # Building, fire, exits
    Evacuation/         # NPCs and movement
    Emergency/          # Recommendation / decision models
    Dashboard/          # UI controllers / views / models
  Networking/           # APIClient, endpoints, DTOs
  Shared/
```

### 2. Python Backend (`backend/app/`)
Responsible for:
- Receiving simulation data
- Validating requests
- Coordinating agents
- Returning recommendations

Layout:
```
app/
  core/                 # config, exceptions
  api/                  # top-level router, dependencies
  middlewares/          # CORS, etc.
  modules/
    emergency/          # routes → controller → service
    risk_assessment/    # agent + schemas + service
    evacuation/         # agent + schemas + service
    coordination/       # agent + schemas + service
  integrations/         # LLM / external systems (when needed)
```

### 3. AI Agents

Risk Assessment Agent → analyses emergency conditions.  
Evacuation Planning Agent → evaluates exits.  
Coordination Agent → combines outputs into one recommendation.

### 4. Human Operator
Reviews AI recommendations and approves or rejects suggested actions.

## Communication

Unity communicates with FastAPI through HTTP JSON
(`POST /emergency/analyze`).

## Workflow

1. Unity collects emergency information.
2. Unity sends information to FastAPI.
3. Backend validates the information.
4. Agents analyse the emergency.
5. Backend returns recommendations.
6. Unity displays recommendations.
7. Human operator reviews the results.
8. Approved actions are applied to the simulation.

## Request path (example)

`routes.py` → `schemas.py` / `validators.py` → `controller.py` →
`service.py` → risk / evacuation / coordination agents → response schemas
