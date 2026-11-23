"""Shared pytest fixtures."""

import pytest
from fastapi.testclient import TestClient

from app.main import app
from app.modules.emergency.service import EmergencyAnalysisService


@pytest.fixture
def client() -> TestClient:
    return TestClient(app)


@pytest.fixture
def service() -> EmergencyAnalysisService:
    return EmergencyAnalysisService()


@pytest.fixture
def sample_emergency() -> dict:
    return {
        "emergency_id": "EMG001",
        "fire_location": "Room 2",
        "fire_severity": 8,
        "total_occupants": 10,
        "available_exits": ["Exit A", "Exit B"],
        "blocked_exits": ["Exit B"],
    }
