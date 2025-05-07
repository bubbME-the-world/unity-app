using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Anteena.Home.Animation
{
    public class SleepAnimation : AnteenaAnimationBase
    {
        [SerializeField] AnteenaSpriteManager spriteManager;

        public override async UniTask Animate()
        {
            await spriteManager.ChangeSprite(AnteenaState.SLEEPING);
            await UniTask.Delay(2000);
            await spriteManager.ChangeSprite(AnteenaState.NORMAL);
        }
    }
}