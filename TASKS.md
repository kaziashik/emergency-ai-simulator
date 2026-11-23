# DEVELOPMENT TASKS

## Phase 1: Project Setup
- [ ] T001: Create Unity project. *(manual — open Unity Hub; see unity/UNITY_SETUP.md)*
- [x] T002: Create basic building scene. *(BuildingBootstrap scripts ready)*
- [x] T003: Add floor and walls.
- [x] T004: Add emergency exits.
- [x] T005: Add fire hazard object.

## Phase 2: Simulation
- [x] T006: Create NPC prefab. *(runtime capsules + EvacueeController)*
- [x] T007: Implement NPC movement.
- [ ] T008: Configure NavMesh. *(manual bake in Editor; fallback movement works)*
- [x] T009: Implement basic evacuation.
- [x] T010: Display NPC status.

## Phase 3: Backend
- [x] T011: Initialize FastAPI project.
- [x] T012: Create emergency data model.
- [x] T013: Create emergency status endpoint.
- [x] T014: Test API using Swagger UI. *(also verified via live GET/POST + pytest)*

## Phase 4: AI
- [x] T015: Implement risk assessment agent.
- [x] T016: Implement evacuation planning agent.
- [x] T017: Implement coordination workflow.
- [x] T018: Return structured recommendations.

## Phase 5: Integration
- [x] T019: Connect Unity to FastAPI. *(EmergencyAPIClient.cs)*
- [x] T020: Send emergency information.
- [x] T021: Display AI recommendations. *(dashboard shows FastAPI values)*
- [x] T022: Implement human approval. *(Approve/Reject + history)*
- [ ] T023: Test complete workflow.

## Phase 6: Documentation
- [ ] T024: Write README.
- [ ] T025: Capture screenshots.
- [ ] T026: Record demonstration.
- [ ] T027: Document limitations and evaluation.
