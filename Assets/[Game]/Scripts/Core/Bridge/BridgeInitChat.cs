using Newtonsoft.Json;

[System.Serializable]
public struct BridgeInitChat
{
    [JsonProperty("chat")]
    public bool Chat;
    [JsonProperty("mood_name")]
    public string MoodName;
}