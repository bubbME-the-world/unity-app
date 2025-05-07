using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameUI : MonoBehaviour
{
    public event Func<MiniGame, UniTask> OnSelect;

    [SerializeField] RawImage thumbnail;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text description;
    [SerializeField] Button playButton;

    MiniGame currentMiniGame;
    void Start()
    {
        playButton.OnClickAsAsyncEnumerable().Subscribe(async x =>
        {
            if (OnSelect != null)
            {
                foreach (var subscriber in OnSelect.GetInvocationList())
                {
                    var handler = (Func<MiniGame, UniTask>)subscriber;
                    await handler.Invoke(currentMiniGame);
                }
            }
        });
    }
    public void SetMiniGame(MiniGame miniGame)
    {
        title.text = miniGame.GameName;
        description.text = miniGame.GameDesc;
    }
}
