using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    [Header("Items")]
    public ItemData blessDustItem;
    public ItemData creationsItem;

    [Header("UI Elements")]
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
        var blessDust = Inventory.instance.GetItemByData(blessDustItem);
        var creations = Inventory.instance.GetItemByData(creationsItem);

        blessDustText.text = "Pó de Bless: " + (blessDust != null ? blessDust.stackSize : 0);
        creationsText.text = "Creations: " + (creations != null ? creations.stackSize : 0);

        Inventory.instance.UpdateSlotUI();
    }

    private void FillMaxAmount()
    {
        var blessDust = Inventory.instance.GetItemByData(blessDustItem);

        if (blessDust != null)
        {
            int maxCraftableAmount = (blessDust.stackSize / 10) * 10;
            inputAmount.text = maxCraftableAmount.ToString();
        }
    }

    // Realiza o craft de creations
    private void CraftCreations()
    {
        if (int.TryParse(inputAmount.text, out int amountToUse))
        {
            var blessDust = Inventory.instance.GetItemByData(blessDustItem);

            if (blessDust != null)
            {
                int craftableAmount = (amountToUse / 10) * 10;

                if (craftableAmount > 0 && craftableAmount <= blessDust.stackSize)
                {
                    int creationsCrafted = craftableAmount / 10;

                    Inventory.instance.RemoveItem(blessDustItem, craftableAmount);
                    Inventory.instance.AddItem(creationsItem, creationsCrafted);

                    UpdateUI();
                }
                else
                {
                    Debug.Log("Valor inválido ou maior do que o disponível.");
                }
            }
            else
            {
                Debug.Log("Pó de Bless não disponível.");
            }
        }
        else
        {
            Debug.Log("Entrada inválida.");
        }
    }
}
