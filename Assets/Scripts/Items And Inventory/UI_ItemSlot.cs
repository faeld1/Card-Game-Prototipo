using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ItemSlot : MonoBehaviour
{
   [SerializeField]private Image itemImage;
   [SerializeField] private TextMeshProUGUI itemText;

    public InventoryItem item;

    private void Start()
    {
    }

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;

        if (item != null)
        {
            itemImage.sprite = item.data.itemIcon;

            if (item.data.itemType == ItemType.Bless)
                itemText.text = Inventory.instance.bless.ToString();
            else if (item.data.itemType == ItemType.Creation)
                itemText.text = Inventory.instance.creations.ToString();
        }
    }
}
