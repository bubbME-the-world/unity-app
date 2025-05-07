using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace Anteena.Home.Animation
{
    public class EatAnimation : AnteenaAnimationBase
    {
        [SerializeField] AnteenaSpriteManager spriteManager;
        [SerializeField] SpriteRenderer foodRenderer;
        [SerializeField] ItemConfig itemConfig;
        [SerializeField] ChannelInteger removeItemID;
        
        [SerializeField] Transform food;
        [SerializeField] Transform star;
        
        [SerializeField] float yPos;
        [SerializeField] float moveDuration;
        [SerializeField] float scaleDuration;

        Vector3 startPos;
        Vector3 starScale;
        Vector3 foodScale;

        private readonly CancellationTokenSource cts = new();

        private void Awake()
        {
            startPos = star.position;

            starScale = star.localScale;
            foodScale = food.localScale;

            foodRenderer.gameObject.SetActive(false);
            star.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            cts.Cancel();
            cts.Dispose();
        }

        private void Start()
        {
            removeItemID.Current.WithoutCurrent().Subscribe(itemID =>
            {
                foodRenderer.sprite = itemConfig.GetSprite(itemID);
            }).AddTo(cts.Token);
        }

        public override async UniTask Animate()
        {
            await spriteManager.ChangeSprite(AnteenaState.EATING);

            food.localScale = new Vector3(0.01f, 0.1f, 1f);
            food.gameObject.SetActive(true);

            await food.DOScale(foodScale, scaleDuration).SetEase(Ease.OutBack).AsyncWaitForCompletion();

            await UniTask.Delay(500);

            star.localScale = new Vector3(0.01f, 0.1f, 1f);
            star.gameObject.SetActive(true);

            await star.DOScale(starScale, scaleDuration).SetEase(Ease.OutBack).AsyncWaitForCompletion();
            await star.DOMoveY(yPos, moveDuration).AsyncWaitForCompletion();
            await star.DOScale(0.1f, scaleDuration).SetEase(Ease.InBack).AsyncWaitForCompletion();

            star.gameObject.SetActive(false);
            star.position = startPos;
            star.localScale = starScale;

            await UniTask.Delay(500);

            await food.DOScale(0.01f, scaleDuration).SetEase(Ease.InBack).AsyncWaitForCompletion();

            food.gameObject.SetActive(false);
            food.localScale = foodScale;
            
            await spriteManager.ChangeSprite(AnteenaState.NORMAL);
        }
    }
}