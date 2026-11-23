using UnityEngine;

namespace EmergencyAI.Modules.Environment
{
    /// <summary>
    /// Marks an emergency exit and whether it is currently blocked.
    /// </summary>
    public class ExitMarker : MonoBehaviour
    {
        [SerializeField] private string exitName = "Exit A";
        [SerializeField] private bool isBlocked;

        public string ExitName
        {
            get => exitName;
            set => exitName = value;
        }

        public bool IsBlocked
        {
            get => isBlocked;
            set
            {
                isBlocked = value;
                ApplyVisualState();
            }
        }

        public Vector3 Position => transform.position;

        private void Start()
        {
            ApplyVisualState();
        }

        private void ApplyVisualState()
        {
            var renderer = GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                return;
            }

            renderer.material.color = isBlocked
                ? new Color(0.85f, 0.15f, 0.15f)
                : new Color(0.15f, 0.75f, 0.25f);
        }
    }
}
