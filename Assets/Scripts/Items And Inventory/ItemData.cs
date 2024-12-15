using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Gold,
    Bless,
    BlessDust,
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

    public List<InventoryItem> craftingMaterials;

    [Range(0, 100)]
    public float dropChance;
}
