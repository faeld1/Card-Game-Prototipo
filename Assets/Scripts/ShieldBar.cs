using TMPro;
using UnityEngine;

public class ShieldBar : MonoBehaviour
{
    [SerializeField] private CharacterStats stats;
    private TextMeshProUGUI shieldText;

    private void Start()
    {
        if(stats == null)
            stats = GetComponentInParent<CharacterStats>();

        shieldText = GetComponentInChildren<TextMeshProUGUI>();

        UpdateShield();
    }

    private void UpdateShield()
    {
        //shieldText.text = stats.shield.ToString();

        int shieldValue = stats.shield;

        if (shieldValue >= 1000)
        {
            int rounded = Mathf.RoundToInt(shieldValue / 1000f); // Arredonda corretamente
            shieldText.text = $"{rounded}k";
        }
        else
        {
            shieldText.text = shieldValue.ToString();
        }
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
