using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmergencyAI.Modules.Emergency;
using EmergencyAI.Modules.Environment;
using EmergencyAI.Modules.Evacuation;
using UnityEngine;
using UnityEngine.Networking;

namespace EmergencyAI.Networking
{
    /// <summary>
    /// Collects simulation state, POSTs JSON to FastAPI, returns the AI recommendation.
    /// </summary>
    public class APIClient : MonoBehaviour
    {
        [SerializeField] private string baseUrl = "http://127.0.0.1:8000";
        [SerializeField] private float timeoutSeconds = 10f;

        public string BaseUrl
        {
            get => baseUrl;
            set => baseUrl = value;
        }

        public bool IsBusy { get; private set; }

        public EmergencyRequestDto BuildRequest(
            string emergencyId,
            FireHazard fire,
            IEnumerable<ExitMarker> exits,
            IEnumerable<EvacueeController> occupants)
        {
            var exitList = exits.Where(e => e != null).ToList();
            var available = exitList
                .Where(e => !e.IsBlocked)
                .Select(e => e.ExitName)
                .ToArray();
            var blocked = exitList
                .Where(e => e.IsBlocked)
                .Select(e => e.ExitName)
                .ToArray();

            int total = occupants != null
                ? occupants.Count(o => o != null)
                : 0;

            return new EmergencyRequestDto
            {
                emergency_id = emergencyId,
                fire_location = fire != null ? fire.LocationName : "Unknown",
                fire_severity = fire != null ? fire.Severity : 1,
                total_occupants = total,
                available_exits = available,
                blocked_exits = blocked
            };
        }

        public IEnumerator AnalyzeEmergency(
            EmergencyRequestDto request,
            Action<AiRecommendation> onSuccess,
            Action<string> onError)
        {
            if (IsBusy)
            {
                onError?.Invoke("API request already in progress.");
                yield break;
            }

            if (request == null)
            {
                onError?.Invoke("Emergency request was null.");
                yield break;
            }

            IsBusy = true;
            string url = baseUrl.TrimEnd('/') + APIEndpoints.EmergencyAnalyze;
            string json = JsonUtility.ToJson(request);
            Debug.Log($"[EmergencyAI] POST {url}\n{json}");

            using (var www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                byte[] body = Encoding.UTF8.GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(body);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                www.timeout = Mathf.CeilToInt(timeoutSeconds);

                yield return www.SendWebRequest();

                IsBusy = false;

#if UNITY_2020_2_OR_NEWER
                bool failed = www.result != UnityWebRequest.Result.Success;
#else
                bool failed = www.isNetworkError || www.isHttpError;
#endif
                if (failed)
                {
                    string detail = string.IsNullOrEmpty(www.downloadHandler?.text)
                        ? www.error
                        : $"{www.error} | {www.downloadHandler.text}";
                    onError?.Invoke($"FastAPI request failed: {detail}");
                    yield break;
                }

                string responseJson = www.downloadHandler.text;
                Debug.Log($"[EmergencyAI] Response:\n{responseJson}");

                EmergencyAnalyzeResponseDto dto;
                try
                {
                    dto = JsonUtility.FromJson<EmergencyAnalyzeResponseDto>(responseJson);
                }
                catch (Exception ex)
                {
                    onError?.Invoke($"Failed to parse FastAPI JSON: {ex.Message}");
                    yield break;
                }

                if (dto == null || dto.recommendation == null)
                {
                    onError?.Invoke("FastAPI response missing recommendation.");
                    yield break;
                }

                onSuccess?.Invoke(MapToRecommendation(dto));
            }
        }

        private static AiRecommendation MapToRecommendation(EmergencyAnalyzeResponseDto dto)
        {
            var rec = dto.recommendation;
            var req = dto.request;

            return new AiRecommendation
            {
                EmergencyId = rec.emergency_id,
                RiskLevel = rec.risk_level,
                Hazards = rec.hazards != null
                    ? new List<string>(rec.hazards)
                    : new List<string>(),
                RecommendedAction = rec.recommended_action,
                RecommendedExit = string.IsNullOrEmpty(rec.recommended_exit)
                    ? null
                    : rec.recommended_exit,
                Explanation = rec.explanation,
                HumanApprovalRequired = rec.human_approval_required,
                FireLocation = req != null ? req.fire_location : null,
                FireSeverity = req != null ? req.fire_severity : 0,
                AvailableExits = req?.available_exits != null
                    ? new List<string>(req.available_exits)
                    : new List<string>(),
                BlockedExits = req?.blocked_exits != null
                    ? new List<string>(req.blocked_exits)
                    : new List<string>(),
                Source = "FastAPI"
            };
        }
    }
}
