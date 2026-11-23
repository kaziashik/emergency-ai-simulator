using UnityEngine;

namespace EmergencyAI.Core
{
    /// <summary>
    /// Auto-creates GameManager + SimulationManager if the scene is empty.
    /// </summary>
    public class SceneEntryPoint : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoBootstrap()
        {
            if (FindObjectOfType<SimulationManager>() != null ||
                FindObjectOfType<GameManager>() != null)
            {
                return;
            }

            var go = new GameObject("EmergencyAI_Simulation");
            go.AddComponent<GameManager>();
            go.AddComponent<SimulationManager>();
        }
    }
}
