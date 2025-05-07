using UnityEngine;

[System.Serializable]
public struct ItemConfigData
{
    public int ID;
    public Sprite Sprite;
}

[CreateAssetMenu(fileName = "new Food Config", menuName = "Game/Food Config")]
public class ItemConfig : ScriptableObject
{
    public ItemConfigData[] Foods;

    public Sprite GetSprite(int id)
    {
        for (int i = 0; i < Foods.Length; i++)
            if (Foods[i].ID == id)
                return Foods[i].Sprite;

        return null;
    }
}