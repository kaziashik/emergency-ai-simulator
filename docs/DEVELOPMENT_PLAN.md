# DEVELOPMENT PLAN

## How to use these files with coding agents

| Coding tool | Instruction file |
| --- | --- |
| OpenAI Codex | AGENTS.md |
| Claude Code | CLAUDE.md |
| Cursor | Project Rules / .cursor/rules/ |

You can use one common `AGENTS.md` as your master specification. For Claude Code, create a short `CLAUDE.md` that tells it to read `AGENTS.md` first. For Cursor, add the same instructions to its project rules if it doesn't automatically load the file.

## Small prompts to give your AI coding agent

Copy these prompts one by one. Do not give all of them at once.

---

### Phase 1 – Project setup

#### Day 1

##### Prompt 1: Understand the project

```
I am starting a project called EmergencyAI Simulator.

First, read:
- AGENTS.md
- docs/PROJECT_SPEC.md
- docs/ARCHITECTURE.md
- docs/AGENT_DESIGN.md
- TASKS.md

Do not write any code yet.

Explain:
1. What this project does.
2. Its main components.
3. How Unity and Python communicate.
4. What we need to build first.

Keep the explanation simple.
Wait for my approval before proceeding.
```

##### Prompt 2: Create the Unity project structure

```
We are starting T001 and T002.

Inspect the existing repository.

Help me set up the Unity 3D project and
organize the Assets folder.

Required folders:
- Assets/Scenes
- Assets/Scripts/Core
- Assets/Scripts/Environment
- Assets/Scripts/People
- Assets/Scripts/Agents
- Assets/Scripts/UI
- Assets/Scripts/Networking
- Assets/Prefabs
- Assets/Materials

Do not generate simulation logic yet.

Tell me which steps I must perform
inside Unity Editor manually.

Do not modify Unity-generated project files
unnecessarily.
```

##### Prompt 3: Create the building

```
Implement T002, T003 and T004.

I need a simple 3D emergency building.

Requirements:
- One floor.
- Three rooms.
- Connecting corridors.
- Two emergency exits.
- A visible floor and walls.

Use simple Unity primitives.
Keep the building small and easy to navigate.

Create a C# script only if necessary.

Explain how to run and inspect the scene.

Do not implement NPCs or fire yet.
```

##### Prompt 4: Add fire hazard

```
Implement T005 only.

Create a simple fire hazard object
using Unity primitives.

Requirements:
- Make the fire visually identifiable.
- Add a FireHazard component.
- Include a configurable severity value.
- Make the hazard location accessible
  to other scripts.

Do not implement real fire physics.

Explain the C# script line by line.
```

---

### Phase 2 – People and evacuation

#### Day 2

##### Prompt 5: Create NPC

```
Implement T006.

Create a simple NPC prefab for the building.

Requirements:
- Use a capsule or simple 3D model.
- Add a NavMeshAgent component.
- Add an EvacueeController C# script.
- Give each NPC an evacuation status.
- Expose the NPC position.

Keep the implementation simple.

Do not implement AI decision-making yet.

Tell me how to create and test the prefab.
```

##### Prompt 6: Implement evacuation movement

```
Implement T007, T008 and T009.

Make the NPC move towards a designated
emergency exit using Unity NavMesh.

Requirements:
- NPC must follow a navigable path.
- NPC must stop at the exit.
- Update its status to Evacuated.
- Handle unreachable destinations safely.

Use Unity's supported NavMesh workflow
for the installed Unity version.

Explain how to bake the navigation mesh
and test the movement.

Do not add LLMs or external APIs.
```

##### Prompt 7: Display NPC status

```
Implement T010.

Display basic simulation information.

Show:
- Total occupants.
- Evacuated occupants.
- Remaining occupants.
- Current emergency status.

Use a simple Unity UI.

Do not redesign the entire interface.

Explain how to test the counters.
```

---

### Phase 3 – Python backend

#### Day 3

##### Prompt 8: Initialize FastAPI

```
Implement T011.

Set up a Python FastAPI backend
inside the existing backend folder.

Requirements:
- Use a virtual environment.
- Create requirements.txt.
- Create app/main.py.
- Create app/api/.
- Create app/models/.
- Create app/agents/.
- Create app/services/.
- Create tests/.

Add a basic health endpoint:
GET /health

Return:
{
  "status": "healthy",
  "service": "EmergencyAI"
}

Do not implement AI yet.

Give me Windows PowerShell commands
to install and run the backend.
```

