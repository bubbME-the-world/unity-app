using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Supabase Config", menuName = "Game/Supabase Config")]
public class SupabaseConfig : ScriptableObject
{
    public string BaseURL;
    public string ConfigPath;
    public string MiniGamePath;
    public string UserPath;
}