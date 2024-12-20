using System.Collections.Generic;
using UnityEngine;

public class UI_EquipmentManager : MonoBehaviour
{
    public List<UI_EquipmentSlot> equipmentSlots;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    private UI_ItemSlot[] inventoryItemSlot;

    private void Start()
    {
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();

        if (PlayerEquipmentManager.Instance == null || PlayerEquipmentManager.Instance.playerEquipment == null)
        {
            Debug.LogError("PlayerEquipmentManager não está inicializado!");
            return;
        }

        RefreshUI();
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        Inventory _inventory = Inventory.instance;

        for (int i = 0; i < _inventory.inventoryItems.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(_inventory.inventoryItems[i]);
        }
    }
    public void RefreshUI()
    {
        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            if (equipmentSlots[i] != null)
            {
                equipmentSlots[i].RefreshSlot();
            }
        }
    }

    // Método para atualizar um slot específico após mudanças
    public void UpdateEquipmentSlot(int index)
    {
        if (index >= 0 && index < equipmentSlots.Count)
        {
            Equipment updatedEquipment = PlayerEquipmentManager.Instance.playerEquipment[index];
            equipmentSlots[index].SetupSlot(updatedEquipment);
        }
        else
        {
            Debug.LogWarning($"Índice {index} está fora do alcance dos slots de equipamento.");
        }
    }
}
