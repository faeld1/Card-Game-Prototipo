using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private GameObject dropPrefab;

    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    public virtual void GenerateDrop()
    {
        if (possibleDrop.Length <= 0)
            return;

        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if(Random.Range(0,100) <= possibleDrop[i].dropChance)
                dropList.Add(possibleDrop[i]);
        }

        for (int i = 0;i < possibleItemDrop; i++)
        {
            if (dropList.Count <= 0)
                return;

            ItemData randomItem = dropList[Random.Range(0, dropList.Count)];

            dropList.Remove(randomItem);

            int randomAmount = Random.Range(1, 5);
            ItemDrop(randomItem,randomAmount);
        }
    }

    public void ItemDrop(ItemData _itemData, int amount)
    {
        GameObject newDrop = Instantiate(dropPrefab,transform.position,Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(4, 6));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData,randomVelocity,amount);
    }
}
