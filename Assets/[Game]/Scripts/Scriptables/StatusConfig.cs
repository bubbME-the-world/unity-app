using UnityEngine;

[CreateAssetMenu(fileName = "Status Config", menuName = "Game/Status Config")]
public class StatusConfig : ScriptableObject
{
    [Header("PreGenerated data, but the value will be dynamic from server")]
    [SerializeField] private StatusData[] statusData;

    public void SetIncrement(ConfigType configType, int value)
    {
        for (int i = 0; i < statusData.Length; i++)
        {
            if (statusData[i].Increment.Type != configType)
                continue;

            statusData[i].Increment.Value = value;
            break;
        }
    }

    public void SetDecrement(ConfigType configType, int value)
    {
        for (int i = 0; i < statusData.Length; i++)
        {
            if (statusData[i].Decrement.Type != configType)
                continue;

            statusData[i].Decrement.Value = value;
            break;
        }
    }

    public StatusModifier GetIncrement(StatusType statusType)
    {
        for (int i = 0; i < statusData.Length; i++)
        {
            if (statusData[i].Type != statusType)
                continue;

            return statusData[i].Increment;
        }

        return new();
    }

    public StatusModifier GetDecrement(StatusType statusType)
    {
        for (int i = 0; i < statusData.Length; i++)
        {
            if (statusData[i].Type != statusType)
                continue;

            return statusData[i].Decrement;
        }

        return new();
    }
}