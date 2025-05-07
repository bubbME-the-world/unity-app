using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] ChannelBoolean channelTransition;
    [SerializeField] ChannelBoolean actionState;
    [SerializeField] ChannelRoom channelRoom;
    [SerializeField] RoomObject[] roomObjects;
    [SerializeField] RectMover[] movers;
    [SerializeField] RectMover[] chatMovers;

    bool inAction;

    private readonly CancellationTokenSource cts = new();

    private void OnDestroy() => StopObserve();

    private void Start()
    {
        actionState.Current.Subscribe(value => inAction = value).AddTo(cts.Token);

        StartObserve();
    }

    public void StartObserve() => ObserveRoom(cts.Token).Forget();

    public void StopObserve()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    private async UniTaskVoid ObserveRoom(CancellationToken token)
    {
        await UniTask.Yield();
        await UniTask.WaitForEndOfFrame();

        while (!token.IsCancellationRequested)
        {
            var room = await channelRoom.Current.WaitAsync();

            if (room == RoomState.CHAT)
                ShowChat().Forget();
            else
                ShowRoom(room).Forget();
        }
    }

    private async UniTask ShowChat()
    {
        if (inAction)
            return;

        await HideTransition();

        HideAll();

        GetRoom(RoomState.MIDDLE).Object.SetActive(true);

        await channelTransition.SetValueAsync(false);
    }

    private async UniTask ShowRoom(RoomState room)
    {
        if (inAction)
            return;

        if (GetRoom(room).Object == null)
            return;

        await HideTransition();

        HideAll();

        GetRoom(room).Object.SetActive(true);

        await ShowTransition();
    }

    async UniTask HideTransition()
    {
        UniTask[] hideTasks = new UniTask[movers.Length + 1];

        for (int i = 0; i < movers.Length; i++)
            hideTasks[i] = movers[i].Hide();

        hideTasks[^1] = channelTransition.SetValueAsync(true);

        await UniTask.WhenAll(hideTasks);
    }

    async UniTask ShowTransition()
    {
        UniTask[] showTasks = new UniTask[movers.Length];

        for (int i = 0; i < movers.Length; i++)
            showTasks[i] = movers[i].Show();

        showTasks[^1] = channelTransition.SetValueAsync(false);

        await UniTask.WhenAll(showTasks);
    }

    RoomObject GetRoom(RoomState roomState)
    {
        for (int i = 0; i < roomObjects.Length; i++)
            if (roomObjects[i].Room == roomState)
                return roomObjects[i];

        return new();
    }

    void HideAll()
    {
        for (int i = 0; i < roomObjects.Length; i++)
            roomObjects[i].Object.SetActive(false);
    }
}