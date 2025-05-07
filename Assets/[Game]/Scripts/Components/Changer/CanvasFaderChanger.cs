using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class CanvasFaderChanger : MonoBehaviour
{
    [SerializeField] private CanvasFader fader;
    [SerializeField] private ChannelBoolean channelRequest;

    private readonly CancellationTokenSource cts = new();

    private void Start()
    {
        fader.Show().Forget();

        channelRequest.Current.Subscribe(RequestHandler).AddTo(cts.Token);
    }

    private async UniTaskVoid RequestHandler(bool value)
    {
        // Notify the channel that this subscriber has started
        channelRequest.NotifySubscriber();

        if (value)
            await fader.Show();
        else
            await fader.Hide();

        // Notify the channel that this subscriber has finished
        channelRequest.SubscriberCompleted();
    }
}
