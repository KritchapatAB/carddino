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
}