using Cysharp.Threading.Tasks;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    [SerializeField] MiniGameDataService gameDataService;
    [SerializeField] ChannelMiniGame channelMiniGame;

    public async UniTask GetMiniGames()
    {
        channelMiniGame.Current.Value = await gameDataService.GetMiniGame();
    }
}