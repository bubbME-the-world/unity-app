using Anteena.Home.Animation;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Anteena.Home
{
    public class EatAction : AnteenaActionBase
    {
        AnteenaActionManager actionManager;

        public EatAction(
            AnteenaActionManager actionManager,
            AnteenaAnimationBase animation,
            UserManager userManager)
            : base(actionManager, animation, userManager)
        {
            this.actionManager = actionManager;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entering Eat action.");
            await UniTask.Yield();
        }

        public override async UniTask Execute()
        {
            await animation.Animate();
            
            userManager.IncreaseStatus(StatusType.HUNGER);

            Debug.Log("Eating...");

            await UniTask.Yield();

            actionManager.CompleteAction();
        }

        public override async UniTask Exit()
        {
            Debug.Log("Exiting Eat action.");
            await UniTask.Yield();
        }
    }
}