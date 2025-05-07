using Cysharp.Threading.Tasks;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    [SerializeField] ConfigDataService configDataService;
    [SerializeField] ChannelConfig config;
    [SerializeField] StatusConfig statusConfig;

    public async UniTask LoadConfig()
    {
        config.Current.Value = await configDataService.GetConfig();

        for (int i = 0; i < config.Current.Value.Length; i++)
        {
            statusConfig.SetIncrement(config.Current.Value[i].Type, config.Current.Value[i].Value);
            statusConfig.SetDecrement(config.Current.Value[i].Type, config.Current.Value[i].Value);
        }
    }
}