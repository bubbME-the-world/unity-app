using Newtonsoft.Json;

[System.Serializable]
public struct BridgeInitUser
{
    [JsonProperty("user_id")]
    public string UserID;
    [JsonProperty("user_name")]
    public string UserName;
    [JsonProperty("token")]
    public string Token;
}