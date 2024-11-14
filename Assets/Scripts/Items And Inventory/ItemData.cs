using UnityEngine;

public enum ItemType
{
    Bless,
    Keys,
    Creation,
    Fragment
}

[CreateAssetMenu(fileName ="New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;

    [Range(0, 100)]
    public float dropChance;
}
