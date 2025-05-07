using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Anteena.Home.Animation
{
    public class ShowerAnimation : AnteenaAnimationBase
    {
        [SerializeField] AnteenaSpriteManager spriteManager;

        public override async UniTask Animate()
        {
            await spriteManager.ChangeSprite(AnteenaState.HAPPY);
            await UniTask.Delay(2000);
            await spriteManager.ChangeSprite(AnteenaState.NORMAL);
        }
    }
}