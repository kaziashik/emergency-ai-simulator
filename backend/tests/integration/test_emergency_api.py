"""API tests for POST /emergency/analyze."""


def test_analyze_high_severity_one_blocked_exit(client, sample_emergency):
    response = client.post("/emergency/analyze", json=sample_emergency)
    assert response.status_code == 200
    data = response.json()

    assert data["status"] == "ok"
    assert data["request"]["emergency_id"] == "EMG001"
    assert data["risk_assessment"]["risk_level"] in {"LOW", "MEDIUM", "HIGH"}
    assert data["recommendation"]["recommended_exit"] == "Exit A"
    assert data["recommendation"]["human_approval_required"] is True


def test_analyze_low_severity(client):
    payload = {
        "emergency_id": "EMG002",
        "fire_location": "Room 1",
        "fire_severity": 2,
        "total_occupants": 4,
        "available_exits": ["Exit A", "Exit B"],
        "blocked_exits": [],
    }
    response = client.post("/emergency/analyze", json=payload)
    assert response.status_code == 200
    data = response.json()
    assert data["risk_assessment"]["risk_level"] == "LOW"
    assert data["recommendation"]["recommended_exit"] in {"Exit A", "Exit B"}


def test_analyze_both_exits_blocked(client):
    payload = {
        "emergency_id": "EMG003",
        "fire_location": "Corridor",
        "fire_severity": 7,
        "total_occupants": 12,
        "available_exits": [],
        "blocked_exits": ["Exit A", "Exit B"],
    }
    response = client.post("/emergency/analyze", json=payload)
    assert response.status_code == 200
    data = response.json()
    assert data["risk_assessment"]["risk_level"] == "HIGH"
    assert data["recommendation"]["recommended_exit"] is None
    assert data["recommendation"]["human_approval_required"] is True


def test_analyze_no_available_exits(client):
    payload = {
        "emergency_id": "EMG004",
        "fire_location": "Lobby",
        "fire_severity": 5,
        "total_occupants": 6,
        "available_exits": [],
        "blocked_exits": [],
    }
    response = client.post("/emergency/analyze", json=payload)
    assert response.status_code == 200
    assert response.json()["recommendation"]["recommended_exit"] is None


def test_invalid_severity_rejected(client):
    payload = {
        "emergency_id": "EMG005",
        "fire_location": "Room 2",
        "fire_severity": 99,
        "total_occupants": 10,
        "available_exits": ["Exit A"],
        "blocked_exits": [],
    }
    response = client.post("/emergency/analyze", json=payload)
    assert response.status_code == 422


def test_blocked_exit_overrides_available(client):
    payload = {
        "emergency_id": "EMG006",
        "fire_location": "Room 3",
        "fire_severity": 4,
        "total_occupants": 5,
        "available_exits": ["Exit A", "Exit B"],
        "blocked_exits": ["Exit A"],
    }
    response = client.post("/emergency/analyze", json=payload)
    assert response.status_code == 200
    data = response.json()
    assert data["recommendation"]["recommended_exit"] == "Exit B"


def test_missing_required_fields_rejected(client):
    response = client.post("/emergency/analyze", json={"emergency_id": "EMG007"})
    assert response.status_code == 422
