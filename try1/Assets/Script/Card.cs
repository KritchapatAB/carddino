using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Card
{
        public int id;
        public string cardname;
        public string cardclass;
        public int cost;
        public int damage;
        public int health;
        public Sprite dinoImage;
        
        public Card()
        {

        }
        
        public Card(int Id, string Cardname, string Cardclass, int Cost, int Damage, int Health, Sprite DinoImage)
        {
            id = Id;
            cardname = Cardname;
            cardclass = Cardclass;
            cost = Cost;
            damage = Damage;
            health = Health;
            dinoImage = DinoImage;
        }
}
