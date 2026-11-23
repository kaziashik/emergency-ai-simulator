"""Extra domain validation beyond Pydantic field checks."""

from typing import List

from app.modules.emergency.schemas import EmergencyRequest


def usable_exits(request: EmergencyRequest) -> List[str]:
    """
    Compute exits that remain usable.

    If an exit appears in both available and blocked lists, blocked wins.
    """
    blocked = set(request.blocked_exits)
    return [name for name in request.available_exits if name not in blocked]
