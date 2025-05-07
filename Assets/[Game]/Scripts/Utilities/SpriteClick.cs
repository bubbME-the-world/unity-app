using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SpriteClick : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent OnSpriteClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnSpriteClick.Invoke();
    }
}
