# Testing Plan — EmergencyAI Simulator

Test early and often: components first, then integration, then full workflow.

| Stage | What to test | Expected result | How | Status |
| --- | --- | --- | --- | --- |
| **T1** | FastAPI health | API responds successfully | `GET /health` | **PASS** |
| **T2** | Emergency endpoint | Validates and returns data | `POST /emergency/analyze` | **PASS** |
| **T3** | Risk agent | Correct rule-based risk output | `tests/unit/test_agents.py` | **PASS** |
| **T4** | Planning agent | Avoids blocked exits | `tests/unit/test_agents.py` | **PASS** |
| **T5** | Coordination agent | Combines recommendations | `tests/unit/test_agents.py` | **PASS** |
| **T6** | Unity scene | Building renders correctly | Unity Play Mode | **Manual — pending** |
| **T7** | NPC movement | People reach exits | Approve → watch capsules | **Manual — pending** |
| **T8** | Unity dashboard | Displays live simulation values | Occupants / evacuated counters | **Manual — pending** |
| **T9** | API integration | Unity receives backend response | Request AI Analysis | **Manual — pending** |
| **T10** | Approval | Records approve/reject decision | Decision history panel | **Manual — pending** |
| **T11** | Full workflow | Complete end-to-end demonstration | Steps below | **Manual — pending** |

Last automated verification: after modular refactor — **14/14 pytest passed**; live `/health` and `/emergency/analyze` OK.

---

## Automated checks (T1–T5)

```powershell
cd D:\EmergencyAI_Simulator\backend
.\.venv\Scripts\Activate.ps1
python -m pytest -v
uvicorn app.main:app --reload --host 127.0.0.1 --port 8000
```

Live smoke:

```powershell
Invoke-RestMethod http://127.0.0.1:8000/health

$body = @{
  emergency_id = "EMG001"
  fire_location = "Room 2"
  fire_severity = 8
  total_occupants = 10
  available_exits = @("Exit A")
  blocked_exits = @("Exit B")
} | ConvertTo-Json

Invoke-RestMethod http://127.0.0.1:8000/emergency/analyze `
  -Method POST -Body $body -ContentType "application/json"
```

Expected analyze highlights:
- `risk_level`: **HIGH**
- `recommended_exit`: **Exit A**
- `human_approval_required`: **true**

Swagger: http://127.0.0.1:8000/docs

---

## Manual Unity checks (T6–T11)

Prerequisites:
1. Create Unity project (see `unity/UNITY_SETUP.md`)
2. Backend running on `http://127.0.0.1:8000`

### T6 — Scene
- [ ] Press Play
- [ ] Floor, walls, three rooms, corridor visible
- [ ] Exit A (green) and Exit B (red/blocked) visible
- [ ] Fire hazard in Room 2
- [ ] Blue capsule NPCs visible

### T7 — NPC movement
- [ ] Trigger Emergency → Request AI Analysis → **Approve**
- [ ] NPCs move toward Exit A
- [ ] Evacuated count increases

### T8 — Dashboard
- [ ] Occupants / Evacuated / Remaining update live
- [ ] Fire location / severity match scene
- [ ] Blocked exits shows Exit B

### T9 — API integration
- [ ] Request AI Analysis shows **source: FastAPI**
- [ ] Risk / exit / explanation match Swagger for same payload
- [ ] Backend stopped → connection error (not hardcoded values)

### T10 — Approval
- [ ] **Approve** → history shows Approved; evacuation starts
- [ ] **Reject** → history shows Rejected; no evacuation

### T11 — Full workflow
1. Start FastAPI  
2. Unity Play Mode  
3. Start Simulation → Trigger Emergency → Request AI Analysis  
4. Confirm FastAPI recommendation on dashboard  
5. Approve or Reject  
6. Confirm history + (on Approve) movement  

---

## Limitations

- T6–T11 cannot be marked PASS until run in Unity Editor.
- Research prototype only — not a validated emergency system.
