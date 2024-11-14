using System.Collections;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField] private ItemData itemData;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = itemData.itemIcon;

        StartCoroutine(MoveToBag());

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bag"))
        {
            Inventory.instance.AddItem(itemData);
            Destroy(gameObject);
        }
    }

    private IEnumerator MoveToBag()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 targetPosition = Inventory.instance.bagContainer.transform.position;

        // Continua movendo enquanto não alcançar a posição alvo
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            // Move o item em direção ao alvo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 50f * Time.deltaTime);
            yield return null; // Espera até o próximo frame
        }
    }
}
