using UnityEngine;

public class RoomChanger:MonoBehaviour
{
    [SerializeField] ChannelRoom room;
    [SerializeField] RoomState roomState;

    public void ChangeRoom()
    {
        room.Current.Value = roomState;
    }
}