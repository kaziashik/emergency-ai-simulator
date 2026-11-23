"""Deterministic Risk Assessment Agent (rule-based, no LLM)."""

from typing import List

from app.modules.risk_assessment.schemas import RiskAssessmentResult


class RiskAssessmentAgent:
    """Assesses simulated emergency danger using simple rules."""

    def assess(
        self,
        fire_severity: int,
        total_occupants: int,
        available_exits: List[str],
        blocked_exits: List[str],
    ) -> RiskAssessmentResult:
        hazards: List[str] = []

        if fire_severity >= 8:
            hazards.append("High fire severity")
        elif fire_severity >= 5:
            hazards.append("Moderate fire severity")
        else:
            hazards.append("Low fire severity")

        if total_occupants >= 20:
            hazards.append("Large number of occupants")
        elif total_occupants >= 8:
            hazards.append("Multiple occupants present")

        if blocked_exits:
            if len(blocked_exits) == 1:
                hazards.append("One blocked exit")
            else:
                hazards.append(f"{len(blocked_exits)} blocked exits")

        if not available_exits:
            hazards.append("No available exits")

        risk_level = self._compute_risk_level(
            fire_severity=fire_severity,
            total_occupants=total_occupants,
            available_count=len(available_exits),
            blocked_count=len(blocked_exits),
        )

        explanation = self._build_explanation(
            risk_level=risk_level,
            fire_severity=fire_severity,
            total_occupants=total_occupants,
            available_exits=available_exits,
            blocked_exits=blocked_exits,
        )

        return RiskAssessmentResult(
            risk_level=risk_level,
            hazards=hazards,
            explanation=explanation,
        )

    def _compute_risk_level(
        self,
        fire_severity: int,
        total_occupants: int,
        available_count: int,
        blocked_count: int,
    ) -> str:
        score = 0
        score += fire_severity
        score += 2 if total_occupants >= 20 else (1 if total_occupants >= 8 else 0)
        score += 3 if available_count == 0 else (2 if available_count == 1 else 0)
        score += min(blocked_count, 2)

        if score >= 12 or available_count == 0 or fire_severity >= 9:
            return "HIGH"
        if score >= 7 or fire_severity >= 5:
            return "MEDIUM"
        return "LOW"

    def _build_explanation(
        self,
        risk_level: str,
        fire_severity: int,
        total_occupants: int,
        available_exits: List[str],
        blocked_exits: List[str],
    ) -> str:
        parts = [
            f"Simulated risk level is {risk_level}.",
            f"Fire severity is {fire_severity}/10 with {total_occupants} occupants.",
        ]
        if available_exits:
            parts.append(f"Available exits: {', '.join(available_exits)}.")
        else:
            parts.append("No exits are currently available.")
        if blocked_exits:
            parts.append(f"Blocked exits: {', '.join(blocked_exits)}.")
        return " ".join(parts)
