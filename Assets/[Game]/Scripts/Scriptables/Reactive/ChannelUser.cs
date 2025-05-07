using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "User", menuName = "Game/Reactive/User")]
public class ChannelUser : ScriptableObject
{
    public AsyncReactiveProperty<User> Current = new (new ());

    private void OnEnable() => Current = new(new());

    private void OnDisable() => Current.Dispose();

    public UserStatus GetStatusUser(StatusType type)
    {
        for (int i = 0; i < Current.Value.Status.Length; i++)
            if (Current.Value.Status[i].Key == type)
                return Current.Value.Status[i];

        return new();
    }
}