"""Domain exceptions for the EmergencyAI backend."""


class EmergencyAIError(Exception):
    """Base error for application-specific failures."""


class AnalysisError(EmergencyAIError):
    """Raised when emergency analysis cannot be completed."""
