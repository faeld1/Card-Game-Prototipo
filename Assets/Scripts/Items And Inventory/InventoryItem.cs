using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ItemData data;

    public int stackSize;

    public InventoryItem(ItemData _newItemData, int initialStack)
    {
        data = _newItemData;
        stackSize = initialStack;
    }

    public void AddStack(int amount)
    {
        stackSize += amount;
        Debug.Log($"Stack atualizado: {data.name}, Novo tamanho do stack: {stackSize}");
    }

    public void RemoveStack(int amount)
    {
        stackSize -= amount;
        if (stackSize < 0)
        {
            stackSize = 0; // Previne que o stack fique negativo
        }
    }
}
