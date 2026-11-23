using UnityEngine;

namespace EmergencyAI.Modules.Environment
{
    /// <summary>
    /// Simulated fire hazard. Visual only — no real fire physics.
    /// </summary>
    public class FireHazard : MonoBehaviour
    {
        [SerializeField, Range(1, 10)] private int severity = 8;
        [SerializeField] private string locationName = "Room 2";

        public int Severity
        {
            get => severity;
            set => severity = Mathf.Clamp(value, 1, 10);
        }

        public string LocationName
        {
            get => locationName;
            set => locationName = value;
        }

        public Vector3 Position => transform.position;

        private void OnValidate()
        {
            severity = Mathf.Clamp(severity, 1, 10);
        }
    }
}
