using UnityEngine;
using UnityEngine.AI;

namespace EmergencyAI.Modules.Evacuation
{
    /// <summary>
    /// Simulated occupant. Moves toward an exit using NavMesh when available,
    /// otherwise falls back to direct movement so the MVP still runs.
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class EvacueeController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float arriveDistance = 0.75f;

        private NavMeshAgent _agent;
        private Transform _destination;
        private bool _useNavMesh;

        public EvacuationStatus Status { get; private set; } = EvacuationStatus.Idle;
        public Vector3 Position => transform.position;
        public bool HasEvacuated => Status == EvacuationStatus.Evacuated;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null)
            {
                _agent = gameObject.AddComponent<NavMeshAgent>();
            }

            _agent.speed = moveSpeed;
            _agent.stoppingDistance = arriveDistance;
            _agent.radius = 0.3f;
            _agent.height = 2f;
        }

        private void Start()
        {
            // NavMesh must be baked (or built at runtime) for pathfinding.
            _useNavMesh = _agent.isOnNavMesh;
            if (!_useNavMesh)
            {
                _agent.enabled = false;
            }
        }

        private void Update()
        {
            if (Status != EvacuationStatus.Evacuating || _destination == null)
            {
                return;
            }

            if (_useNavMesh && _agent.enabled)
            {
                if (!_agent.pathPending &&
                    _agent.remainingDistance <= arriveDistance &&
                    (!_agent.hasPath || _agent.velocity.sqrMagnitude < 0.01f))
                {
                    CompleteEvacuation();
                }

                return;
            }

            Vector3 target = _destination.position;
            target.y = transform.position.y;
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime);

            Vector3 flat = target - transform.position;
            flat.y = 0f;
            if (flat.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(flat);
            }

            if (Vector3.Distance(transform.position, target) <= arriveDistance)
            {
                CompleteEvacuation();
            }
        }

        public void StartEvacuation(Transform exitTransform)
        {
            if (exitTransform == null || Status == EvacuationStatus.Evacuated)
            {
                return;
            }

            _destination = exitTransform;
            Status = EvacuationStatus.Evacuating;

            if (_useNavMesh && _agent != null)
            {
                _agent.enabled = true;
                if (_agent.isOnNavMesh)
                {
                    _agent.isStopped = false;
                    _agent.SetDestination(exitTransform.position);
                    return;
                }

                // Destination unreachable on NavMesh — fall back.
                _useNavMesh = false;
                _agent.enabled = false;
                Status = EvacuationStatus.Blocked;
            }
        }

        public void StopEvacuation()
        {
            if (Status == EvacuationStatus.Evacuated)
            {
                return;
            }

            Status = EvacuationStatus.Idle;
            _destination = null;
            if (_agent != null && _agent.enabled && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }
        }

        private void CompleteEvacuation()
        {
            Status = EvacuationStatus.Evacuated;
            _destination = null;
            if (_agent != null && _agent.enabled)
            {
                _agent.isStopped = true;
                _agent.ResetPath();
            }

            gameObject.SetActive(false);
        }
    }
}
