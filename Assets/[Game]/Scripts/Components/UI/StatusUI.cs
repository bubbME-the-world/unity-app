using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    [SerializeField] StatusType statusType;
    [SerializeField] ChannelUser channelUser;
    [SerializeField] ChannelConfig channelConfig;
    [SerializeField] Image fillBar;

    private readonly CancellationTokenSource cts = new();
    private Tweener tweener;

    float maxValue;
    bool inited;

    private void OnDestroy()
    {
        StopObserve();
    }

    private void Start()
    {
        StartObserve();
    }

    public void StartObserve()
    {
        ObserveState(cts.Token).Forget();
    }

    public void StopObserve()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    private async UniTaskVoid ObserveState(CancellationToken token)
    {
        await UniTask.Yield();
        await UniTask.WaitForEndOfFrame();

        while (!token.IsCancellationRequested)
        {
            // Wait for the user change
            if (!inited)
            {
                await channelConfig.Current.WaitAsync();
                maxValue = channelConfig.GetConfig(ConfigType.MAX_STATUS_VALUE).Value * 1.0f;

                inited = true;
            }

            var user = await channelUser.Current.WaitAsync();

            // React to the user change
            UpdateStatusUI(user);
        }
    }

    private void UpdateStatusUI(User user)
    {
        for (var i = 0; i < user.Status.Length; i++)
        {
            if (user.Status[i].Key == statusType)
            {
                float newValue = user.Status[i].Value * 1.0f / maxValue;

                if (newValue > fillBar.fillAmount)
                {
                    tweener?.Kill();
                    tweener = fillBar.DOFillAmount(newValue, 0.25f);
                }
                else
                {
                    fillBar.fillAmount = newValue;
                }
            }
        }
    }
}
