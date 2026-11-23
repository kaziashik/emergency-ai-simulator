"""FastAPI entrypoint for EmergencyAI Simulator."""

from fastapi import FastAPI

from app.api import api_router
from app.core.config import settings
from app.middlewares import add_cors_middleware

app = FastAPI(
    title="EmergencyAI Simulator API",
    description=(
        "Academic research prototype for AI-assisted evacuation recommendations. "
        "Not a validated real-world emergency response system."
    ),
    version="0.1.0",
)

add_cors_middleware(app)
app.include_router(api_router)


@app.get("/")
def root() -> dict:
    return {
        "service": settings.app_name,
        "docs": "/docs",
        "health": "/health",
    }
