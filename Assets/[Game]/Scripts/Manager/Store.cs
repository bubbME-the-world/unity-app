using Cysharp.Threading.Tasks;
using Lean.Pool;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] CanvasFader storeFader;
    [SerializeField] LeanGameObjectPool pool;
    [SerializeField] Transform itemParent;
    [SerializeField] ItemConfig itemConfig;
    [SerializeField] ChannelUser user;
    [SerializeField] ChannelItem item;
    [SerializeField] ChannelInteger buyItemID;

    List<StoreItemUI> storeItems = new List<StoreItemUI>();

    public void Show()
    {
        ClearStoreItem();

        for (int i = 0; i < item.Current.Value.Length; i++)
        {
            GameObject storeObject = pool.Spawn(itemParent);
            StoreItemUI storeItem = storeObject.GetComponent<StoreItemUI>();

            var itemData = item.Current.Value[i];

            storeItem.SetItem(itemConfig.GetSprite(itemData.ID), itemData);
            storeItem.OnItemBuy += BuyItem;

            storeItems.Add(storeItem);
        }

        storeFader.Show().Forget();
    }

    public async UniTask BuyItem(Item item)
    {
        buyItemID.Current.Value = item.ID;
        await UniTask.Yield();
    }

    void ClearStoreItem()
    {
        for (int i = 0; i < storeItems.Count; i++)
            storeItems[i].OnItemBuy -= BuyItem;

        pool.DespawnAll();
        storeItems.Clear();
    }
}