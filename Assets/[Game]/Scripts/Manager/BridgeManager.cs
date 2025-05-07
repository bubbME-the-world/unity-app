using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BridgeManager : MonoBehaviour
{
    [SerializeField] BridgeScript bridge;
    [SerializeField] ChannelBridge channel;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        bridge.OnReceiveMessage += Bridge_OnReceiveMessage;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        bridge.OnReceiveMessage -= Bridge_OnReceiveMessage;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        bridge.OnReceiveMessage -= Bridge_OnReceiveMessage;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SendLoadedData().Forget();
    }

    private async UniTask SendLoadedData()
    {
        BridgeData data = new()
        {
            Key = BridgeType.UNITY_LOADED,
            Value = string.Empty,
        };

        await UniTask.Delay(TimeSpan.FromMilliseconds(500), ignoreTimeScale: true);

        SendToWeb(data);
    }

    private void Bridge_OnReceiveMessage(string message)
    {
        channel.Current.Value = JsonConvert.DeserializeObject<BridgeData>(message);
    }

    public void RequestScene(string sceneName)
    {
        BridgeData data = new()
        {
            Key = BridgeType.REQUEST_SCENE,
            Value = sceneName,
        };

        SendToWeb(data);
    }

    public void RequestGame(MiniGame miniGame)
    {
        BridgeData data = new()
        {
            Key = BridgeType.REQUEST_MINIGAME,
            Value = JsonConvert.SerializeObject(new BridgeReqGame()
            {
                GameName = miniGame.GameName,
                GameURL = miniGame.GameURL,
                Orientation = miniGame.Orientation,
            }),
        };

        SendToWeb(data);
    }

    public void RequestToken()
    {
        BridgeData data = new()
        {
            Key = BridgeType.REQUEST_TOKEN,
            Value = string.Empty,
        };

        SendToWeb(data);
    }

    public void CompleteQuiz(bool value)
    {
        BridgeData data = new()
        {
            Key = BridgeType.COMPLETE_QUIZ,
            Value = JsonConvert.SerializeObject(new BridgeCompleteQuiz()
            {
                Pass = value,
            }),
        };

        Debug.Log(JsonConvert.SerializeObject(data));

        SendToWeb(data);
    }

    public void CompleteMiniGame()
    {
        BridgeData data = new()
        {
            Key = BridgeType.COMPLETE_MINIGAME,
            Value = string.Empty,
        };

        SendToWeb(data);
    }

    private void SendToWeb(BridgeData data)
    {
#if !UNITY_EDITOR
        bridge.SendMessageToPage(JsonConvert.SerializeObject(data));
#endif
    }
}
