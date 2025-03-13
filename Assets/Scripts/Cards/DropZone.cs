using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public DeckManager deckManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CardDragHandler card = eventData.pointerDrag?.GetComponent<CardDragHandler>();
        if (card != null && card.cardType == CardType.Attack)
        {
            Player player = BattleManager.instance.playerStats.GetComponent<Player>();

            EnemyFX enemyFX = player.CurrentEnemyTarget.GetComponent<EnemyFX>();

            enemyFX.ShowSelectedEffect(true);
        }
        else if (card != null)
        {
            if (card.cardType == CardType.Defense || card.cardType == CardType.Support)
            {
                PlayerFX playerFX = BattleManager.instance.playerStats.GetComponent<PlayerFX>();
                playerFX.ShowSelectedEffect(true);
            }
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CardDragHandler card = eventData.pointerDrag?.GetComponent<CardDragHandler>();
        if (card != null && card.cardType == CardType.Attack)
        {
            Player player = BattleManager.instance.playerStats.GetComponent<Player>();

            EnemyFX enemyFX = player.CurrentEnemyTarget.GetComponent<EnemyFX>();

            enemyFX.ShowSelectedEffect(false);
        }
        else if (card != null)
        {
            if (card.cardType == CardType.Defense || card.cardType == CardType.Support)
            {
                PlayerFX playerFX = BattleManager.instance.playerStats.GetComponent<PlayerFX>();
                playerFX.ShowSelectedEffect(false);
            }

        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        CardDragHandler card = eventData.pointerDrag.GetComponent<CardDragHandler>();
        
        if (card != null)
        {
            Player player = BattleManager.instance.playerStats.GetComponent<Player>();

            EnemyFX enemyFX = player.CurrentEnemyTarget.GetComponent<EnemyFX>();

           /* if (card.cardType == CardType.Attack)
            {
                UI_Manager.instance.ShowBlockHandCards();
            }*/

            deckManager.UseCard(card.cardData);
            UI_Manager.instance.ShowBlockHandCards();

            enemyFX.ShowSelectedEffect(false);

            PlayerFX playerFX = BattleManager.instance.playerStats.GetComponent<PlayerFX>();
            playerFX.ShowSelectedEffect(false);
        }
    }
}
