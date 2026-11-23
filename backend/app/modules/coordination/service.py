"""Service wrapper around the coordination agent."""

from app.modules.coordination.agent import CoordinationAgent
from app.modules.coordination.schemas import CoordinationResult
from app.modules.evacuation.schemas import EvacuationPlanResult
from app.modules.risk_assessment.schemas import RiskAssessmentResult


class CoordinationService:
    def __init__(self, agent: CoordinationAgent | None = None) -> None:
        self.agent = agent or CoordinationAgent()

    def coordinate(
        self,
        emergency_id: str,
        risk_assessment: RiskAssessmentResult,
        evacuation_plan: EvacuationPlanResult,
    ) -> CoordinationResult:
        return self.agent.coordinate(
            emergency_id=emergency_id,
            risk_assessment=risk_assessment,
            evacuation_plan=evacuation_plan,
        )
