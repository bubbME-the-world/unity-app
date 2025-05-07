using Anteena.Home.Animation;
using Cysharp.Threading.Tasks;

namespace Anteena.Home
{
    public abstract class AnteenaActionBase
    {
        protected AnteenaActionManager stateManager;
        protected AnteenaAnimationBase animation;
        protected UserManager userManager;

        // Constructor
        public AnteenaActionBase(AnteenaActionManager stateManager, AnteenaAnimationBase animation, UserManager userManager)
        {
            this.stateManager = stateManager;
            this.animation = animation;
            this.userManager = userManager;
        }

        // Methods to override in derived classes
        public abstract UniTask Enter(); // Called when entering the state
        public abstract UniTask Execute(); // Called while in the state
        public abstract UniTask Exit(); // Called when exiting the state
    }
}
