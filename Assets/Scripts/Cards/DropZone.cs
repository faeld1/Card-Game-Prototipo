using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public DeckManager deckManager;

    public void OnDrop(PointerEventData eventData)
    {
        CardDragHandler card = eventData.pointerDrag.GetComponent<CardDragHandler>();
        if (card != null)
        {
            deckManager.UseCard(card.cardData);
            //Destroy(card.gameObject); // Remove a carta da UI após o uso
        }
    }
}
