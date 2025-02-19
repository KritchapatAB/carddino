using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    public int id;
    public string cardName;
    public Sprite cardClass;
    public int cost;
    public int damage;
    public int health;
    public string dinoType;
    public Sprite dinoImage;

    public Card(Card other)
    {
        this.id = other.id;
        this.cardName = other.cardName;
        this.health = other.health;
        this.damage = other.damage;
        this.cost = other.cost;
        this.dinoType = other.dinoType;
        this.dinoImage = other.dinoImage;
        this.cardClass = other.cardClass;
    }
}