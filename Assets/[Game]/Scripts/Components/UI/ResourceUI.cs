using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] ChannelUser channelUser;
    [SerializeField] TMP_Text point;
    [SerializeField] TMP_Text coin;

    private readonly CancellationTokenSource cts = new();

    private void OnDestroy()
    {
        StopObserve();
    }

    private void Start()
    {
        channelUser.Current.Subscribe(UpdateResourceUI).AddTo(cts.Token);
    }

    public void StopObserve()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    private void UpdateResourceUI(User user)
    {
        point.text = user.Point.ToString();
        coin.text = user.Coin.ToString();
    }
}