using System;
using System.Collections.Generic;
using System.Linq;
using EmergencyAI.Modules.Dashboard;
using EmergencyAI.Modules.Emergency;
using EmergencyAI.Modules.Environment;
using EmergencyAI.Modules.Evacuation;
using EmergencyAI.Networking;
using UnityEngine;

namespace EmergencyAI.Core
{
    /// <summary>
    /// Owns simulation lifecycle: build scene, spawn people, evacuate,
    /// call FastAPI for AI analysis, and record operator approve/reject.
    /// </summary>
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private int occupantCount = 10;
        [SerializeField] private bool autoBuildOnStart = true;
        [SerializeField] private string apiBaseUrl = "http://127.0.0.1:8000";

        private BuildingBootstrap _building;
        private EmergencyDashboardController _dashboard;
        private APIClient _apiClient;
        private readonly List<EvacueeController> _evacuees = new List<EvacueeController>();
        private readonly List<OperatorDecision> _decisionHistory = new List<OperatorDecision>();

        private AiRecommendation _currentRecommendation;
        private string _emergencyId = "EMG001";
        private string _statusMessage = "Waiting for analysis";
        private bool _simulationActive;
        private bool _evacuationStarted;
        private bool _awaitingApproval;
        private bool _analysisInFlight;

        public IReadOnlyList<OperatorDecision> DecisionHistory => _decisionHistory;

        private void Start()
        {
            if (autoBuildOnStart)
            {
                BootstrapScene();
            }

            RefreshDashboard();
        }

        private void Update()
        {
            if (_simulationActive)
            {
                RefreshDashboard();
            }
        }

        public void BootstrapScene()
        {
            _building = FindObjectOfType<BuildingBootstrap>();
            if (_building == null)
            {
                var buildingGo = new GameObject("BuildingBootstrap");
                _building = buildingGo.AddComponent<BuildingBootstrap>();
            }

            _building.Build();

            _apiClient = FindObjectOfType<APIClient>();
            if (_apiClient == null)
            {
                _apiClient = gameObject.AddComponent<APIClient>();
            }

            _apiClient.BaseUrl = apiBaseUrl;

            _dashboard = FindObjectOfType<EmergencyDashboardController>();
            if (_dashboard == null)
            {
                var uiGo = new GameObject("EmergencyDashboard");
                _dashboard = uiGo.AddComponent<EmergencyDashboardController>();
                _dashboard.BuildUi();
                _dashboard.Bind(this);
            }

            SpawnOccupants();
            _simulationActive = true;
            RefreshDashboard();
        }

        public void StartSimulation()
        {
            _simulationActive = true;
            _statusMessage = "Simulation active";
            RefreshDashboard();
        }

        public void TriggerTestEmergency()
        {
            _emergencyId = "EMG" + DateTime.Now.ToString("HHmmss");
            if (_building != null && _building.ExitB != null)
            {
                _building.ExitB.IsBlocked = true;
            }

            _currentRecommendation = null;
            _awaitingApproval = false;
            _evacuationStarted = false;
            _analysisInFlight = false;
            _statusMessage = "Test emergency ready — request AI analysis";
            RefreshDashboard();
        }

        public void RequestAiAnalysis()
        {
            if (_building == null || _apiClient == null)
            {
                _statusMessage = "Simulation not ready.";
                RefreshDashboard();
                return;
            }

            if (_analysisInFlight || _apiClient.IsBusy)
            {
                _statusMessage = "Analysis already in progress…";
                RefreshDashboard();
                return;
            }

            var exits = new[] { _building.ExitA, _building.ExitB }
                .Where(e => e != null);

            EmergencyRequestDto request = _apiClient.BuildRequest(
                _emergencyId,
                _building.Fire,
                exits,
                _evacuees);

            _analysisInFlight = true;
            _awaitingApproval = false;
            _currentRecommendation = null;
            _statusMessage = $"Contacting FastAPI at {_apiClient.BaseUrl}…";
            RefreshDashboard();

            StartCoroutine(_apiClient.AnalyzeEmergency(
                request,
                onSuccess: OnAnalysisSuccess,
                onError: OnAnalysisError));
        }

        private void OnAnalysisSuccess(AiRecommendation recommendation)
        {
            _analysisInFlight = false;
            _currentRecommendation = recommendation;
            _awaitingApproval = recommendation != null && recommendation.HumanApprovalRequired;
            _statusMessage = _awaitingApproval
                ? "Waiting for operator approval (from FastAPI)"
                : "Recommendation received from FastAPI";
            RefreshDashboard();
            Debug.Log(
                $"[EmergencyAI] FastAPI recommendation: {recommendation.RiskLevel} → " +
                $"{recommendation.RecommendedExit}");
        }

        private void OnAnalysisError(string error)
        {
            _analysisInFlight = false;
            _currentRecommendation = null;
            _awaitingApproval = false;
            _statusMessage = error;
            RefreshDashboard();
            Debug.LogError("[EmergencyAI] " + error);
        }

