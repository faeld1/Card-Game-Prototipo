using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> inventoryItems;
    public Dictionary<ItemData, InventoryItem> inventoryDirectionary;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;

    private UI_ItemSlot[] inventoryItemSlot;

    [Header("Itens Dropaveis")]
    public int bless;
    public int creations;


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
        inventoryDirectionary = new Dictionary<ItemData, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();

        AddStartingItems();
    }
    private void AddStartingItems()
    {
        for (int i = 0; i < startingItems.Count; i++)
        {
            if(startingItems[i] != null)
            {
                AddItem(startingItems[i]);
                RemoveItem(startingItems[i]);
            }
        }
    }
    public void AddItem(ItemData _item)
    {
        if(inventoryDirectionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
            if(_item.itemType == ItemType.Bless)
                bless++;
            else if(_item.itemType == ItemType.Creation)
                creations++;
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventoryItems.Add(newItem);
            inventoryDirectionary.Add(_item, newItem);
        }

        UpdateSlotUI();
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDirectionary.TryGetValue(_item, out InventoryItem value))
        {
            value.RemoveStack();
        }
    }

    private void UpdateSlotUI()
    {
        for(int i = 0; i < inventoryItems.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventoryItems[i]);
        }
    }

}
