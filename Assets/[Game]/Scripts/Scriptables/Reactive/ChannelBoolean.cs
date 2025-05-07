using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reactive Boolean", menuName = "Game/Reactive/Boolean")]
public class ChannelBoolean : ScriptableObject
{
    [SerializeField] private bool defaultValue;

    public AsyncReactiveProperty<bool> Current = new(false);

    // Task completion source to await subscriber completion
    private UniTaskCompletionSource<bool> taskCompletionSource;

    // Counter for active subscribers
    private int activeSubscribers;

    // Set a value and await until subscribers are done processing
    public async UniTask<bool> SetValueAsync(bool value)
    {
        // Create a new task completion source
        taskCompletionSource = new UniTaskCompletionSource<bool>();

        // Reset subscriber counter
        activeSubscribers = 0;

        // Trigger the change
        Current.Value = value;

        // Wait until the task is completed by subscribers
        return await taskCompletionSource.Task;
    }

    // Mark a subscriber as completed
    public void SubscriberCompleted()
    {
        activeSubscribers--;

        // Complete the task when all subscribers are done
        if (activeSubscribers <= 0)
        {
            activeSubscribers = 0;
            taskCompletionSource?.TrySetResult(true);
        }
    }

    // Call when a subscriber starts processing
    public void NotifySubscriber() => activeSubscribers++;

    private void OnEnable() => Current = new(defaultValue);

    private void OnDisable() => Current.Dispose();
}
