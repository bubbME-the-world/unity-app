using UnityEngine;

public class AnteenaStateChanger : MonoBehaviour
{
    [SerializeField] ChannelAnteenaState channel;
    [SerializeField] AnteenaState state;

    public void ChangeState()
    {
        channel.Current.Value = state;
    }
}
