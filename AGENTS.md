# AGENTS.md

# Project: EmergencyAI Simulator

## 1. Project Overview

Build a small interactive emergency simulation using
Unity, C#, Python, FastAPI, and AI agents.

The system simulates a building emergency and provides
AI-assisted evacuation recommendations to a human operator.

This is an academic research prototype, not a validated
real-world emergency response system.

## 2. Technology Stack

- Unity: 3D simulation
- C#: Unity scripting
- Python: AI backend
- FastAPI: REST API
- Pydantic: Data validation
- OpenAI API: Optional LLM reasoning
- Git: Version control

## 3. Development Rules

- Work on one small task at a time.
- Do not implement future features without permission.
- Do not rewrite working files unnecessarily.
- Keep code simple and modular.
- Avoid unnecessary dependencies.
- Use meaningful variable and function names.
- Explain every important implementation.
- Never invent test results.
- Do not claim a feature works until tested.

## 4. Unity Rules

- Use C# for Unity scripts.
- Separate environment, people, UI, and networking.
- Use Unity components appropriately.
- Do not put all logic in one script.
- Use NavMesh for basic navigation when appropriate.

## 5. Backend Rules

- Use Python and FastAPI.
- Use Pydantic models for API validation.
- Keep API routes separate from agent logic.
- Keep secrets in environment variables.
- Never commit API keys.

## 6. AI Agent Rules

- Start with deterministic rule-based logic.
- Introduce LLM-based reasoning incrementally.
- Keep agents' responsibilities separate.
- Validate AI-generated actions before execution.
- Require human approval for consequential actions.
- Never allow an LLM to directly control the simulation.

## 7. Testing Rules

For every task:

1. Explain the proposed change.
2. Implement only the requested task.
3. Identify modified files.
4. Provide testing instructions.
5. Explain expected results.
6. Report limitations and unfinished work.

## 8. Documentation

Update README.md and relevant documentation
when functionality changes.

## 9. Important Restrictions

Do not introduce reinforcement learning,
complex multi-agent frameworks, databases,
or unnecessary infrastructure during the
initial MVP unless explicitly requested.

## 10. Architecture and Code Organization

Use a modular, feature-based architecture (modular monolith).
Do not create every target folder/file up front — add them
when the feature is implemented.

### Backend
- Use Python and FastAPI.
- Organize application functionality inside `app/modules/`.
- Keep routes, controllers, services, agents, schemas,
  interfaces, and validators separate where appropriate.
- Keep external integrations inside `app/integrations/`.
- Store LLM prompt templates in dedicated prompt files.
- Keep configuration inside `app/core/`.
- Avoid unnecessary abstractions and duplicate validation.
- Pydantic already validates fields — use `validators.py`
  only for extra domain rules.
- Add `interfaces.py` only when a real contract is needed.
- No database/repository layer for the MVP unless requested.

### Unity Frontend
- Use C# and Unity components.
- Organize scripts by feature under `Assets/Scripts/Modules/`.
- Separate controllers, views, models, and components
  when appropriate.
- Keep API communication inside `Networking/`.
- Avoid putting all simulation logic in one MonoBehaviour.

### Development Principles
- Implement one small feature at a time.
- Follow existing folder and naming conventions.
- Keep modules independent where practical.
- Do not create unnecessary files or folders.
- Explain the purpose of every new file.
- Write tests for important business logic.
- Update documentation when architecture changes.
- Do not modify unrelated modules without permission.
