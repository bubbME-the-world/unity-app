using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    [SerializeField] UserDataService userDataService;
    [SerializeField] StatusConfig statusConfig;

    [SerializeField] ChannelConfig config;
    [SerializeField] ChannelUser user;
    [SerializeField] ChannelInventory inventory;

    [SerializeField] ChannelInteger removeInventoryID;
    [SerializeField] ChannelInteger addItemID;

    private readonly CancellationTokenSource cts = new();
    private List<UserInventory> userInventories = new();

    public async UniTask LoadUser(string userID)
    {
        await LoadUserData(userID);
        await LoadUserItem(userID);

        removeInventoryID.Current.WithoutCurrent().Subscribe(RemoveItem).AddTo(cts.Token);
        addItemID.Current.WithoutCurrent().SubscribeAwait(AddItem).AddTo(cts.Token);

        PeriodicStatusUpdate().Forget();
    }

    private async UniTask LoadUserData(string userID)
    {
        user.Current.Value = await userDataService.GetUser(userID, config.GetConfig(ConfigType.MAX_STATUS_VALUE).Value);

        DateTime lastUpdate = DateTimeOffset.Parse(user.Current.Value.LastUpdate).UtcDateTime;
        int totalSeconds = Mathf.RoundToInt((float)DateTime.UtcNow.Subtract(lastUpdate).TotalSeconds);

        DecreaseStatuses(totalSeconds);
    }

    private async UniTask LoadUserItem(string userID)
    {
        inventory.Current.Value = await userDataService.GetUserItem(userID);
        userInventories = new(inventory.Current.Value);
    }

    private async UniTaskVoid PeriodicStatusUpdate()
    {
        while (!cts.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            DecreaseStatuses(1);
        }
    }

    private void DecreaseStatuses(int seconds)
    {
        User tempUser = user.Current.Value;

        for (int i = 0; i < tempUser.Status.Length; i++)
        {
            int decreaseRate = statusConfig.GetDecrement(tempUser.Status[i].Key).Value * seconds;
            int currentValue = tempUser.Status[i].Value;

            currentValue -= decreaseRate;

            tempUser.Status[i].Value = Mathf.Clamp(currentValue, 0, config.GetConfig(ConfigType.MAX_STATUS_VALUE).Value);
        }

        user.Current.Value = tempUser;
    }

    public void IncreaseStatus(StatusType statusType)
    {
        User tempUser = user.Current.Value;

        for (int i = 0; i < tempUser.Status.Length; i++)
        {
            if (tempUser.Status[i].Key != statusType)
                continue;

            var amount = statusConfig.GetIncrement(tempUser.Status[i].Key).Value;
            tempUser.Status[i].Value += Mathf.Clamp(amount, 0, config.GetConfig(ConfigType.MAX_STATUS_VALUE).Value);
            break;
        }

        for (int i = 0; i < tempUser.Status.Length; i++)
            userDataService.UpdateUserAndStatus(tempUser.UserID, tempUser.Status[i]).Forget();

        user.Current.Value = tempUser;
    }

    private void RemoveItem(int inventoryID)
    {
        userInventories.RemoveAll(item => item.ID == inventoryID);
        userDataService.RemoveItem(inventoryID).Forget();

        inventory.Current.Value = userInventories.ToArray();
    }

    private async UniTask AddItem(int itemID)
    {
        UserInventory newInventory = await userDataService.AddItem(user.Current.Value.UserID, itemID);

        userInventories.Add(newInventory);

        inventory.Current.Value = userInventories.ToArray();
    }

    private void DisposeAll()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    private void OnDestroy()
    {
        DisposeAll();
    }
}