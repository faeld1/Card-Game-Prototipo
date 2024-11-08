using TMPro;
using UnityEngine;

public class ShieldBar : MonoBehaviour
{
    private PlayerStats playerStats;
    private TextMeshProUGUI shieldText;

    private void Start()
    {
        playerStats = GetComponentInParent<PlayerStats>();
        shieldText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateShield();
    }

    private void UpdateShield()
    {
        shieldText.text = playerStats.shield.ToString();
    }

    private void OnEnable()
    {
        CharacterStats.OnHealthChanged += UpdateShield;
    }

    private void OnDisable()
    {
        CharacterStats.OnHealthChanged -= UpdateShield;
    }
}
