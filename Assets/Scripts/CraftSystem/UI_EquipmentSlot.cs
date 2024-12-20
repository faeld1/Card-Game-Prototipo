using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static Equipment;

public class UI_EquipmentSlot : MonoBehaviour
{
    [Header("UI Elements")]
    public Image equipmentIcon;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI rarityText;
    public Transform starContainer; // Container para exibir estrelas
    public GameObject starPrefab; // Prefab para uma estrela
    public Button upgradeButton;

    [Header("Equipment Type")]
    public EquipmentType equipmentType; // Define o tipo deste slot (Gloves, Pants, etc.)

    private Equipment assignedEquipment; // Refer�ncia ao equipamento atual

    private void Start()
    {
        
    }

    public void RefreshSlot()
    {
        assignedEquipment = PlayerEquipmentManager.Instance.GetEquipmentByType(equipmentType);

        if (assignedEquipment != null)
        {
            // Atualiza o �cone
           // equipmentIcon.sprite = assignedEquipment.equipmentIcon ?? null;

            // Atualiza o texto
            levelText.text = $"Level: {assignedEquipment.level}";
            rarityText.text = assignedEquipment.rarity.ToString();

            // Atualiza o n�mero de estrelas
            UpdateStars(assignedEquipment.stars);

            // Ativa o bot�o de upgrade
            upgradeButton.interactable = true;
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(() => OnUpgradeButtonPressed2());
        }
        else
        {
            // Caso n�o tenha equipamento, limpa o slot
            ClearSlot();
        }
    }

    private void ClearSlot()
    {
        equipmentIcon.sprite = null;
        levelText.text = "Level: -";
        rarityText.text = "None";
        UpdateStars(0);
        upgradeButton.interactable = false;
        upgradeButton.onClick.RemoveAllListeners();
    }

    public void SetupSlot(Equipment equipment)
    {

        Debug.Log($"SetupSlot chamado para {equipment?.equipmentName ?? "N/A"}");
        assignedEquipment = equipment;

        if (equipment != null)
        {
            //equipmentIcon.sprite = equipment.equipmentIcon;
            levelText.text = $"Level: {equipment.level}";
            rarityText.text = equipment.rarity.ToString();

            // Atualiza o n�mero de estrelas
            UpdateStars(equipment.stars);

            // Ativa o bot�o de upgrade
            upgradeButton.interactable = true;
            upgradeButton.onClick.RemoveAllListeners(); // Remove listeners antigos para evitar duplica��o
            upgradeButton.onClick.AddListener(() => OnUpgradeButtonPressed(equipment));
        }
        else
        {
            // Limpa os dados se n�o houver equipamento
            equipmentIcon.sprite = null;
            levelText.text = "Level: -";
            rarityText.text = "None";

            UpdateStars(0);
            upgradeButton.interactable = false;
            upgradeButton.onClick.RemoveAllListeners();
        }
    }

    public void UpdateStars(int starCount)
    {
        foreach (Transform child in starContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < 5; i++) // Exibe at� 5 estrelas
        {
            GameObject star = Instantiate(starPrefab, starContainer);
            star.GetComponent<Image>().color = i < starCount ? Color.yellow : Color.gray;
        }
    }

    public void OnUpgradeButtonPressed(Equipment equipment)
    {
        Debug.Log("Button: "+equipment+ " Upgrade Working");
        if (PlayerEquipmentManager.Instance.TryUpgradeEquipment(equipmentType))
        {
            // Atualiza o slot com os novos dados
            SetupSlot(PlayerEquipmentManager.Instance.GetEquipmentByType(equipmentType));
        }
    }

    private void OnUpgradeButtonPressed2()
    {
        if (PlayerEquipmentManager.Instance.TryUpgradeEquipment(equipmentType))
        {
            RefreshSlot(); // Atualiza o slot ap�s o upgrade
        }
    }
}
