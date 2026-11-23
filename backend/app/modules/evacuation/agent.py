"""Deterministic Evacuation Planning Agent (rule-based, no LLM)."""

from typing import List, Optional

from app.modules.evacuation.schemas import EvacuationPlanResult
from app.modules.risk_assessment.schemas import RiskAssessmentResult


class EvacuationPlanningAgent:
    """Selects a usable evacuation exit using simple deterministic rules."""

    def plan(
        self,
        risk_assessment: RiskAssessmentResult,
        available_exits: List[str],
        blocked_exits: List[str],
    ) -> EvacuationPlanResult:
        usable = [
            exit_name
            for exit_name in available_exits
            if exit_name not in blocked_exits
        ]

        if not usable:
            return EvacuationPlanResult(
                recommended_exit=None,
                usable_exits=[],
                reason=(
                    "No usable exits remain after excluding blocked exits. "
                    "Human operator review is required."
                ),
            )

        recommended: Optional[str] = usable[0]
        reason_parts = [f"{recommended} is available"]
        if blocked_exits:
            reason_parts.append(
                f"while {', '.join(blocked_exits)} "
                f"{'is' if len(blocked_exits) == 1 else 'are'} blocked"
            )
        reason_parts.append(f"(risk level: {risk_assessment.risk_level})")

        return EvacuationPlanResult(
            recommended_exit=recommended,
            usable_exits=usable,
            reason=" ".join(reason_parts) + ".",
        )
