using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public TextMeshProUGUI blessDustText;
    public TextMeshProUGUI creationsText;
    public TMP_InputField inputAmount;
    public Button craftButton;
    public Button maxButton;

    private void Start()
    {
        UpdateUI();

        craftButton.onClick.AddListener(CraftCreations);
        maxButton.onClick.AddListener(FillMaxAmount);
    }

    private void UpdateUI()
    {
        blessDustText.text = "P� de Bless: " + Inventory.instance.blessDust;
        creationsText.text = "Creations: " + Inventory.instance.creations;
        Inventory.instance.UpdateSlotUI();
    }

    private void FillMaxAmount()
    {
        int maxCraftableAmount = (Inventory.instance.blessDust / 10) * 10;
        inputAmount.text = maxCraftableAmount.ToString();
    }

    // Realiza o craft de creations
    private void CraftCreations()
    {
        if (int.TryParse(inputAmount.text, out int amountToUse))
        {
            // Calcula o m�ximo m�ltiplo de 10 que pode ser usado
            int craftableAmount = (amountToUse / 10) * 10;
            if (craftableAmount > 0 && craftableAmount <= Inventory.instance.blessDust)
            {
                int creationsCrafted = craftableAmount / 10;
                Inventory.instance.creations += creationsCrafted;
                Inventory.instance.blessDust -= craftableAmount;
                UpdateUI();
            }
            else
            {
                Debug.Log("Valor inv�lido ou maior do que o dispon�vel.");
            }
        }
        else
        {
            Debug.Log("Entrada inv�lida.");
        }
    }
}
