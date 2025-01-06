using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        

        
        //UpdateSlotUI();
        if (inventoryItems.Count == 0)
        {
            AddStartingItems();
        }
        EnsureStartingItemsInInventory();

    }
    private void AddStartingItems()
    {
        // Verifica se a cena atual é a do menu
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Debug.Log("AddStartingItems só funciona na cena do menu.");
            return;
        }
        foreach (var itemData in startingItems)
        {
            if (itemData != null && !inventoryDictionary.ContainsKey(itemData))
            {
                AddItem(itemData, 0); // Adiciona os itens ao inventário com stack 0
            }
        }
    }
    private void EnsureStartingItemsInInventory()
    {
        // Verifica se a cena atual é a do menu
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Debug.Log("EnsureStartingItemsInInventory só funciona na cena do menu.");
            return;
        }
        // Usamos um HashSet para rastrear os itens já verificados e evitar duplicatas
        HashSet<ItemData> alreadyProcessed = new HashSet<ItemData>();

        foreach (var itemData in startingItems)
        {
            if (itemData == null) continue; // Ignora itens nulos

            // Verifica se o item já foi processado para evitar múltiplas adições
            if (alreadyProcessed.Contains(itemData))
            {
                Debug.LogWarning($"Item {itemData.itemName} já foi processado anteriormente. Ignorando.");
                continue;
            }

            if (itemData.itemIcon == null)
            {
                Debug.LogWarning($"Item {itemData.itemName} não possui ícone. Verifique o ScriptableObject.");
                continue;
            }

            // Marca o item como processado
            alreadyProcessed.Add(itemData);

            // Verifica se o item já está presente no inventário
            if (!inventoryDictionary.ContainsKey(itemData))
            {
                Debug.Log($"Adicionando item inicial ausente: {itemData.itemName}");

                // Adiciona o item com stack 0
                AddItem(itemData, 0);
            }
            else
            {
                Debug.Log($"Item {itemData.itemName} já está presente no inventário.");
            }
        }
        UpdateSlotUI();
        // Salva o estado atualizado do inventário apenas uma vez
        SaveInventoryData();
    }
    private void EnsureItemDataIntegrity(InventoryItem item)
    {
        if (item.data == null)
        {
            Debug.LogWarning("ItemData é nulo. Verificação ignorada.");
            return;
        }

        // Verifica se o ícone do item está correto
        if (item.data.itemIcon == null)
        {
            Debug.LogWarning($"Ícone do item {item.data.itemName} está nulo. Verifique o ScriptableObject.");
        }

        // Outras verificações podem ser adicionadas aqui, se necessário
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
    public bool CanRemoveItem(ItemData _item, int amount)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            return value.stackSize >= amount;
        }
        return false; // Item não encontrado ou quantidade insuficiente
    }

    public void UpdateSlotUI()
    {
        for(int i = 0; i < inventoryItems.Count; i++)
        {
            if(i < inventoryItemSlot.Length)
            {
                inventoryItemSlot[i].UpdateSlot(inventoryItems[i]);
            }
            else
            {
                Debug.LogWarning("Número de itens no inventário excede o número de slots disponíveis.");
                break;
            }       
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
                //Debug.Log($"Carregado Item: {item.data.name}, Quantidade: {item.stackSize}");
            }
        }
        else
        {
            inventoryItems = new List<InventoryItem>();
            inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        }
    }

}
