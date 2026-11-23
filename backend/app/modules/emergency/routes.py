"""HTTP routes for emergency analysis."""

from fastapi import APIRouter

from app.modules.emergency.controller import EmergencyController
from app.modules.emergency.schemas import EmergencyAnalyzeResponse, EmergencyRequest

router = APIRouter(tags=["emergency"])
_controller = EmergencyController()


@router.post("/emergency/analyze", response_model=EmergencyAnalyzeResponse)
def analyze_emergency(payload: EmergencyRequest) -> EmergencyAnalyzeResponse:
    """
    Receive emergency data, run agents, return a structured recommendation.

    The AI never executes consequential actions; human approval is required.
    """
    return _controller.analyze(payload)
