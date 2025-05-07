using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ConfigDataService : MonoBehaviour
{
    [SerializeField] string baseURL;
    [SerializeField] string anonKey;

    public async UniTask<Config[]> GetConfig()
    {
        string url = $"{baseURL}config";
        //Debug.Log(url);

        Config[] configs = new Config[0];

        UnityWebRequest request = new(url, "GET")
        {
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", $"Bearer {anonKey}"); // Supabase generally requires both
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and await its completion
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Config loaded successfully: {request.downloadHandler.text}");

            try
            {
                // Deserialize the JSON response into Config[]
                configs = JsonConvert.DeserializeObject<Config[]>(request.downloadHandler.text);
                Debug.Log($"Configs parsed successfully. Count: {configs.Length}");
            }
            catch (JsonException ex)
            {
                Debug.LogError($"Error parsing JSON: {ex.Message}");
            }
        }
        else
        {
            // Log errors if the request fails
            Debug.LogError($"Error fetching config. Status: {request.responseCode}, Error: {request.error}");
        }

        return configs;
    }
}