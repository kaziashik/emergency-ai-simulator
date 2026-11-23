"""Schemas for coordination outputs."""

from typing import List, Literal, Optional

from pydantic import BaseModel


class CoordinationResult(BaseModel):
    emergency_id: str
    risk_level: Literal["LOW", "MEDIUM", "HIGH"]
    hazards: List[str]
    recommended_action: str
    recommended_exit: Optional[str]
    explanation: str
    human_approval_required: bool = True
    limitations: str = (
        "Prototype recommendation only. Not a validated emergency-response decision."
    )
