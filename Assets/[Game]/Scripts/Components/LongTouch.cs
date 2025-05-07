using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongTouchUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float longTouchDuration = 1.0f; // Duration in seconds to register as a long touch
    [SerializeField] CanvasFader canvasFader;
    [SerializeField] RectMover[] rectMovers;

    private bool isTouching = false; // Tracks if the user is still holding down
    private bool longTouchTriggered = false; // Prevents multiple triggers on one touch

    public async void OnPointerDown(PointerEventData eventData)
    {
        // Start tracking touch
        isTouching = true;
        longTouchTriggered = false;

        // Optionally show the progress bar filling up
        float elapsedTime = 0f;

        while (isTouching && elapsedTime < longTouchDuration)
        {
            elapsedTime += Time.deltaTime;

            await UniTask.Yield(); // Yield control back to the main thread to avoid blocking
        }

        // Check if the touch was held long enough
        if (isTouching && elapsedTime >= longTouchDuration && !longTouchTriggered)
        {
            longTouchTriggered = true; // Prevent duplicate triggers
            OnLongTouch().Forget();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouching = false; // Stop tracking touch
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isTouching = false; // Stop tracking touch if the pointer exits the element
    }

    private async UniTask OnLongTouch()
    {
        isTouching = false;
        longTouchTriggered = true;

        await canvasFader.Show();

        for (int i = 0; i < rectMovers.Length; i++)
            rectMovers[i].Show().Forget();
    }

    public void Hide()
    {
        for (int i = 0; i < rectMovers.Length; i++)
            rectMovers[i].Hide().Forget();

        canvasFader.Hide().Forget();
    }
}