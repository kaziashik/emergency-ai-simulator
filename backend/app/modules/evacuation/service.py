"""Service wrapper around the evacuation planning agent."""

from typing import List

from app.modules.evacuation.agent import EvacuationPlanningAgent
from app.modules.evacuation.schemas import EvacuationPlanResult
from app.modules.risk_assessment.schemas import RiskAssessmentResult


class EvacuationPlanningService:
    def __init__(self, agent: EvacuationPlanningAgent | None = None) -> None:
        self.agent = agent or EvacuationPlanningAgent()

    def plan(
        self,
        risk_assessment: RiskAssessmentResult,
        available_exits: List[str],
        blocked_exits: List[str],
    ) -> EvacuationPlanResult:
        return self.agent.plan(
            risk_assessment=risk_assessment,
            available_exits=available_exits,
            blocked_exits=blocked_exits,
        )
