using UnityEngine;

[System.Serializable]
public struct SpriteConfigData
{
    public AnteenaState State;
    public Sprite Sprite;
}

[CreateAssetMenu(fileName = "new Sprite Config", menuName = "Game/Sprite Config")]
public class SpriteConfig : ScriptableObject
{
    public SpriteConfigData[] Sprites;

    public Sprite GetSprite(AnteenaState state)
    {
        for (int i = 0; i < Sprites.Length; i++)
            if(Sprites[i].State == state)
                return Sprites[i].Sprite;

        return null;
    }
}
