using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Game/Reactive/Room")]
public class ChannelRoom : ScriptableObject
{
    public AsyncReactiveProperty<RoomState> Current = new(new());

    private void OnEnable() => Current = new(new());

    private void OnDisable() => Current.Dispose();
}