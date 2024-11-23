using System.Collections;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
   [SerializeField] private SpriteRenderer sr;
   [SerializeField] private Rigidbody2D rb;

    [SerializeField] private ItemData itemData;

    private void Start()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;

        StartCoroutine(MoveToBag());
    }

    private void SetupVisuals()
    {
        if (itemData == null)
            return;

        sr.sprite = itemData.itemIcon;
        gameObject.name = "Item Object - " + itemData.name;
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.linearVelocity = _velocity;

        SetupVisuals();
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
        yield return new WaitForSeconds(0.35f);

        rb.bodyType = RigidbodyType2D.Kinematic;

        Vector3 targetPosition = Inventory.instance.bagContainer.transform.position;

        // Continua movendo enquanto não alcançar a posição alvo
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            // Move o item em direção ao alvo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 100f * Time.deltaTime);
            yield return null; // Espera até o próximo frame
        }
    }
}
