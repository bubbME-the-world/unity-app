using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class RectMover : MonoBehaviour
{
    [SerializeField] Vector2 startPos;
    [SerializeField] float duration;
    [SerializeField] float delay;
    [SerializeField] Ease ease;

    Vector2 endPos;

    RectTransform rectTransform;
    Tweener tweener;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        endPos = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        rectTransform.anchoredPosition = startPos;
    }

    public async UniTask Show()
    {
        tweener?.Kill();

        tweener = rectTransform.DOAnchorPos(endPos, duration).SetDelay(delay).SetEase(ease);
        await tweener.AsyncWaitForCompletion();
    }

    public async UniTask Hide()
    {
        tweener?.Kill();

        tweener = rectTransform.DOAnchorPos(startPos, duration).SetDelay(delay).SetEase(ease);
        await tweener.AsyncWaitForCompletion();
    }
}
