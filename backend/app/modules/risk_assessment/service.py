"""Service wrapper around the risk assessment agent."""

from typing import List

from app.modules.risk_assessment.agent import RiskAssessmentAgent
from app.modules.risk_assessment.schemas import RiskAssessmentResult


class RiskAssessmentService:
    def __init__(self, agent: RiskAssessmentAgent | None = None) -> None:
        self.agent = agent or RiskAssessmentAgent()

    def assess(
        self,
        fire_severity: int,
        total_occupants: int,
        available_exits: List[str],
        blocked_exits: List[str],
    ) -> RiskAssessmentResult:
        return self.agent.assess(
            fire_severity=fire_severity,
            total_occupants=total_occupants,
            available_exits=available_exits,
            blocked_exits=blocked_exits,
        )
