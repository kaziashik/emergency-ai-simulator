"""Unit tests for deterministic AI agents."""

from app.modules.coordination.agent import CoordinationAgent
from app.modules.evacuation.agent import EvacuationPlanningAgent
from app.modules.risk_assessment.agent import RiskAssessmentAgent
from app.modules.risk_assessment.schemas import RiskAssessmentResult


def test_risk_low_severity():
    agent = RiskAssessmentAgent()
    result = agent.assess(
        fire_severity=2,
        total_occupants=3,
        available_exits=["Exit A", "Exit B"],
        blocked_exits=[],
    )
    assert result.risk_level == "LOW"
    assert any("Low fire severity" in h for h in result.hazards)


def test_risk_high_severity():
    agent = RiskAssessmentAgent()
    result = agent.assess(
        fire_severity=9,
        total_occupants=15,
        available_exits=["Exit A"],
        blocked_exits=["Exit B"],
    )
    assert result.risk_level == "HIGH"
    assert any("High fire severity" in h for h in result.hazards)


def test_risk_no_available_exits_is_high():
    agent = RiskAssessmentAgent()
    result = agent.assess(
        fire_severity=3,
        total_occupants=5,
        available_exits=[],
        blocked_exits=["Exit A", "Exit B"],
    )
    assert result.risk_level == "HIGH"
    assert "No available exits" in result.hazards


def test_planning_prefers_available_exit():
    risk = RiskAssessmentResult(
        risk_level="HIGH",
        hazards=["High fire severity", "One blocked exit"],
        explanation="test",
    )
    agent = EvacuationPlanningAgent()
    plan = agent.plan(
        risk_assessment=risk,
        available_exits=["Exit A", "Exit B"],
        blocked_exits=["Exit B"],
    )
    assert plan.recommended_exit == "Exit A"
    assert "Exit B" not in plan.usable_exits
    assert "Exit A" in plan.reason


def test_planning_both_exits_blocked():
    risk = RiskAssessmentResult(
        risk_level="HIGH",
        hazards=["No available exits"],
        explanation="test",
    )
    agent = EvacuationPlanningAgent()
    plan = agent.plan(
        risk_assessment=risk,
        available_exits=[],
        blocked_exits=["Exit A", "Exit B"],
    )
    assert plan.recommended_exit is None
    assert plan.usable_exits == []


def test_coordination_requires_human_approval():
    risk = RiskAssessmentResult(
        risk_level="HIGH",
        hazards=["High fire severity"],
        explanation="Severity is high.",
    )
    planner = EvacuationPlanningAgent()
    plan = planner.plan(
        risk_assessment=risk,
        available_exits=["Exit A"],
        blocked_exits=["Exit B"],
    )
    result = CoordinationAgent().coordinate(
        emergency_id="EMG001",
        risk_assessment=risk,
        evacuation_plan=plan,
    )
    assert result.human_approval_required is True
    assert result.recommended_exit == "Exit A"
    assert "Exit A" in result.recommended_action
