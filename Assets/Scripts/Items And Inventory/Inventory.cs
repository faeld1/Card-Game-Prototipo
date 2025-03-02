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
        // Verifica se a cena atual � a do menu
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Debug.Log("AddStartingItems s� funciona na cena do menu.");
            return;
        }
        foreach (var itemData in startingItems)
        {
            if (itemData != null && !inventoryDictionary.ContainsKey(itemData))
            {
                AddItem(itemData, 0); // Adiciona os itens ao invent�rio com stack 0
            }
        }
    }
    private void EnsureStartingItemsInInventory()
    {
        // Verifica se a cena atual � a do menu
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            return;
        }
        // Usamos um HashSet para rastrear os itens j� verificados e evitar duplicatas
        HashSet<ItemData> alreadyProcessed = new HashSet<ItemData>();

        foreach (var itemData in startingItems)
        {
            if (itemData == null) continue; // Ignora itens nulos

            // Verifica se o item j� foi processado para evitar m�ltiplas adi��es
            if (alreadyProcessed.Contains(itemData))
            {
                Debug.LogWarning($"Item {itemData.itemName} j� foi processado anteriormente. Ignorando.");
                continue;
            }

            if (itemData.itemIcon == null)
            {
                Debug.LogWarning($"Item {itemData.itemName} n�o possui �cone. Verifique o ScriptableObject.");
                continue;
            }

            // Marca o item como processado
            alreadyProcessed.Add(itemData);

            // Verifica se o item j� est� presente no invent�rio
            if (!inventoryDictionary.ContainsKey(itemData))
            {
                Debug.Log($"Adicionando item inicial ausente: {itemData.itemName}");

                // Adiciona o item com stack 0
                AddItem(itemData, 0);
            }
            else
            {
                //Debug.Log($"Item {itemData.itemName} j� est� presente no invent�rio.");
            }
        }
        UpdateSlotUI();
        // Salva o estado atualizado do invent�rio apenas uma vez
        SaveInventoryData();
    }
    private void EnsureItemDataIntegrity(InventoryItem item)
    {
        if (item.data == null)
        {
            Debug.LogWarning("ItemData � nulo. Verifica��o ignorada.");
            return;
        }

        // Verifica se o �cone do item est� correto
        if (item.data.itemIcon == null)
        {
            Debug.LogWarning($"�cone do item {item.data.itemName} est� nulo. Verifique o ScriptableObject.");
        }

        // Outras verifica��es podem ser adicionadas aqui, se necess�rio
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

        // Atualiza a lista para refletir o dicion�rio
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
                value.RemoveStack(amount);  // Remove a quantidade necess�ria
                if (value.stackSize <= 0)
                {
                    value.stackSize = 0;
                }
            }
            else
            {
                Debug.LogWarning("Quantidade de item insuficiente para remo��o.");
            }
        }
        else
        {
            Debug.LogWarning("Item n�o encontrado no invent�rio.");
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
        return false; // Item n�o encontrado ou quantidade insuficiente
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
                Debug.LogWarning("N�mero de itens no invent�rio excede o n�mero de slots dispon�veis.");
                break;
            }       
        }
    }

    public bool CanCraft(ItemData itemToCraft, int _amount)
    {
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        // Itera sobre os materiais necess�rios
        foreach (var requiredMaterial in itemToCraft.craftingMaterials)
        {
            // Verifica se o material est� dispon�vel no invent�rio
            if (inventoryDictionary.TryGetValue(requiredMaterial.data, out InventoryItem inventoryItem))
            {
                // Calcula a quantidade total necess�ria para o crafting
                int totalRequired = requiredMaterial.stackSize * _amount;

                // Verifica se a quantidade dispon�vel � suficiente
                if (inventoryItem.stackSize < totalRequired)
                {
                    Debug.Log($"N�o h� materiais suficientes para {requiredMaterial.data.itemName}");
                    return false; // N�o h� quantidade suficiente
                }
                else
                {
                    // Adiciona � lista de materiais a serem consumidos
                    materialsToRemove.Add(inventoryItem);
                }
            }
            else
            {
                Debug.Log($"Material {requiredMaterial.data.itemName} n�o encontrado no invent�rio.");
                return false; // Material n�o encontrado
            }
        }

        // Se todos os materiais forem suficientes, consome-os e adiciona o item ao invent�rio
        foreach (var requiredMaterial in itemToCraft.craftingMaterials)
        {
            int totalRequired = requiredMaterial.stackSize * _amount;
            RemoveItem(requiredMaterial.data, totalRequired); // Remove apenas o necess�rio
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
