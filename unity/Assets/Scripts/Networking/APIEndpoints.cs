namespace EmergencyAI.Networking
{
    /// <summary>
    /// Central list of FastAPI paths used by the Unity client.
    /// </summary>
    public static class APIEndpoints
    {
        public const string Health = "/health";
        public const string EmergencyAnalyze = "/emergency/analyze";
    }
}
