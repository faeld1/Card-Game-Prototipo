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

    private void Update()
    {
        ShowItemsQuant();
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

    private void ShowItemsQuant()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            foreach (var item in inventoryItems)
            {
                Debug.Log($"Lista - ItemName: {item.data.name}, ItemQuant: {item.stackSize}");
            }

            foreach (var kvp in inventoryDictionary)
            {
                Debug.Log($"Dicionário - ItemName: {kvp.Key.name}, ItemQuant: {kvp.Value.stackSize}");
            }
        }
    }

    public void RemoveItem(ItemData _item, int amount)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.RemoveStack(amount);
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

    public InventoryItem GetItemByData(ItemData itemData)
    {
        return inventoryDictionary.ContainsKey(itemData) ? inventoryDictionary[itemData] : null;
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
