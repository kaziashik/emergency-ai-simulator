"""Schemas for evacuation planning outputs."""

from typing import List, Optional

from pydantic import BaseModel


class EvacuationPlanResult(BaseModel):
    recommended_exit: Optional[str]
    usable_exits: List[str]
    reason: str
