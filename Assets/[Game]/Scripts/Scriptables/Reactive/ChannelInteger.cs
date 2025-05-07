using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Reactive Integer", menuName = "Game/Reactive/Integer")]
public class ChannelInteger : ScriptableObject
{
    public AsyncReactiveProperty<int> Current = new(-1);

    private void OnEnable() => Current = new(-1);

    private void OnDisable() => Current.Dispose();
}