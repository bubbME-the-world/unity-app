using Anteena.Home.Animation;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Anteena.Home
{
    public class SleepAction : AnteenaActionBase
    {
        AnteenaActionManager actionManager;

        public SleepAction(
            AnteenaActionManager actionManager,
            AnteenaAnimationBase animation,
            UserManager userManager)
            : base(actionManager, animation, userManager)
        {
            this.actionManager = actionManager;
        }
        public override async UniTask Enter()
        {
            Debug.Log("Entering Sleep action.");
            // Start eating animation or setup
            await UniTask.Yield(); // Simulate async setup if necessary
        }

        public override async UniTask Execute()
        {
            await animation.Animate();

            Debug.Log("Sleeping...");

            userManager.IncreaseStatus(StatusType.ENERGY);
            await UniTask.Yield();

            actionManager.CompleteAction();
        }

        public override async UniTask Exit()
        {
            Debug.Log("Exiting Sleep action.");
            // Cleanup eating animation or logic
            await UniTask.Yield(); // Simulate async cleanup if necessary
        }
    }
}