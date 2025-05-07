using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Rendering.STP;

public class UserDataService : MonoBehaviour
{
    [SerializeField] string baseURL;
    [SerializeField] string anonKey;

    public async UniTask<User> GetUser(string userID, int maxValue)
    {
        string url = $"{baseURL}users?select=id,user_id,updated_at,point,coin,user_status(key,value)&user_id=eq.{userID}";

        User user = new();

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
            try
            {
                User[] users = JsonConvert.DeserializeObject<User[]>(request.downloadHandler.text);

                if (users.Length > 0)
                {
                    user = users[0]; // Use the first user if found
                    Debug.Log($"User found: {user.UserID}");
                }
                else
                {
                    Debug.LogWarning("No user found with the given userID. Creating a new user...");

                    // Call a method to create a new user
                    user = await CreateNewUser(userID, maxValue);
                }
            }
            catch (JsonException ex)
            {
                Debug.LogError($"Error parsing JSON: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"Error fetching user. Status: {request.responseCode}, Error: {request.error}");
        }

        return user;
    }

    public async UniTask<User> CreateNewUser(string userID, int maxvalue)
    {
        string url = $"{baseURL}users";
        string updatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        Debug.Log($"Creating new user at: {url}");

        // Create a new user object
        User newUser = new()
        {
            UserID = userID,
            LastUpdate = updatedAt // ISO 8601 format
        };

        string json = $"{{" +
            $"\"user_id\": \"{userID}\"," +
            $"\"updated_at\": \"{updatedAt}\"}}";

        Debug.Log(json);

        // Create the POST request
        UnityWebRequest request = new(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        // Set headers
        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", $"Bearer {anonKey}");
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and await its completion
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success || request.responseCode == 201) // HTTP 201: Created
        {
            try
            {
                // Deserialize the created user to get its ID (or user_id)
                // newUser = JsonConvert.DeserializeObject<User>(request.downloadHandler.text);
                UserStatus[] status = CreateDefaultStatus(maxvalue);

                for (int i = 0; i < status.Length; i++)
                {
                    await InsertUserStatus(newUser.UserID, status[i]);
                }

                newUser.Status = status;
                return newUser;
            }
            catch (JsonException ex)
            {
                newUser.ID = null;
                Debug.LogError($"Error parsing JSON: {ex.Message}");
            }
        }
        else
        {
            newUser.ID = null;
            Debug.LogError($"Error creating new user. Status: {request.responseCode}, Error: {request.error}");
        }

        return newUser; // Return null if creation failed
    }

    private async UniTask InsertUserStatus(string userID, UserStatus status)
    {
        // URL for the `user_status` table
        string statusUrl = $"{baseURL}user_status";
        Debug.Log($"Creating user status at: {statusUrl}");

        string json = $"{{" +
            $"\"user_id\": \"{userID}\"," +
            $"\"key\": \"{status.Key}\"," +
            $"\"value\": \"{status.Value}\"}}";

        // Create the POST request for the `user_status` table
        UnityWebRequest statusRequest = new(statusUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        // Set headers
        statusRequest.SetRequestHeader("apikey", anonKey);
        statusRequest.SetRequestHeader("Authorization", $"Bearer {anonKey}");
        statusRequest.SetRequestHeader("Content-Type", "application/json");

        // Send the request and await its completion
        await statusRequest.SendWebRequest();

        if (statusRequest.result == UnityWebRequest.Result.Success || statusRequest.responseCode == 201) // HTTP 201: Created
        {
            Debug.Log($"User status created successfully: {statusRequest.downloadHandler.text}");
        }
        else
        {
            Debug.LogError($"Error creating user status. Status: {statusRequest.responseCode}, Error: {statusRequest.error}");
        }
    }

    UserStatus[] CreateDefaultStatus(int maxValue)
    {
        UserStatus hunger = new()
        {
            Key = StatusType.HUNGER,
            Value = maxValue,
        };

        UserStatus hygiene = new()
        {
            Key = StatusType.HYGIENE,
            Value = maxValue,
        };

        UserStatus energy = new()
        {
            Key = StatusType.ENERGY,
            Value = maxValue,
        };

        UserStatus mood = new()
        {
            Key = StatusType.MOOD,
            Value = maxValue,
        };

        return new UserStatus[4] { hunger, hygiene, energy, mood };
    }

    public async UniTask<UserInventory[]> GetUserItem(string userID)
    {
        string url = $"{baseURL}user_inventory?select=id,user_id,item_id&user_id=eq.{userID}";

        UserInventory[] userItems = new UserInventory[0];

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
            try
            {
                userItems = JsonConvert.DeserializeObject<UserInventory[]>(request.downloadHandler.text);
                Debug.Log($"User items parsed successfully. Count: {userItems.Length}");
            }
            catch (JsonException ex)
            {
                Debug.LogError($"Error parsing JSON: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"Error fetching user item. Status: {request.responseCode}, Error: {request.error}");
        }

        return userItems;
    }

    public async UniTask<UserInventory> AddItem(string userID, int itemID)
    {
        string url = $"{baseURL}user_inventory";
        string body = $"{{" +
            $"\"item_id\": \"{itemID}\"," +
            $"\"user_id\": \"{userID}\"}}";

        UserInventory newInventory = new();

        // Create the POST request
        UnityWebRequest request = new(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        // Set headers
        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", $"Bearer {anonKey}");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Prefer", "return=representation");

        // Send the request and await its completion
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success || request.responseCode == 201) // HTTP 201: Created
        {
            Debug.Log($"Item added successfully: {request.downloadHandler.text}");

            UserInventory[] inventories = JsonConvert.DeserializeObject<UserInventory[]>(request.downloadHandler.text);

            if (inventories.Length > 0)
            {
                newInventory = inventories[0];
                return newInventory;
            }
        }
        else
        {
            Debug.LogError($"Error creating user status. Status: {request.responseCode}, Error: {request.error}");
        }

        return newInventory;
    }

    public async UniTask RemoveItem(int userItemID)
    {
        string url = $"{baseURL}user_inventory?id=eq.{userItemID}";

        // Create the DELETE request
        UnityWebRequest request = new(url, "DELETE");

        // Set headers
        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", $"Bearer {anonKey}");
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and await its completion
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success || request.responseCode == 204) // HTTP 204: No content
        {
            Debug.Log($"Item deleted successfully");
        }
        else
        {
            Debug.LogError($"Error deleting item. Status: {request.responseCode}, Error: {request.error}");
        }
    }

    public async UniTask UpdateUserAndStatus(string userID, UserStatus status)
    {
        string updatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

        await UpdateUsers(userID, updatedAt);
        await UpdateUserStatus(userID, status);
    }

    private async UniTask UpdateUsers(string userID, string updatedAt)
    {
        string url = $"{baseURL}users?user_id=eq.{userID}";
        string body = $"{{\"updated_at\": \"{updatedAt}\"}}";

        try
        {
            string response = await SendPatchRequest(url, body);
            Debug.Log("Users table updated successfully: " + response);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error updating users table: " + ex.Message);
        }
    }

    private async UniTask UpdateUserStatus(string userID, UserStatus status)
    {
        string url = $"{baseURL}user_status?user_id=eq.{userID}&key=eq.{status.Key}";
        string body = $"{{\"value\": {status.Value}}}";

        try
        {
            string response = await SendPatchRequest(url, body);
            Debug.Log($"Status '{status.Key}' updated successfully: " + response);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating status '{status.Key}': " + ex.Message);
        }
    }

    private async UniTask<string> SendPatchRequest(string url, string jsonBody)
    {
        using UnityWebRequest request = new(url, "PATCH");
        request.SetRequestHeader("apikey", anonKey);
        request.SetRequestHeader("Authorization", $"Bearer {anonKey}");
        request.SetRequestHeader("Content-Type", "application/json");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            return request.downloadHandler.text;
        }
        else
        {
            throw new Exception(request.error);
        }
    }
}