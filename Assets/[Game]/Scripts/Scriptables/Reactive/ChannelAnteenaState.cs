using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Anteena State", menuName = "Game/Reactive/Anteena State")]
public class ChannelAnteenaState : ScriptableObject
{
    public AsyncReactiveProperty<AnteenaState> Current = new (AnteenaState.NORMAL);

    // Task completion source to await subscriber completion
    private UniTaskCompletionSource taskCompletionSource;

    // Set a value and await until subscribers are done processing
    public async UniTask SetValueAsync(AnteenaState value)
    {
        // Create a new task completion source
        taskCompletionSource = new UniTaskCompletionSource();

        // Trigger the change
        Current.Value = value;

        // Wait until the task is completed by subscribers
        await taskCompletionSource.Task;
    }

    // Mark the event as completed
    public void Complete() => taskCompletionSource?.TrySetResult();

    private void OnEnable() => Current = new(AnteenaState.NORMAL);

    private void OnDisable() => Current.Dispose();
}
