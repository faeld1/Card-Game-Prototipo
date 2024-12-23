using UnityEngine;

[System.Serializable]
public class Equipment
{
    public EquipmentType equipmentType;
    public string equipmentName;
    public Sprite equipmentIcon;
    public int level;
    public Rarity rarity;
    public int stars;

    // Atualiza as estrelas e a raridade
    public void AddStars(int starCount)
    {
        stars += starCount;

        if (stars >= 5)
        {
            stars = 0; // Reseta as estrelas
            UpgradeRarity();
        }
    }

    public void UpgradeRarity()
    {
        if (rarity < Rarity.Divine)
        {
            rarity++;
            Debug.Log($"Raridade evoluiu para {rarity}!");
        }
    }
}

public enum EquipmentType
{
    Gloves,
    Boots,
    Pants,
    Armor,
    Helmet,
    Weapon,
    Wings
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Divine
}
