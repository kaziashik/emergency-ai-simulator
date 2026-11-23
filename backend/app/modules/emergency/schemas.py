"""Request/response schemas for the emergency analysis API."""

from typing import List, Literal

from pydantic import BaseModel, Field, field_validator

from app.modules.coordination.schemas import CoordinationResult
from app.modules.evacuation.schemas import EvacuationPlanResult
from app.modules.risk_assessment.schemas import RiskAssessmentResult


class EmergencyRequest(BaseModel):
    """Incoming emergency information from the Unity simulation."""

    emergency_id: str = Field(..., min_length=1, max_length=64, examples=["EMG001"])
    fire_location: str = Field(..., min_length=1, max_length=128, examples=["Room 2"])
    fire_severity: int = Field(..., ge=1, le=10, examples=[8])
    total_occupants: int = Field(..., ge=0, le=1000, examples=[10])
    available_exits: List[str] = Field(..., examples=[["Exit A", "Exit B"]])
    blocked_exits: List[str] = Field(default_factory=list, examples=[["Exit B"]])

    @field_validator("emergency_id", "fire_location")
    @classmethod
    def strip_nonempty(cls, value: str) -> str:
        cleaned = value.strip()
        if not cleaned:
            raise ValueError("must not be empty or whitespace")
        return cleaned

    @field_validator("available_exits", "blocked_exits")
    @classmethod
    def normalize_exit_names(cls, exits: List[str]) -> List[str]:
        normalized: List[str] = []
        for exit_name in exits:
            cleaned = exit_name.strip()
            if cleaned and cleaned not in normalized:
                normalized.append(cleaned)
        return normalized


class EmergencyAnalyzeResponse(BaseModel):
    """Full API response for POST /emergency/analyze."""

    status: Literal["ok"] = "ok"
    acknowledgement: str
    request: EmergencyRequest
    risk_assessment: RiskAssessmentResult
    evacuation_plan: EvacuationPlanResult
    recommendation: CoordinationResult
