using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Anteena.Home.Animation
{
    public class AnteenaAnimationBase : MonoBehaviour
    {
        public virtual async UniTask Animate()
        {
            await UniTask.WaitForEndOfFrame();
        }
    }
}