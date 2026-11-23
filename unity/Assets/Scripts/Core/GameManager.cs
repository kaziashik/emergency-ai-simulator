using UnityEngine;

namespace EmergencyAI.Core
{
    /// <summary>
    /// Top-level entry for the Unity application.
    /// Owns the SimulationManager for the emergency scene.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SimulationManager simulationManager;

        public SimulationManager Simulation => simulationManager;

        private void Awake()
        {
            if (simulationManager == null)
            {
                simulationManager = GetComponent<SimulationManager>();
            }

            if (simulationManager == null)
            {
                simulationManager = gameObject.AddComponent<SimulationManager>();
            }
        }
    }
}
