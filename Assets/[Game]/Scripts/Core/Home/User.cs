using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[System.Serializable]
public struct User
{
    [JsonProperty("id")]
    public int? ID;

    [JsonProperty("user_id")]
    public string UserID;

    [JsonProperty("point")]
    public int Point;

    [JsonProperty("coin")]
    public int Coin;

    [JsonProperty("updated_at")]
    public string LastUpdate;

    [JsonProperty("user_status")]
    public UserStatus[] Status;
}

[System.Serializable]
public struct UserStatus
{
    [JsonProperty("key"), JsonConverter(typeof(StringEnumConverter))]
    public StatusType Key;

    [JsonProperty("value")]
    public int Value;
}