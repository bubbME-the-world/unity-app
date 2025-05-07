using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemUI : MonoBehaviour
{
    public event Func<Item, UniTask> OnItemBuy;

    [SerializeField] ChannelItem item;
    [SerializeField] Image itemImage;
    [SerializeField] Button buyButton;
    [SerializeField] TMP_Text price;

    Item currentItem = new();

    void Start()
    {
        buyButton.OnClickAsAsyncEnumerable().Subscribe(async x =>
        {
            buyButton.interactable = false;

            if (OnItemBuy != null)
            {
                foreach (var subscriber in OnItemBuy.GetInvocationList())
                {
                    var handler = (Func<Item, UniTask>)subscriber;
                    await handler.Invoke(currentItem);
                }
            }

            buyButton.interactable = true;
        });
    }

    public void SetItem(Sprite sprite, Item item)
    {
        itemImage.sprite = sprite;
        currentItem = item;
        price.text = currentItem.Price.ToString();
    }
}