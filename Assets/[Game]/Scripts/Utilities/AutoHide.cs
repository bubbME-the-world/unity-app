using Cysharp.Threading.Tasks;
using UnityEngine;

public class AutoHide : MonoBehaviour
{
    [SerializeField] CanvasFader fader;
    [SerializeField] float delay;

    public void DelayedHide()
    {
        Invoke(nameof(Hide), delay);
    }

    void Hide()
    {
        fader.Hide().Forget();
    }

}
