using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardData cardData;
    public CardType cardType;
    private Vector3 originalPosition;
    private Transform parentToReturnTo = null;
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        //cardType = cardData.cardType;
    }

    public void OnBeginDrag(PointerEventData eventData)
    { 
        originalPosition = rectTransform.position;
        parentToReturnTo = transform.parent;
        transform.SetParent(transform.root);  // Torna a carta um elemento de nível superior para que não fique "preso" na UI enquanto arrasta
        GetComponent<CanvasGroup>().blocksRaycasts = false; // Permite que o Raycast da área de soltura funcione
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentToReturnTo);
        transform.position = originalPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true; // Restaura o Raycast após o arraste
    }
}
