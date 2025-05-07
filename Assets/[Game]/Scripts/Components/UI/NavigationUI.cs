using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using UnityEngine;

public class NavigationUI : MonoBehaviour
{
    [SerializeField] ChannelBoolean longPress;
    [SerializeField] CanvasFader canvasFader;
    [SerializeField] RectMover[] rectMovers;

    private readonly CancellationTokenSource cts = new ();

    private void OnDestroy()
    {
        StopObserve();
    }

    private void Start()
    {
        longPress.Current.Subscribe(value => OnLongTouch(value).Forget()).AddTo(cts.Token);
    }

    public void StopObserve()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    private async UniTask OnLongTouch(bool value)
    {
        if (!value)
            return;

        await canvasFader.Show();

        for (int i = 0; i < rectMovers.Length; i++)
            rectMovers[i].Show().Forget();
    }

    public void Hide()
    {
        for (int i = 0; i < rectMovers.Length; i++)
            rectMovers[i].Hide().Forget();

        canvasFader.Hide().Forget();
    }
}