        public void ApproveRecommendation()
        {
            if (_currentRecommendation == null || !_awaitingApproval)
            {
                return;
            }

            RecordDecision(DecisionType.Approved);
            _awaitingApproval = false;
            _statusMessage = "Approved — starting evacuation toward recommended exit";
            BeginEvacuationTowardRecommendedExit();
            RefreshDashboard();
        }

        public void RejectRecommendation()
        {
            if (_currentRecommendation == null || !_awaitingApproval)
            {
                return;
            }

            RecordDecision(DecisionType.Rejected);
            _awaitingApproval = false;
            _statusMessage = "Rejected — evacuation not started";
            RefreshDashboard();
        }

        private void BeginEvacuationTowardRecommendedExit()
        {
            if (_evacuationStarted || _currentRecommendation == null)
            {
                return;
            }

            ExitMarker target = ResolveExit(_currentRecommendation.RecommendedExit);
            if (target == null || target.IsBlocked)
            {
                Debug.LogWarning("[EmergencyAI] No usable exit for evacuation.");
                _statusMessage = "No usable exit for evacuation";
                return;
            }

            foreach (var person in _evacuees)
            {
                if (person != null && !person.HasEvacuated)
                {
                    person.StartEvacuation(target.transform);
                }
            }

            _evacuationStarted = true;
        }

        private ExitMarker ResolveExit(string exitName)
        {
            if (_building == null || string.IsNullOrEmpty(exitName))
            {
                return null;
            }

            if (_building.ExitA != null &&
                string.Equals(_building.ExitA.ExitName, exitName, StringComparison.OrdinalIgnoreCase))
            {
                return _building.ExitA;
            }

            if (_building.ExitB != null &&
                string.Equals(_building.ExitB.ExitName, exitName, StringComparison.OrdinalIgnoreCase))
            {
                return _building.ExitB;
            }

            return null;
        }

        private void RecordDecision(DecisionType decision)
        {
            var entry = new OperatorDecision
            {
                EmergencyId = _currentRecommendation.EmergencyId,
                Decision = decision,
                RecommendedExit = _currentRecommendation.RecommendedExit,
                RiskLevel = _currentRecommendation.RiskLevel,
                Timestamp = DateTime.Now.ToString("HH:mm:ss")
            };
            _decisionHistory.Insert(0, entry);
            Debug.Log("[EmergencyAI] Operator decision: " + entry);
        }

        private void SpawnOccupants()
        {
            foreach (var existing in _evacuees)
            {
                if (existing != null)
                {
                    Destroy(existing.gameObject);
                }
            }

            _evacuees.Clear();

            Vector3[] spawnPoints =
            {
                new Vector3(-7f, 0f, 3f),
                new Vector3(-6f, 0f, 2f),
                new Vector3(-5f, 0f, 4f),
                new Vector3(-1f, 0f, 3.5f),
                new Vector3(0f, 0f, 2f),
                new Vector3(1f, 0f, 4f),
                new Vector3(5f, 0f, -2f),
                new Vector3(6f, 0f, -3f),
                new Vector3(7f, 0f, -1.5f),
                new Vector3(-2f, 0f, -1f)
            };

            int count = Mathf.Min(occupantCount, spawnPoints.Length);
            var root = new GameObject("Occupants").transform;

            for (int i = 0; i < count; i++)
            {
                var person = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                person.name = $"Evacuee_{i + 1}";
                person.transform.SetParent(root);
                person.transform.position = spawnPoints[i] + Vector3.up;
                person.transform.localScale = new Vector3(0.6f, 1f, 0.6f);

                var renderer = person.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(0.2f, 0.55f, 0.95f);
                }

                var controller = person.AddComponent<EvacueeController>();
                _evacuees.Add(controller);
            }
        }

        private void RefreshDashboard()
        {
            if (_dashboard == null)
            {
                return;
            }

            int total = _evacuees.Count;
            int evacuated = _evacuees.Count(e => e != null && e.HasEvacuated);
            int remaining = total - evacuated;

            var blockedNames = new[] { _building?.ExitA, _building?.ExitB }
                .Where(e => e != null && e.IsBlocked)
                .Select(e => e.ExitName)
                .ToList();
            string blocked = blockedNames.Count > 0 ? string.Join(", ", blockedNames) : "None";

            string fireLocation = _building != null && _building.Fire != null
                ? _building.Fire.LocationName
                : "-";
            int fireSeverity = _building != null && _building.Fire != null
                ? _building.Fire.Severity
                : 0;

            _dashboard.UpdateSimulationStats(
                simulationActive: _simulationActive,
                occupants: total,
                evacuated: evacuated,
                remaining: remaining,
                fireLocation: fireLocation,
                fireSeverity: fireSeverity,
                blockedExits: blocked);

            _dashboard.UpdateRecommendation(
                _currentRecommendation,
                _awaitingApproval,
                _statusMessage);
            _dashboard.UpdateDecisionHistory(_decisionHistory);
        }
    }
}
