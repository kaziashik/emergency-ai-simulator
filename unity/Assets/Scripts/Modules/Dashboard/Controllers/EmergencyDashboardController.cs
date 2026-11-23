using System.Collections.Generic;
using System.Text;
using EmergencyAI.Core;
using EmergencyAI.Modules.Emergency;
using UnityEngine;
using UnityEngine.UI;

namespace EmergencyAI.Modules.Dashboard
{
    /// <summary>
    /// Runtime uGUI Emergency Control Centre dashboard.
    /// Built in code so the MVP works without manual Canvas setup.
    /// </summary>
    public class EmergencyDashboardController : MonoBehaviour
    {
        private SimulationManager _manager;

        private Text _statusText;
        private Text _occupantsText;
        private Text _evacuatedText;
        private Text _remainingText;
        private Text _fireLocationText;
        private Text _fireSeverityText;
        private Text _blockedExitsText;
        private Text _riskText;
        private Text _recommendedExitText;
        private Text _explanationText;
        private Text _approvalStatusText;
        private Text _historyText;

        public void Bind(SimulationManager manager)
        {
            _manager = manager;
        }

        public void BuildUi()
        {
            var canvasGo = new GameObject("DashboardCanvas");
            canvasGo.transform.SetParent(transform);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            EnsureEventSystem();

            var panel = CreatePanel(canvasGo.transform, "MainPanel");
            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 12, 12);
            layout.spacing = 6;
            layout.childForceExpandHeight = false;
            layout.childControlHeight = true;
            layout.childControlWidth = true;

            CreateTitle(panel.transform, "EMERGENCY CONTROL CENTRE", 26);
            _statusText = CreateBody(panel.transform, "SIMULATION INACTIVE", 16);

            var stats = CreateRow(panel.transform);
            _occupantsText = CreateStat(stats.transform, "0\nOccupants");
            _evacuatedText = CreateStat(stats.transform, "0\nEvacuated");
            _remainingText = CreateStat(stats.transform, "0\nRemaining");

            CreateTitle(panel.transform, "Emergency information", 18);
            _fireLocationText = CreateBody(panel.transform, "Fire location: -", 14);
            _fireSeverityText = CreateBody(panel.transform, "Fire severity: -", 14);
            _blockedExitsText = CreateBody(panel.transform, "Blocked exits: -", 14);

            CreateTitle(panel.transform, "AI Recommendation", 18);
            _riskText = CreateBody(panel.transform, "Risk: -", 16);
            _recommendedExitText = CreateBody(panel.transform, "Recommended exit: -", 14);
            _explanationText = CreateBody(panel.transform, "No analysis yet.", 13);
            _approvalStatusText = CreateBody(panel.transform, "Waiting for analysis", 13);

            var actions = CreateRow(panel.transform);
            CreateButton(actions.transform, "Start Simulation", () => _manager?.StartSimulation());
            CreateButton(actions.transform, "Trigger Emergency", () => _manager?.TriggerTestEmergency());
            CreateButton(actions.transform, "Request AI Analysis", () => _manager?.RequestAiAnalysis());

            var decisions = CreateRow(panel.transform);
            CreateButton(decisions.transform, "Approve", () => _manager?.ApproveRecommendation());
            CreateButton(decisions.transform, "Reject", () => _manager?.RejectRecommendation());

            CreateTitle(panel.transform, "Decision history", 16);
            _historyText = CreateBody(panel.transform, "No decisions yet.", 12);
        }

        public void UpdateSimulationStats(
            bool simulationActive,
            int occupants,
            int evacuated,
            int remaining,
            string fireLocation,
            int fireSeverity,
            string blockedExits)
        {
            if (_statusText != null)
            {
                _statusText.text = simulationActive ? "SIMULATION ACTIVE" : "SIMULATION INACTIVE";
            }

            SetStat(_occupantsText, occupants, "Occupants");
            SetStat(_evacuatedText, evacuated, "Evacuated");
            SetStat(_remainingText, remaining, "Remaining");

            if (_fireLocationText != null)
            {
                _fireLocationText.text = "Fire location: " + fireLocation;
            }

            if (_fireSeverityText != null)
            {
                _fireSeverityText.text = $"Fire severity: {fireSeverity} / 10";
            }

            if (_blockedExitsText != null)
            {
                _blockedExitsText.text = "Blocked exits: " + blockedExits;
            }
        }

