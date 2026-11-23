"""Controller for emergency HTTP handlers."""

from app.modules.emergency.schemas import EmergencyAnalyzeResponse, EmergencyRequest
from app.modules.emergency.service import EmergencyAnalysisService


class EmergencyController:
    def __init__(self, service: EmergencyAnalysisService | None = None) -> None:
        self.service = service or EmergencyAnalysisService()

    def analyze(self, payload: EmergencyRequest) -> EmergencyAnalyzeResponse:
        return self.service.analyze(payload)