##### Prompt 9: Create emergency data model

```
Implement T012.

Create a Pydantic model representing
the simulated emergency.

Fields:
- emergency_id
- fire_location
- fire_severity
- total_occupants
- available_exits
- blocked_exits

Add validation for the fields.

Use realistic example data.

Do not add LLM integration.

Explain the purpose of each field.
```

##### Prompt 10: Create emergency endpoint

```
Implement T013 and T014.

Create:
POST /emergency/analyze

The endpoint should:
1. Receive emergency information.
2. Validate the request.
3. Return the received data.
4. Return a basic acknowledgement.

Add a simple API test.

I want to test it through FastAPI Swagger UI.

Do not add AI agents yet.
```

---

### Phase 4 – Agentic AI

#### Day 4

##### Prompt 11: Risk Assessment Agent

```
Implement T015.

Create a RiskAssessmentAgent in Python.

Input:
- Fire severity.
- Number of occupants.
- Available exits.
- Blocked exits.

For the first version, use deterministic
rule-based decision-making.

Output:
- Risk level: LOW, MEDIUM or HIGH.
- Identified hazards.
- Explanation.

Keep the agent independent from FastAPI routes.

Add unit tests for different scenarios.

Do not use an LLM yet.
```

##### Prompt 12: Evacuation Planning Agent

```
Implement T016.

Create an EvacuationPlanningAgent.

Input:
- Risk assessment.
- Available exits.
- Blocked exits.

Responsibilities:
- Identify usable exits.
- Avoid blocked exits.
- Recommend an available exit.
- Explain the recommendation.

Use simple deterministic logic.

Do not pretend to calculate
real-world fire or smoke propagation.

Add tests for multiple exit scenarios.
```

##### Prompt 13: Coordination Agent

```
Implement T017.

Create a CoordinationAgent.

It should:
1. Receive the risk assessment.
2. Receive the evacuation recommendation.
3. Combine both results.
4. Produce a structured final recommendation.

Return:
- Emergency severity.
- Recommended action.
- Recommended exit.
- Explanation.
- Human approval required: true.

Do not allow the agent to execute
consequential actions automatically.

Add tests.
```

##### Prompt 14: Introduce an LLM

```
Now introduce an optional LLM component.

Use an LLM to explain the structured
recommendations produced by the existing agents.

Requirements:
- Keep deterministic risk assessment.
- Keep deterministic exit selection.
- Do not allow the LLM to invent exits.
- Validate its output.
- Keep the system functional if the
  LLM API is unavailable.
- Store API keys in environment variables.

Do not rewrite the existing agents.

Explain which decisions are made by
rules and which are generated by the LLM.
```

---

### Phase 5 – Connect everything

#### Day 5

##### Prompt 15: Connect Unity and Python

```
Implement T019 and T020.

Connect the Unity simulation to the
FastAPI backend using HTTP and JSON.

Requirements:
- Unity collects current emergency data.
- Unity sends a POST request to
  /emergency/analyze.
- Python validates the request.
- Python returns the recommendation.
- Unity receives and parses the response.
- Handle API connection errors.

Use C# UnityWebRequest.

Do not implement human approval yet.

Provide instructions to test the connection.
```

##### Prompt 16: Display recommendations

```
Implement T021.

Display the AI recommendation in
the Unity user interface.

Show:
- Emergency risk level.
- Identified hazards.
- Recommended evacuation exit.
- Explanation.
- Human approval status.

Use a clear and simple interface.

Do not add unnecessary visual effects.
```

##### Prompt 17: Human approval

```
Implement T022.

Add two buttons:
- Approve Recommendation
- Reject Recommendation

Requirements:
- Display the recommendation before approval.
- Record the operator's decision.
- Show the approval status.
- Do not automatically execute
  consequential actions without approval.

Keep the implementation simple.

Add a test for both decisions.
```

##### Prompt 18: Complete integration testing

```
Implement T023.

Review the entire application.

Test this workflow:

1. Start Unity.
2. Start the FastAPI backend.
3. Run the building simulation.
4. Generate an emergency scenario.
5. Send emergency information to Python.
6. Execute the agents.
7. Receive recommendations.
8. Display recommendations in Unity.
9. Approve or reject the recommendation.

Identify bugs and missing functionality.

Do not rewrite working components.

Provide a final test report.
Clearly distinguish tested features
from features that remain incomplete.
```
