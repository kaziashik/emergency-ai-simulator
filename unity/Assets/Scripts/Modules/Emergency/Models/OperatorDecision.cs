using System;

namespace EmergencyAI.Modules.Emergency
{
    public enum DecisionType
    {
        Approved,
        Rejected
    }

    [Serializable]
    public class OperatorDecision
    {
        public string EmergencyId;
        public DecisionType Decision;
        public string RecommendedExit;
        public string RiskLevel;
        public string Timestamp;

        public override string ToString()
        {
            return $"[{Timestamp}] {Decision} · {RiskLevel} · exit={RecommendedExit} · id={EmergencyId}";
        }
    }
}
