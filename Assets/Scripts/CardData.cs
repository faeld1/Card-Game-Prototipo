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

}

public enum CardType
{
    Attack,
    Defense,
    Support
}
