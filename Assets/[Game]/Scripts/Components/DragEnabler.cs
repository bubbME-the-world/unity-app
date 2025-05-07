using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using UnityEngine;

public class DragEnabler : MonoBehaviour
{
    [SerializeField] ChannelBoolean drag;

    private readonly CancellationTokenSource cts = new();

    private void Start()
    {
        drag.Current.Subscribe(value => DragHandler(value)).AddTo(cts.Token);
    }

    void DragHandler(bool value)
    {
        gameObject.SetActive(value);
    }

    private void OnDestroy()
    {
        cts.Cancel();
        cts.Dispose();
    }
}