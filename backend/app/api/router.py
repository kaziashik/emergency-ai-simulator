"""Top-level API router that mounts feature-module routes."""

from fastapi import APIRouter

from app.core.config import settings
from app.modules.emergency.routes import router as emergency_router

api_router = APIRouter()


@api_router.get("/health")
def health() -> dict:
    """Liveness check for the backend service."""
    return {"status": "healthy", "service": settings.app_name}


api_router.include_router(emergency_router)
