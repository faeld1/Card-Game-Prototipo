using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ItemData itemData;

    public delegate void OnCraftSlotSelected(ItemData item);
    public static event OnCraftSlotSelected CraftSlotSelected;

    public void SetupCraftSlot(ItemData item)
    {
        itemData = item;
       // GetComponentInChildren<UnityEngine.UI.Image>().sprite = item.itemIcon;
        GetComponent<UnityEngine.UI.Image>().sprite = item.itemIcon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //CraftSlotSelected?.Invoke(itemData);
        UI_CraftWindow.Instance.SelectItem(itemData);
    }
}
