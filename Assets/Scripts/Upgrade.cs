using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public string id;
    public new string name;
    [TextArea]
    public string description;
    public Sprite icon;
    public int minLevel;
}