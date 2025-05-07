using DG.Tweening;
using UnityEngine;

public class FloatTweenLoop : MonoBehaviour
{
    [SerializeField] float endValue;
    [SerializeField] float duration;
    [SerializeField] bool autoStart;
    [SerializeField] Ease ease;

    Transform trans;
    Tweener tweener;

    private void Awake()
    {
        trans = transform;
    }

    private void Start()
    {
        if (autoStart)
            Floating(true);
    }

    public void Floating(bool value)
    {
        tweener?.Kill(true);

        if (!value)
            return;    

        tweener = trans
            .DOLocalMoveY(endValue, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(ease);
    }
}
