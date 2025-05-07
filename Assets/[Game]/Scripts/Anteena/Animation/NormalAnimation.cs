using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Anteena.Home.Animation
{
    public class NormalAnimation : AnteenaAnimationBase
    {
        [SerializeField] AnteenaSpriteManager spriteManager;

        public override async UniTask Animate()
        {
            await spriteManager.ChangeSprite(AnteenaState.NORMAL);
        }
    }
}