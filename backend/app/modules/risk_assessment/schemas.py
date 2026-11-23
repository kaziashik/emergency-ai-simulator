"""Schemas for risk assessment outputs."""

from typing import List, Literal

from pydantic import BaseModel


class RiskAssessmentResult(BaseModel):
    risk_level: Literal["LOW", "MEDIUM", "HIGH"]
    hazards: List[str]
    explanation: str
