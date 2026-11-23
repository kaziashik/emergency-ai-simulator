"""Coordinates risk → planning → coordination for one emergency."""

from app.modules.coordination.service import CoordinationService
from app.modules.emergency.schemas import EmergencyAnalyzeResponse, EmergencyRequest
from app.modules.emergency.validators import usable_exits
from app.modules.evacuation.service import EvacuationPlanningService
from app.modules.risk_assessment.service import RiskAssessmentService


class EmergencyAnalysisService:
    """Orchestrates the three agent modules into one API response."""

    def __init__(
        self,
        risk_service: RiskAssessmentService | None = None,
        planning_service: EvacuationPlanningService | None = None,
        coordination_service: CoordinationService | None = None,
    ) -> None:
        self.risk_service = risk_service or RiskAssessmentService()
        self.planning_service = planning_service or EvacuationPlanningService()
        self.coordination_service = coordination_service or CoordinationService()

    def analyze(self, request: EmergencyRequest) -> EmergencyAnalyzeResponse:
        usable = usable_exits(request)

        risk = self.risk_service.assess(
            fire_severity=request.fire_severity,
            total_occupants=request.total_occupants,
            available_exits=usable,
            blocked_exits=request.blocked_exits,
        )
        plan = self.planning_service.plan(
            risk_assessment=risk,
            available_exits=usable,
            blocked_exits=request.blocked_exits,
        )
        recommendation = self.coordination_service.coordinate(
            emergency_id=request.emergency_id,
            risk_assessment=risk,
            evacuation_plan=plan,
        )

        return EmergencyAnalyzeResponse(
            acknowledgement=(
                f"Emergency {request.emergency_id} analysed. "
                "Human approval is required before acting."
            ),
            request=request,
            risk_assessment=risk,
            evacuation_plan=plan,
            recommendation=recommendation,
        )
