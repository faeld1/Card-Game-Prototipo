using UnityEngine;

public class UI_CraftList : MonoBehaviour
{
    [SerializeField] private GameObject craftSlotPrefab;
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private ItemData[] craftableItems; // Lista de itens que podem ser fabricados

    private void Start()
    {
        PopulateCraftList();
        SelectFirstItemAsDefault();
    }

    private void PopulateCraftList()
    {
        foreach (var item in craftableItems)
        {
            GameObject slot = Instantiate(craftSlotPrefab, craftSlotParent);
            UI_CraftSlot craftSlot = slot.GetComponent<UI_CraftSlot>();
            craftSlot.SetupCraftSlot(item);
        }
    }

    private void SelectFirstItemAsDefault()
    {
        if (craftableItems.Length > 0)
        {
            UI_CraftWindow.Instance.SelectItem(craftableItems[0]);
        }
    }
}
