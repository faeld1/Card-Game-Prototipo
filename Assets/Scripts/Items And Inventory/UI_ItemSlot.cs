using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ItemSlot : MonoBehaviour
{
   [SerializeField]private Image itemImage;
   [SerializeField] private TextMeshProUGUI itemText;

    public InventoryItem item;

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;

        /* if (item != null)
         {
             itemImage.sprite = item.data.itemIcon;
             itemText.text = item.stackSize.ToString();
         }*/
        itemImage.sprite = item.data.itemIcon;
        itemText.text = item.stackSize.ToString();
    }
}
