using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
public class UI_CraftWindow : MonoBehaviour
{
    public static UI_CraftWindow Instance;

    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Slider quantitySlider;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI maxCraftableText;

    [SerializeField] private Transform materialRequirementsPanel; // Painel de materiais
    [SerializeField] private GameObject materialSlotPrefab; // Prefab para os materiais

    private Dictionary<string, GameObject> materialSlots = new Dictionary<string, GameObject>();

    private ItemData currentItem;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

   /* private void OnEnable()
    {
        UI_CraftSlot.CraftSlotSelected += UpdateSelectedItem;
    }

    private void OnDisable()
    {
        UI_CraftSlot.CraftSlotSelected -= UpdateSelectedItem;
    }*/

    public void SelectItem(ItemData item)
    {
        /*currentItem = item;

        // Atualiza o CraftWindow com os detalhes do item
        itemImage.sprite = item.itemIcon;
        itemNameText.text = item.itemName;

        string materials = "";
        foreach (var material in item.craftingMaterials)
        {
            int available = Inventory.instance.GetItemCount(material.data);
            materials += $"{material.data.itemName}: {available}/{material.stackSize}\n";
        }

        SetupSlider();*/
        ShowItemDetails(item);
    }

    public void ShowItemDetails(ItemData item)
    {
        currentItem = item;
        itemImage.sprite = item.itemIcon;
        itemNameText.text = item.itemName;

        // Limpa o painel de materiais
        foreach (Transform child in materialRequirementsPanel)
        {
            Destroy(child.gameObject);
        }

        materialSlots.Clear();
        
        foreach (var material in item.craftingMaterials)
        {
            GameObject materialSlot = Instantiate(materialSlotPrefab, materialRequirementsPanel);

            // Atualiza o ícone do material
            Image icon = materialSlot.transform.GetComponent<Image>();
            icon.sprite = material.data.itemIcon;

            // Atualiza o texto da quantidade
            TextMeshProUGUI quantityText = materialSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            int available = Inventory.instance.GetItemCount(material.data);
            int required = material.stackSize * (int)quantitySlider.value; // Multiplica pelo valor do Slider
            string color = available >= required ? "black" : "red";
            quantityText.text = $"<color={color}>{available} / {required}</color>";

            // Adiciona o slot ao dicionário
            materialSlots[material.data.itemName] = materialSlot;
        }

        SetupSlider();
    }

    public void HideItemDetails()
    {
        itemImage.sprite = null;
        itemNameText.text = "";
        quantitySlider.value = 1;
        quantityText.text = "1";

        // Limpa o painel de materiais
        foreach (Transform child in materialRequirementsPanel)
        {
            Destroy(child.gameObject);
        }
    }

    private void UpdateSelectedItem(ItemData item)
    {
        ShowItemDetails(item);
    }
    private void SetupSlider()
    {
        int maxCraftable = CalculateMaxCraftable(currentItem);
        if (maxCraftable > 0)
        {
            quantitySlider.interactable = true;
            quantitySlider.maxValue = maxCraftable;
            quantitySlider.minValue = 1; // Mínimo de 1 para crafting
            quantitySlider.value = 1;
        }
        else
        {
            quantitySlider.interactable = false;
            quantitySlider.maxValue = 0;
            quantitySlider.minValue = 0;
            quantitySlider.value = 0;
        }

        quantityText.text = maxCraftable > 0 ? "1" : "0";
        UpdateMaxCraftableText(maxCraftable);

        quantitySlider.onValueChanged.RemoveAllListeners();
        quantitySlider.onValueChanged.AddListener(amount =>
        {
            quantityText.text = ((int)amount).ToString();
            UpdateMaterialSlots();
        });

        // Atualiza os materiais inicialmente
        UpdateMaterialSlots();
    }
    private void UpdateMaxCraftableText(int maxCraftable)
    {
        if (maxCraftable > 0)
        {
            maxCraftableText.text = maxCraftable.ToString();
        }
        else
        {
            maxCraftableText.text = "<color=red>0</color>";
        }

    }
    private int CalculateMaxCraftable(ItemData item)
    {
        int maxCraftable = int.MaxValue;
        foreach (var material in item.craftingMaterials)
        {
            int available = Inventory.instance.GetItemCount(material.data);
            maxCraftable = Mathf.Min(maxCraftable, available / material.stackSize);
        }
        return maxCraftable;
    }

    private void UpdateMaterialSlots()
    {
        foreach (var material in currentItem.craftingMaterials)
        {
            // Acessa diretamente pelo dicionário
            if (materialSlots.TryGetValue(material.data.itemName, out GameObject materialSlot))
            {
                TextMeshProUGUI quantityText = materialSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                int available = Inventory.instance.GetItemCount(material.data);
                int required = material.stackSize * (int)quantitySlider.value; // Multiplica pelo valor do Slider
                if(required < material.stackSize)
                    required = material.stackSize;
                string color = available >= required ? "black" : "red";
                quantityText.text = $"<color={color}>{available} / {required}</color>";
            }
        }
    }

    public void OnCraftButtonPressed()
    {
        int amountToCraft = (int)quantitySlider.value;

        if (!Inventory.instance.CanCraft(currentItem, amountToCraft))
        {
            Debug.Log("Não foi possível fabricar o item.");
        }
        else
        {
            Debug.Log($"{currentItem.itemName} fabricado com sucesso!");
            ShowItemDetails(currentItem); // Atualiza os detalhes após o craft
            PlayerEquipmentManager.Instance.equipmentManager.UpdateInventory();
        }
    }
}
