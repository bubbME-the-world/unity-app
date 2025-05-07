using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using DG.Tweening;
using System.Threading;
using UnityEngine;

namespace Anteena.Home
{
    public class AnteenaPosition : MonoBehaviour
    {
        [SerializeField] ChannelAnteenaState state;
        [SerializeField] ChannelBoolean dragable;
        [SerializeField] ChannelBoolean dragging;
        [SerializeField] float normalScale;
        [SerializeField] float dragScale;
        [SerializeField] float actionScale;
        [SerializeField] float duration;

        private Transform trans, target;
        private Vector3 startPos;
        private bool onCrystal, onShower, isDragging = false;

        private CancellationTokenSource cts = new();

        Tweener scaleTweener;
        Sequence jumpSequence;

        private void OnDestroy()
        {
            cts.Cancel();
            cts.Dispose();
        }

        private void Start()
        {
            trans = transform;
            startPos = trans.position;

            scaleTweener.SetRecyclable(true);
            jumpSequence.SetRecyclable(true);

            dragging.Current.Subscribe(value => DragHandle(value)).AddTo(cts.Token);
        }

        async UniTaskVoid DragHandle(bool value)
        {
            dragging.NotifySubscriber();

            isDragging = value;

            if (isDragging)
                return;

            dragable.SetValueAsync(false).Forget();

            jumpSequence?.Kill();
            scaleTweener?.Kill();

            if (!target)
            {
                await ReturnToStart();
                return;
            }

            jumpSequence = trans.DOJump(target.position, duration, 1, 0.25f).SetAutoKill(false);

            await jumpSequence.Play().AsyncWaitForCompletion();

            if (onCrystal)
                await state.SetValueAsync(AnteenaState.SLEEPING);
            else if (onShower)
                await state.SetValueAsync(AnteenaState.SHOWERING);

            await ReturnToStart();

            dragging.SubscriberCompleted();
        }

        private async UniTask ReturnToStart()
        {
            onCrystal = false;
            onShower = false;
            target = null;

            scaleTweener = trans.DOScale(normalScale, duration);
            jumpSequence = trans.DOJump(startPos, duration, 1, 0.25f).SetAutoKill(false);

            await scaleTweener.AsyncWaitForCompletion();
            await jumpSequence.Play().AsyncWaitForCompletion();

            dragable.SetValueAsync(true).Forget();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isDragging)
                return;

            scaleTweener?.Kill();
            scaleTweener = trans.DOScale(actionScale, duration);

            onCrystal = collision.CompareTag("Crystal");
            onShower = collision.CompareTag("Shower");
            target = collision.gameObject.transform;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!isDragging)
                return;

            scaleTweener?.Kill(true);
            scaleTweener = trans.DOScale(dragScale, duration);

            onCrystal = false;
            onShower = false;
            target = null;
        }
    }
}
