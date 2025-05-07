using Anteena.Home.Animation;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Anteena.Home
{
    public class ShowerAction : AnteenaActionBase
    {
        AnteenaActionManager actionManager;

        public ShowerAction(
            AnteenaActionManager actionManager,
            AnteenaAnimationBase animation,
            UserManager userManager)
            : base(actionManager, animation, userManager)
        {
            this.actionManager = actionManager;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entering Shower action.");
            // Start eating animation or setup
            await UniTask.Yield(); // Simulate async setup if necessary
        }

        public override async UniTask Execute()
        {
            await animation.Animate();

            Debug.Log("Showering...");
            
            userManager.IncreaseStatus(StatusType.HYGIENE);
            await UniTask.Yield();

            actionManager.CompleteAction();
        }

        public override async UniTask Exit()
        {
            Debug.Log("Exiting Shower action.");
            // Cleanup eating animation or logic
            await UniTask.Yield(); // Simulate async cleanup if necessary
        }
    }
}