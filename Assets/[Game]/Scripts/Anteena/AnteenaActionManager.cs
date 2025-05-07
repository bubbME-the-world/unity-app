using Anteena.Home.Animation;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using UnityEngine;

namespace Anteena.Home
{
    public class AnteenaActionManager : MonoBehaviour
    {
        [SerializeField] UserManager userManager;
        [SerializeField] ChannelBoolean actionState;
        [SerializeField] ChannelAnteenaState channelState;

        [SerializeField] AnteenaAnimationBase eatAnimation;
        [SerializeField] AnteenaAnimationBase sleepAnimation;
        [SerializeField] AnteenaAnimationBase showerAnimation;
        [SerializeField] AnteenaAnimationBase normalAnimation;

        private AnteenaActionBase currentAction;
        private readonly CancellationTokenSource cts = new();

        bool isAction;

        private void OnDestroy()
        {
            StopObserve();
        }

        public void StopObserve()
        {
            cts?.Cancel();
            cts?.Dispose();
        }

        private void Start()
        {
            channelState.Current.Value = AnteenaState.NORMAL;
            channelState.Current.Subscribe(ChangeStateHandler).AddTo(cts.Token);
        }

        public void ChangeStateHandler(AnteenaState state)
        {
            if (isAction)
                return;

            actionState.Current.Value = isAction = true;

            switch (state)
            {
                case AnteenaState.SLEEPING:
                    ChangeAction(new SleepAction(this, sleepAnimation, userManager));
                    break;
                case AnteenaState.EATING:
                    ChangeAction(new EatAction(this, eatAnimation, userManager));
                    break;
                case AnteenaState.SHOWERING:
                    ChangeAction(new ShowerAction(this, showerAnimation, userManager));
                    break;
                default:
                    ChangeAction(new NormalAction(this, normalAnimation, userManager));
                    break;
            }
        }

        public async void ChangeAction(AnteenaActionBase newAction)
        {
            if (currentAction != null)
                await currentAction.Exit();

            currentAction = newAction;

            if (currentAction != null)
            {
                await currentAction.Enter();
                await currentAction.Execute();
            }
        }

        public void CompleteAction()
        {
            actionState.Current.Value = isAction = false;
            channelState.Complete();
        }
    }
}