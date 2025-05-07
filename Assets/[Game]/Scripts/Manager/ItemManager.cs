using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] ItemDataService itemDataService;
    [SerializeField] ChannelItem item;

    public async UniTask LoadItem()
    {
        item.Current.Value = await itemDataService.GetItem();
    }
}