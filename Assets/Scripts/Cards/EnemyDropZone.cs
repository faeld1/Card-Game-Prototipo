using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private EnemyFX enemyFX;

    private void Awake()
    {
        enemyFX = GetComponentInParent<EnemyFX>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        CardDragHandler card = eventData.pointerDrag?.GetComponent<CardDragHandler>();
        if (card != null && card.cardType == CardType.Attack)
        {
            enemyFX.ShowSelectedEffect(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CardDragHandler card = eventData.pointerDrag?.GetComponent<CardDragHandler>();
        if (card != null && card.cardType == CardType.Attack)
        {
            enemyFX.ShowSelectedEffect(false);
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        CardDragHandler card = eventData.pointerDrag?.GetComponent<CardDragHandler>();

        if (card != null && card.cardType == CardType.Attack)
        {
            Enemy_Stats enemyTarget = GetComponentInParent<Enemy_Stats>();

            //Debug.Log("Carta de ataque aceita!");

            DeckManager.instance.UseCardAttack(enemyTarget, card.cardData);

            UI_Manager.instance.ShowBlockHandCards();

            enemyFX.ShowSelectedEffect(false);
        }

    }

}
