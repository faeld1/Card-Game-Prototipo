using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private GameObject dropPrefab;

    public void ItemDrop()
    {
        GameObject newDrop = Instantiate(dropPrefab,transform.position,Quaternion.identity);
    }
}
