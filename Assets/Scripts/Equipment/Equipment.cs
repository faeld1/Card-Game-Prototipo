using UnityEngine;

[System.Serializable]
public class Equipment
{
    public EquipmentData equipmentData; // Dados base do equipamento
    public EquipmentType equipmentType;
    public string equipmentName;
    public Sprite equipmentIcon;
    public int level;
    public Rarity rarity;
    public int stars;

    [Header("Stats")]
    // Valores calculados com base no level e raridade
    public int MaxHPBonus => equipmentData.baseMaxHPBonus * CalculateStatMultiplier() * level;
    public int ArmorBonus => equipmentData.baseArmorBonus * CalculateStatMultiplier() * level;
    public int CritChanceBonus => equipmentData.baseCritChanceBonus * CalculateStatMultiplier() * level;
    public int CritPowerBonus => equipmentData.baseCritPowerBonus * CalculateStatMultiplier() * level;
    public int EvasionBonus => equipmentData.baseEvasionBonus * CalculateStatMultiplier() * level;
    public int DamageBonus => equipmentData.baseDamageBonus * CalculateStatMultiplier() * level;

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

    public void AddModifiers(PlayerStats playerStats)
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats está nulo ao aplicar modificadores!");
            return;
        }

        // Adiciona modificadores apenas se forem maiores que 0
        if (MaxHPBonus > 0) playerStats.maxHealth.AddModifiers(MaxHPBonus);
        if (ArmorBonus > 0) playerStats.armor.AddModifiers(ArmorBonus);
        if (CritChanceBonus > 0) playerStats.critChance.AddModifiers(CritChanceBonus);
        if (CritPowerBonus > 0) playerStats.critPower.AddModifiers(CritPowerBonus);
        if (EvasionBonus > 0) playerStats.evasion.AddModifiers(EvasionBonus);
        if (DamageBonus > 0) playerStats.damage.AddModifiers(DamageBonus);
    }

    public void RemoveModifiers(PlayerStats playerStats)
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats está nulo ao remover modificadores!");
            return;
        }

        playerStats.maxHealth.RemoveModifiers(MaxHPBonus);
        playerStats.armor.RemoveModifiers(ArmorBonus);
        playerStats.critChance.RemoveModifiers(CritChanceBonus);
        playerStats.critPower.RemoveModifiers(CritPowerBonus);
        playerStats.evasion.RemoveModifiers(EvasionBonus);
        playerStats.damage.RemoveModifiers(DamageBonus);
    }

    public void UpgradeRarity()
    {
        if (rarity < Rarity.Divine)
        {
            rarity++;
            Debug.Log($"Raridade evoluiu para {rarity}!");

            // Atualiza os stats com base na nova raridade
           // UpdateStatsForRarity();
        }
    }
    private int CalculateStatMultiplier()
    {
        return (int)rarity + 1; // Exemplo: Common = 1, Uncommon = 2, etc.
    }
    public int GetMaxLevel()
    {
        switch (rarity)
        {
            case Rarity.Common: return 20;
            case Rarity.Uncommon: return 40;
            case Rarity.Rare: return 60;
            case Rarity.Epic: return 80;
            case Rarity.Legendary: return 100;
            default: return 100;
        }
    }
    public float GetUpgradeChance()
    {
        if (level <= 10) return 1.0f; // 100% de chance até o nível 10
        if (level <= 90) return Mathf.Max(0.1f, 1.0f - (level - 10) * 0.01f); // Diminui até 10%
        return 0.1f; // Mínimo de 10%
    }
    public string GetUpgradeChanceFormatted()
    {
        float chance = GetUpgradeChance();
        return $"{Mathf.RoundToInt(chance * 100)}%";
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
