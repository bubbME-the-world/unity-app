using Newtonsoft.Json;

[System.Serializable]
public struct UserInventory
{
    [JsonProperty("id")]
    public int ID;

    [JsonProperty("user_id")]
    public string UserID;

    [JsonProperty("item_id")]
    public int ItemID;
}