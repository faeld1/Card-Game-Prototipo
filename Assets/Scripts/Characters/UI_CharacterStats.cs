using TMPro;
using UnityEngine;


public class UI_CharacterStats : MonoBehaviour
{
    public TextMeshProUGUI maxHPText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI critChanceText;
    public TextMeshProUGUI critPowerText;
    public TextMeshProUGUI evasionText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI levelText;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        if (playerStats == null) return;

        maxHPText.text = $"Max HP: {playerStats.maxHealth.GetValue()}";
        armorText.text = $"Shield: {playerStats.armor.GetValue()}";
        critChanceText.text = $"Crit Chance: {playerStats.critChance.GetValue()}%";
        critPowerText.text = $"Crit Power: {playerStats.critPower.GetValue()}%";
        evasionText.text = $"Evasion: {playerStats.evasion.GetValue()}%";
        damageText.text = $"Damage: {playerStats.damage.GetValue()}";
        levelText.text = $"Level: {playerStats.level}";
    }
}
