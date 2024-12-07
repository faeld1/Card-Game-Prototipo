using UnityEngine;

[CreateAssetMenu(fileName ="New Card", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public int energyCost;
    public int attackValue;
    public int defenseValue;
    public Sprite cardSprite;
    public CardType cardType;

    public CardData evolvedCard; // Carta para a qual essa carta evolui (pode ser null)
    public bool isTemporary; // Indica se a carta é temporária
}

public enum CardType
{
    Attack,
    Defense,
    Support
}
