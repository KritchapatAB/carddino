using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

[CreateAssetMenu(menuName = "card")]
public class Card
{
        public int id;
        public string cardName;
        public Sprite cardClass;
        public int cost;
        public int damage;
        public int health;
        public Sprite dinoImage;
        
}
