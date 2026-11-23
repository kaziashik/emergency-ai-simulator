using System;

namespace EmergencyAI.Networking
{
    /// <summary>
    /// JSON DTOs matching FastAPI / Pydantic snake_case field names.
    /// </summary>
    [Serializable]
    public class EmergencyRequestDto
    {
        public string emergency_id;
        public string fire_location;
        public int fire_severity;
        public int total_occupants;
        public string[] available_exits;
        public string[] blocked_exits;
    }

    [Serializable]
    public class RiskAssessmentDto
    {
        public string risk_level;
        public string[] hazards;
        public string explanation;
    }

    [Serializable]
    public class EvacuationPlanDto
    {
        public string recommended_exit;
        public string[] usable_exits;
        public string reason;
    }

    [Serializable]
    public class CoordinationDto
    {
        public string emergency_id;
        public string risk_level;
        public string[] hazards;
        public string recommended_action;
        public string recommended_exit;
        public string explanation;
        public bool human_approval_required;
        public string limitations;
    }

    [Serializable]
    public class EmergencyAnalyzeResponseDto
    {
        public string status;
        public string acknowledgement;
        public EmergencyRequestDto request;
        public RiskAssessmentDto risk_assessment;
        public EvacuationPlanDto evacuation_plan;
        public CoordinationDto recommendation;
    }
}
