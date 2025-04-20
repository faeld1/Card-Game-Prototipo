using TMPro;
using UnityEngine;

public class ShieldBar : MonoBehaviour
{
    private CharacterStats stats;
    private TextMeshProUGUI shieldText;

    private void Start()
    {
        stats = GetComponentInParent<CharacterStats>();
        shieldText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateShield();
    }

    private void UpdateShield()
    {
        shieldText.text = stats.shield.ToString();
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
