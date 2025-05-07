using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodUI : MonoBehaviour
{
    public event Func<UserInventory, UniTask> OnItemUse;

    [SerializeField] Image foodImage;
    [SerializeField] Button useButton;
    [SerializeField] TMP_Text amount;

    UserInventory currentInventory;
    int currentAmount = 0;

    void Start()
    {
        useButton.OnClickAsAsyncEnumerable().Subscribe(async x =>
        {
            useButton.interactable = false;

            if (OnItemUse != null)
            {
                foreach (var subscriber in OnItemUse.GetInvocationList())
                {
                    var handler = (Func<UserInventory, UniTask>)subscriber;
                    await handler.Invoke(currentInventory);
                }
            }

            useButton.interactable = true;
        });
    }

    public void SetFood(Sprite sprite, UserInventory inventory)
    {
        foodImage.sprite = sprite;
        currentAmount = 1;

        currentInventory = inventory;
        amount.text = currentAmount.ToString();
    }

    public void AddAmount()
    {
        currentAmount++;
        amount.text = currentAmount.ToString();
    }
}