using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> inventoryItems;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;

    private UI_ItemSlot[] inventoryItemSlot;

    private const string InventorySaveKey = "PlayerInventory";

    public Transform bagContainer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inventoryItems = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();


        LoadInventoryData();
        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        UpdateSlotUI();
        if(inventoryItems.Count == 0 )
        {
            AddStartingItems();
        }
        
    }
    private void AddStartingItems()
    {
        foreach (var itemData in startingItems)
        {
            if (itemData != null && !inventoryDictionary.ContainsKey(itemData))
            {
                AddItem(itemData, 0); // Adiciona os itens ao inventário com stack 0
            }
        }
    }
    public void AddItem(ItemData _item, int amount)
    {
        if(inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack(amount);
            Debug.Log($"Item atualizado: {_item.name}, Nova Quantidade: {value.stackSize}");
        }
        else
        {
            // Cria um novo item com o stack inicial
            InventoryItem newItem = new InventoryItem(_item, Mathf.Max(0, amount));
            inventoryItems.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }

        // Atualiza a lista para refletir o dicionário
        inventoryItems = new List<InventoryItem>(inventoryDictionary.Values);

        SaveInventoryData();

        UpdateSlotUI();
    }

    public void RemoveItem(ItemData _item, int amount)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            // Verifica se o item tem stack suficiente
            if (value.stackSize >= amount)
            {
                value.RemoveStack(amount);  // Remove a quantidade necessária
                if (value.stackSize <= 0)
                {
                    value.stackSize = 0;
                }
            }
            else
            {
                Debug.LogWarning("Quantidade de item insuficiente para remoção.");
            }
        }
        else
        {
            Debug.LogWarning("Item não encontrado no inventário.");
        }

        SaveInventoryData();
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        for(int i = 0; i < inventoryItems.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventoryItems[i]);
        }
    }

    public bool CanCraft(ItemData itemToCraft, int _amount)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        // Itera sobre os materiais necessários
        foreach (var requiredMaterial in itemToCraft.craftingMaterials)
        {
            // Verifica se o material está disponível no inventário
            if (inventoryDictionary.TryGetValue(requiredMaterial.data, out InventoryItem inventoryItem))
            {
                // Calcula a quantidade total necessária para o crafting
                int totalRequired = requiredMaterial.stackSize * _amount;

                // Verifica se a quantidade disponível é suficiente
                if (inventoryItem.stackSize < totalRequired)
                {
                    Debug.Log($"Não há materiais suficientes para {requiredMaterial.data.itemName}");
                    return false; // Não há quantidade suficiente
                }
                else
                {
                    // Adiciona à lista de materiais a serem consumidos
                    materialsToRemove.Add(inventoryItem);
                }
            }
            else
            {
                Debug.Log($"Material {requiredMaterial.data.itemName} não encontrado no inventário.");
                return false; // Material não encontrado
            }
        }

        // Se todos os materiais forem suficientes, consome-os e adiciona o item ao inventário
        foreach (var requiredMaterial in itemToCraft.craftingMaterials)
        {
            int totalRequired = requiredMaterial.stackSize * _amount;
            RemoveItem(requiredMaterial.data, totalRequired); // Remove apenas o necessário
        }
        AddItem(itemToCraft,_amount);

        Debug.Log($"Item {itemToCraft.itemName} foi fabricado com sucesso!");

        return true;
    }

    public InventoryItem GetItemByData(ItemData itemData)
    {
        return inventoryDictionary.ContainsKey(itemData) ? inventoryDictionary[itemData] : null;
    }
    public int GetItemCount(ItemData item)
    {
        if (inventoryDictionary.TryGetValue(item, out InventoryItem inventoryItem))
        {
            return inventoryItem.stackSize;
        }
        return 0;
    }

    private void SaveInventoryData()
    {
        ES3.Save(InventorySaveKey + "_Dictionary", inventoryDictionary);
        ES3.Save(InventorySaveKey + "_List", inventoryItems);
    }

    private void LoadInventoryData()
    {
        Debug.Log("Carregando inventário...");
        if (ES3.KeyExists(InventorySaveKey + "_List"))
        {
            inventoryItems = ES3.Load<List<InventoryItem>>(InventorySaveKey + "_List");
            inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
            foreach (var item in inventoryItems)
            {
                inventoryDictionary.Add(item.data, item);
                Debug.Log($"Carregado Item: {item.data.name}, Quantidade: {item.stackSize}");
            }
        }
        else
        {
            inventoryItems = new List<InventoryItem>();
            inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        }
    }

}