        public void UpdateRecommendation(
            AiRecommendation recommendation,
            bool awaitingApproval,
            string statusMessage = null)
        {
            if (recommendation == null)
            {
                if (_riskText != null) _riskText.text = "Risk: -";
                if (_recommendedExitText != null) _recommendedExitText.text = "Recommended exit: -";
                if (_explanationText != null)
                {
                    _explanationText.text = string.IsNullOrEmpty(statusMessage)
                        ? "No analysis yet."
                        : statusMessage;
                }

                if (_approvalStatusText != null)
                {
                    _approvalStatusText.text = string.IsNullOrEmpty(statusMessage)
                        ? "Waiting for analysis"
                        : statusMessage;
                }

                return;
            }

            // Values come from FastAPI — not hardcoded.
            if (_riskText != null)
            {
                _riskText.text = $"{recommendation.RiskLevel} RISK  ·  source: {recommendation.Source}";
            }

            if (_recommendedExitText != null)
            {
                string action = string.IsNullOrEmpty(recommendation.RecommendedAction)
                    ? ""
                    : $"\n{recommendation.RecommendedAction}";
                _recommendedExitText.text =
                    "Recommended exit: " + (recommendation.RecommendedExit ?? "None") + action;
            }

            if (_explanationText != null)
            {
                string hazards = recommendation.Hazards != null && recommendation.Hazards.Count > 0
                    ? "Hazards: " + string.Join("; ", recommendation.Hazards) + "\n"
                    : "";
                _explanationText.text = hazards + recommendation.Explanation;
            }

            if (_approvalStatusText != null)
            {
                if (!string.IsNullOrEmpty(statusMessage))
                {
                    _approvalStatusText.text = statusMessage;
                }
                else
                {
                    _approvalStatusText.text = awaitingApproval
                        ? "Waiting for operator approval"
                        : "Decision recorded (approval separate from execution)";
                }
            }
        }

        public void UpdateDecisionHistory(IReadOnlyList<OperatorDecision> history)
        {
            if (_historyText == null)
            {
                return;
            }

            if (history == null || history.Count == 0)
            {
                _historyText.text = "No decisions yet.";
                return;
            }

            var sb = new StringBuilder();
            int shown = Mathf.Min(5, history.Count);
            for (int i = 0; i < shown; i++)
            {
                sb.AppendLine(history[i].ToString());
            }

            _historyText.text = sb.ToString();
        }

        private static void SetStat(Text text, int value, string label)
        {
            if (text != null)
            {
                text.text = $"{value}\n{label}";
            }
        }

        private static void EnsureEventSystem()
        {
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null)
            {
                return;
            }

            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        private static GameObject CreatePanel(Transform parent, string name)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.62f, 0.02f);
            rt.anchorMax = new Vector2(0.99f, 0.98f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.08f, 0.1f, 0.14f, 0.92f);
            return panel;
        }

        private static void CreateTitle(Transform parent, string text, int size)
        {
            var t = CreateText(parent, text, size, FontStyle.Bold);
            t.color = Color.white;
        }

        private static Text CreateBody(Transform parent, string text, int size)
        {
            return CreateText(parent, text, size, FontStyle.Normal);
        }

        private static Text CreateText(Transform parent, string content, int size, FontStyle style)
        {
            var go = new GameObject("Text", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.text = content;
            text.fontSize = size;
            text.fontStyle = style;
            text.color = new Color(0.9f, 0.92f, 0.95f);
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            var le = go.AddComponent<LayoutElement>();
            le.minHeight = size + 8;
            le.preferredHeight = size + 10;
            return text;
        }

        private static GameObject CreateRow(Transform parent)
        {
            var row = new GameObject("Row", typeof(RectTransform));
            row.transform.SetParent(parent, false);
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8;
            layout.childForceExpandWidth = true;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            var le = row.AddComponent<LayoutElement>();
            le.minHeight = 36;
            le.preferredHeight = 40;
            return row;
        }

        private static Text CreateStat(Transform parent, string content)
        {
            var go = new GameObject("Stat", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = new Color(0.15f, 0.18f, 0.24f, 1f);
            var text = CreateText(go.transform, content, 15, FontStyle.Bold);
            text.alignment = TextAnchor.MiddleCenter;
            var rt = text.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(4, 4);
            rt.offsetMax = new Vector2(-4, -4);
            return text;
        }

        private static void CreateButton(Transform parent, string label, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            go.GetComponent<Image>().color = new Color(0.18f, 0.42f, 0.72f, 1f);
            var button = go.GetComponent<Button>();
            button.onClick.AddListener(onClick);

            var textGo = new GameObject("Label", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var text = textGo.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.text = label;
            text.fontSize = 13;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            var rt = text.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
