using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class AnteenaSpriteManager : MonoBehaviour
{
    [SerializeField] float scaleX = 0.1f;
    [SerializeField] float scaleY = 1.1f;
    [SerializeField] float duration;
    [SerializeField] SpriteConfig anteenaStates;

    Transform trans;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        trans = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public async UniTask ChangeSprite(AnteenaState state)
    {
        Sprite sprite = anteenaStates.GetSprite(state);

        if (sprite == null)
            return;

        Vector3 originalScale = trans.localScale;

        await UniTask.WhenAll(
                trans.DOScaleY(scaleY, duration).SetEase(Ease.InBack).ToUniTask(),
                trans.DOScaleX(scaleX, duration).SetEase(Ease.InBack).ToUniTask()
            );

        spriteRenderer.sprite = sprite;

        await UniTask.WhenAll(
            trans.DOScaleY(originalScale.y, duration).SetEase(Ease.OutBack).ToUniTask(),
            trans.DOScaleX(originalScale.x, duration).SetEase(Ease.OutBack).ToUniTask()
        );

        trans.localScale = originalScale;
    }
}
