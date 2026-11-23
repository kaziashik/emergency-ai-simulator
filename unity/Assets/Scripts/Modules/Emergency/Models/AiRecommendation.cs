using System;
using System.Collections.Generic;

namespace EmergencyAI.Modules.Emergency
{
    /// <summary>
    /// Structured recommendation shape matching the Python backend response.
    /// </summary>
    [Serializable]
    public class AiRecommendation
    {
        public string EmergencyId;
        public string RiskLevel;
        public List<string> Hazards = new List<string>();
        public string RecommendedAction;
        public string RecommendedExit;
        public string Explanation;
        public bool HumanApprovalRequired = true;
        public string FireLocation;
        public int FireSeverity;
        public List<string> AvailableExits = new List<string>();
        public List<string> BlockedExits = new List<string>();
        public string Source = "FastAPI";
    }
}
