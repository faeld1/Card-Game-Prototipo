using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardData cardData;
    private Vector3 originalPosition;
    private Transform parentToReturnTo = null;
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //originalPosition = transform.position;
        originalPosition = rectTransform.position;
        parentToReturnTo = transform.parent;
        transform.SetParent(transform.root);  // Torna a carta um elemento de n�vel superior para que n�o fique "preso" na UI enquanto arrasta
        GetComponent<CanvasGroup>().blocksRaycasts = false; // Permite que o Raycast da �rea de soltura funcione
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = eventData.position; // Atualiza a posi��o da carta para a posi��o do ponteiro
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentToReturnTo);
        transform.position = originalPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true; // Restaura o Raycast ap�s o arraste
    }
}