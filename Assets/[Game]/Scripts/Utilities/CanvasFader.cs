using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasFader : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] float delay;

    CanvasGroup canvasGroup;
    Tweener tweener;

    private void Awake() => canvasGroup = GetComponent<CanvasGroup>();

    public void SyncShow()
    {
        Show().Forget();
    }

    public void SyncHide()
    {
        Hide().Forget();
    }

    public async UniTask Show()
    {
        tweener?.Kill();

        tweener = canvasGroup.DOFade(1f, duration).SetDelay(delay);
        await tweener.AsyncWaitForCompletion();

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public async UniTask Hide()
    {
        tweener?.Kill();

        tweener = canvasGroup.DOFade(0f, duration).SetDelay(delay);
        await tweener.AsyncWaitForCompletion();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void SetInteracable(bool interacable) => canvasGroup.interactable = interacable;
}
