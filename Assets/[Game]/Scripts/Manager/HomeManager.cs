using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Newtonsoft.Json;
using System.Threading;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    [SerializeField] ConfigManager configManager;
    [SerializeField] ItemManager itemManager;
    [SerializeField] UserManager userManager;

    [SerializeField] ChannelBridge bridge;
    [SerializeField] ChannelRoom room;

    private readonly CancellationTokenSource cts = new();

    private void OnDestroy()
    {
        cts.Cancel();
        cts.Dispose();
    }

    private async void Start()
    {
        await UniTask.WaitForEndOfFrame();

        configManager.LoadConfig().Forget();
        itemManager.LoadItem().Forget();

        bridge.Current.Subscribe(CheckBridgeData).AddTo(cts.Token);
    }

    private void CheckBridgeData(BridgeData data)
    {
        switch (data.Key)
        {
            case BridgeType.INIT_CHAT:
                HandleChat(data);
                break;
            case BridgeType.INIT_USER:
                HandleUser(data);
                break;
            default:
                break;
        }
    }

    void HandleChat(BridgeData data)
    {
        BridgeInitChat initChat = JsonConvert.DeserializeObject<BridgeInitChat>(data.Value);

        room.Current.Value = RoomState.MIDDLE;
        /*
        if (initChat.Chat)
            room.Current.Value = RoomState.CHAT;
        else
            room.Current.Value = RoomState.MIDDLE;
        */
    }

    void HandleUser(BridgeData data)
    {
        BridgeInitUser initUser = JsonConvert.DeserializeObject<BridgeInitUser>(data.Value);

        userManager.LoadUser(initUser.UserID).Forget();
    }
}