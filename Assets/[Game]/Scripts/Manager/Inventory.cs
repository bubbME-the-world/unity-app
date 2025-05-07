using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Lean.Pool;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] CanvasFader fader;
    [SerializeField] LeanGameObjectPool pool;
    [SerializeField] Transform itemParent;
    
    [SerializeField] ChannelInventory inventory;
    [SerializeField] ChannelBoolean ActionState;
    [SerializeField] ChannelInteger useInventoryID;
    [SerializeField] ChannelInteger useItemID;

    [SerializeField] ChannelAnteenaState state;
    [SerializeField] ItemConfig itemConfig;

    private readonly Dictionary<int, FoodUI> foodDict = new();
    private readonly CancellationTokenSource cts = new();
    private bool inAction;

    private void Start()
    {
        ActionState.Current.WithoutCurrent().Subscribe(state => inAction = state).AddTo(cts.Token);
    }

    public void Show()
    {
        if (inAction)
            return;

        ClearFood();

        for (int i = 0; i < inventory.Current.Value.Length; i++)
        {
            if (foodDict.ContainsKey(inventory.Current.Value[i].ItemID))
            {
                foodDict[inventory.Current.Value[i].ItemID].AddAmount();
                continue;
            }
            else
            {
                GameObject foodObject = pool.Spawn(itemParent);
                FoodUI food = foodObject.GetComponent<FoodUI>();

                UserInventory inv = inventory.Current.Value[i];

                food.SetFood(itemConfig.GetSprite(inv.ItemID), inv);
                food.OnItemUse += UseItem;

                foodDict.Add(inv.ItemID, food);
            }
        }

        fader.Show().Forget();
    }

    public async UniTask UseItem(UserInventory inventory)
    {
        await fader.Hide();

        useInventoryID.Current.Value = inventory.ID;
        useItemID.Current.Value = inventory.ItemID;

        await state.SetValueAsync(AnteenaState.EATING);

        await UniTask.Yield();
    }

    void ClearFood()
    {
        foreach(var foodUI in foodDict.Values)
            foodUI.OnItemUse -= UseItem;

        pool.DespawnAll();
        foodDict.Clear();
    }
}