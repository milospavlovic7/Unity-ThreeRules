using UnityEngine;

public enum ItemType
{
    Active,
    Passive
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite sprite;

    public ItemType itemType = ItemType.Passive;
    [Tooltip("ScriptableObject that performs effect logic")]
    public ItemEffect effect;
}
