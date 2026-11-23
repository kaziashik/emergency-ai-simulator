# Unity Setup Guide (Phase 3)

Scripts for the 3D simulation and Emergency Control Centre dashboard are in
`unity/Assets/Scripts` using a modular feature layout
(`Core/`, `Modules/`, `Networking/`). A real Unity project must still be
created with Unity Hub.

> Use this repo path: `D:\EmergencyAI_Simulator\unity`  
> (Your notes mention `E:\...` — only change that if your project lives there.)

## Step A — Create the Unity project (manual)

1. Open **Unity Hub**.
2. Click **New project**.
3. Template: **3D (Core)** / **3D**.
4. Project name: `EmergencyAISimulation` (or keep folder name `unity`).
5. Location: parent folder `D:\EmergencyAI_Simulator`, so Assets land in
   `D:\EmergencyAI_Simulator\unity\Assets`.
6. If Unity creates a nested folder, move/copy our existing `Assets/Scripts`
   into the Unity project's `Assets` folder so this structure remains:

```
Assets/
  Scenes/
  Scripts/
    Core/
    Environment/
    People/
    UI/
    Networking/
    Agents/
  Prefabs/
  Materials/
```

## Step B — Open the scene

1. Open the default SampleScene (or create `Assets/Scenes/EmergencyBuilding.unity`).
2. Press **Play**.

`SceneEntryPoint` auto-creates `SimulationManager`, which builds:

- Floor, walls, 3 rooms, corridor
- Exit A (open) and Exit B (blocked)
- Fire hazard in Room 2
- 10 capsule NPCs
- Right-side **Emergency Control Centre** dashboard

## Step C — Optional NavMesh bake (better pathfinding)

Movement works with a direct-move fallback. For NavMesh:

1. Window → AI → Navigation (or AI Navigation package).
2. Select the Floor object.
3. Mark it as **Navigation Static**.
4. Bake the NavMesh.
5. Press Play again — NPCs use `NavMeshAgent` when on the mesh.

## Step D — Full workflow (Unity + FastAPI)

1. Start the backend:
   ```powershell
   cd D:\EmergencyAI_Simulator\backend
   .\.venv\Scripts\Activate.ps1
   uvicorn app.main:app --reload --host 127.0.0.1 --port 8000
   ```
2. In Unity, press **Play**.
3. Dashboard buttons:
   1. **Start Simulation**
   2. **Trigger Emergency**
   3. **Request AI Analysis** → POSTs to `/emergency/analyze` and shows **real** FastAPI values
   4. **Approve** → records decision, then starts evacuation toward the recommended exit
   5. **Reject** → records decision, does **not** start evacuation

Default API URL: `http://127.0.0.1:8000` (change on `SimulationManager` / `EmergencyAPIClient` if needed).

Approval is recorded separately from execution. No LLM controls the sim.

## Status

| Step | Status |
| --- | --- |
| 10 Create Unity project via Hub | Manual — you must do this |
| 11–16 Unity simulation + dashboard | Scripts ready |
| 17 EmergencyAPIClient | Ready |
| 18 Display FastAPI response | Ready |

Complete end-to-end demo still needs you to run Unity Hub project + backend together (T023).
