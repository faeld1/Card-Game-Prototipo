using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Data", menuName = "Data/Equipment")]
public class EquipmentData : ScriptableObject
{
    public EquipmentType equipmentType; // Tipo de equipamento (Gloves, Boots, etc.)
    public string equipmentName; // Nome do equipamento
    public Sprite equipmentIcon; // Ícone do equipamento

    // Stats base
    public int baseMaxHPBonus;
    public int baseArmorBonus;
    public int baseCritChanceBonus;
    public int baseCritPowerBonus;
    public int baseEvasionBonus;
    public int baseDamageBonus;
}
