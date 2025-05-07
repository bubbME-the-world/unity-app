using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Anteena.Home
{
    public class AnteenaTouch : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerExitHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [SerializeField] ChannelBoolean longPress;
        [SerializeField] ChannelBoolean dragable;
        [SerializeField] ChannelBoolean drag;

        [SerializeField] Vector3 positionOffset = Vector3.zero;
        [SerializeField] float longTouchDuration = 1.0f;

        private Vector3 clickOffset = Vector3.zero;
        private Vector3 tempVector = Vector3.zero;

        private float zAxis = 0;

        private Camera mainCamera;
        private Transform trans;

        private bool isPointerDown = false;
        private bool longPressTriggered = false;
        private bool isDrag;
        private bool isAbleToDrag;

        private UniTaskCompletionSource longTouchCompletionSource;

        private readonly CancellationTokenSource dragableCancelToken = new();

        private void Start()
        {
            mainCamera = Camera.main;
            trans = transform;

            if (longTouchDuration <= 0f)
                longTouchDuration = 0.5f;

            if (mainCamera.GetComponent<Physics2DRaycaster>() == null)
                mainCamera.gameObject.AddComponent<Physics2DRaycaster>();

            dragable.Current.Subscribe(CanDrag).AddTo(dragableCancelToken.Token);
        }

        public void CanDrag(bool value) => isAbleToDrag = value;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isAbleToDrag)
                return;

            zAxis = trans.position.z;
            clickOffset = trans.position - mainCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, zAxis)) + positionOffset;
            trans.position = new Vector3(trans.position.x, trans.position.y, zAxis);

            isDrag = true;
            drag.SetValueAsync(isDrag).Forget();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isAbleToDrag)
                return;

            tempVector = mainCamera.ScreenToWorldPoint(eventData.position) + clickOffset;
            tempVector.z = zAxis; // Ensure the z-axis remains unchanged
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isAbleToDrag)
                return;

            isDrag = false;
            drag.SetValueAsync(isDrag).Forget();
        }

        private void LateUpdate()
        {
            // Smoothly move the transform while dragging
            if (isDrag)
            {
                trans.position = Vector3.Lerp(trans.position, tempVector, Time.deltaTime * 10f);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            longPressTriggered = false;

            longPress.SetValueAsync(longPressTriggered).Forget();

            // Start long-touch detection asynchronously
            DetectLongTouch().Forget();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;

            // Reset the long-touch trigger and cancel the detection
            longPressTriggered = false;
            longTouchCompletionSource?.TrySetCanceled();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerDown = false;

            // Reset the long-touch trigger and cancel the detection
            longPressTriggered = false;
            longTouchCompletionSource?.TrySetCanceled();
        }

        private async UniTask DetectLongTouch()
        {
            // Reset the completion source for new long-touch detection
            longTouchCompletionSource = new UniTaskCompletionSource();

            // Await the long touch duration or pointer release/cancel
            await UniTask.WhenAny(
                UniTask.Delay((int)(longTouchDuration * 1000)), // Delay for the long touch duration
                longTouchCompletionSource.Task
            );

            if (isPointerDown && !longPressTriggered && !isDrag)
            {
                longPressTriggered = true;
                longPress.SetValueAsync(longPressTriggered).Forget();
            }
        }
    }
}