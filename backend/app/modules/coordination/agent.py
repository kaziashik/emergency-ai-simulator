"""Coordination Agent: merges risk and planning outputs."""

from app.modules.coordination.schemas import CoordinationResult
from app.modules.evacuation.schemas import EvacuationPlanResult
from app.modules.risk_assessment.schemas import RiskAssessmentResult


class CoordinationAgent:
    """Combines agent outputs into a single structured recommendation."""

    def coordinate(
        self,
        emergency_id: str,
        risk_assessment: RiskAssessmentResult,
        evacuation_plan: EvacuationPlanResult,
    ) -> CoordinationResult:
        recommended_exit = evacuation_plan.recommended_exit

        if recommended_exit:
            recommended_action = f"Evacuate occupants through {recommended_exit}"
        else:
            recommended_action = (
                "Do not auto-evacuate: no usable exit available. "
                "Await operator decision."
            )

        explanation = f"{risk_assessment.explanation} {evacuation_plan.reason}"

        return CoordinationResult(
            emergency_id=emergency_id,
            risk_level=risk_assessment.risk_level,
            hazards=risk_assessment.hazards,
            recommended_action=recommended_action,
            recommended_exit=recommended_exit,
            explanation=explanation,
            human_approval_required=True,
        )
