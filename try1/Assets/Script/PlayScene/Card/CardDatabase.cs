using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public List<Card> cards = new List<Card>();

    public delegate void DatabaseReadyAction();
    public event DatabaseReadyAction OnDatabaseReady;

    void Start()
    {
        InitializeDatabase();
    }

    void InitializeDatabase()
    {
        // Initialize with sample cards, including image assignments
        #region dino
        AddCard(new Card
        {
            id = 1,
            cardName = "Dodo",
            cost = 0,
            damage = 0,
            health = 1,
            dinoType = "Normal",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Normal/Dodo"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 6,
            cardName = "Triceratops",
            cost = 1,
            damage = 1,
            health = 3,
            dinoType = "Defender",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Defender/Triceratops"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 12,
            cardName = "Raptor",
            cost = 1,
            damage = 2,
            health = 1,
            dinoType = "Attacker",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Attacker/Raptor"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });
        AddCard(new Card
        {
            id = 2,
            cardName = "Microraptor",
            cost = 0,
            damage = 1,
            health = 1,
            dinoType = "Normal",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Normal/Microraptor"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 3,
            cardName = "Ichthyosaurus",
            cost = 0,
            damage = 0,
            health = 1,
            dinoType = "Normal",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Normal/Ichthyosaurus"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 4,
            cardName = "Utahraptor",
            cost = 0,
            damage = 0,
            health = 2,
            dinoType = "Normal",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Normal/Utahraptor"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 5,
            cardName = "Parasaur",
            cost = 1,
            damage = 0,
            health = 3,
            dinoType = "Defender",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Defender/Parasaur"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 7,
            cardName = "Ankylosaurus",
            cost = 1,
            damage = 0,
            health = 4,
            dinoType = "Defender",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Defender/Ankylosaurus"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 8,
            cardName = "Stegosaurus",
            cost = 2,
            damage = 1,
            health = 4,
            dinoType = "Defender",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Defender/Stegosaurus"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 9,
            cardName = "Quetzalcoatlus",
            cost = 2,
            damage = 0,
            health = 6,
            dinoType = "Defender",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Defender/Quetzalcoatlus"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 10,
            cardName = "Bronto",
            cost = 3,
            damage = 1,
            health = 7,
            dinoType = "Defender",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Defender/Bronto"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 11,
            cardName = "Brachiosaurus",
            cost = 4,
            damage = 2,
            health = 10,
            dinoType = "Defender",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Defender/Brachiosaurus"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 13,
            cardName = "Deinonychus",
            cost = 1,
            damage = 2,
            health = 1,
            dinoType = "Attacker",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Attacker/Deinonychus"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 14,
            cardName = "Pteranodon",
            cost = 2,
            damage = 3,
            health = 1,
            dinoType = "Attacker",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Attacker/Pteranodon"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 15,
            cardName = "Carnotaurus",
            cost = 2,
            damage = 3,
            health = 2,
            dinoType = "Attacker",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Attacker/Carnotaurus"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 16,
            cardName = "Spinosaurus",
            cost = 2,
            damage = 3,
            health = 3,
            dinoType = "Attacker",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Attacker/Spinosaurus"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 17,
            cardName = "Allosaurus",
            cost = 3,
            damage = 3,
            health = 4,
            dinoType = "Attacker",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Attacker/Allosaurus"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 18,
            cardName = "Mosasaurus",
            cost = 3,
            damage = 2,
            health = 6,
            dinoType = "Attacker",
            dinoImage = Resources.Load<Sprite>("CardImage/Dino/Attacker/Mosasaurus"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 19,
            cardName = "Megalodon",
            cost = 3,
            damage = 4,
            health = 3,
            dinoType = "Attacker",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Attacker/Megalodon"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 20,
            cardName = "Trex",
            cost = 4,
            damage = 5,
            health = 6,
            dinoType = "Attacker",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Attacker/Trex"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });

        AddCard(new Card
        {
            id = 21,
            cardName = "Giganotosaurus(Boss)",
            cost = 0,
            damage = 2,
            health = 20,
            dinoType = "Boss",
            dinoImage = Resources.Load<Sprite>("Picture/Dino/Boss/Giganotosaurus(Boss)"),
            cardClass = Resources.Load<Sprite>("Picture/Card/Icon/AssaultClass")
        });
        #endregion
        
    Debug.Log("Card count in database: " + cards.Count);

        if (cards.Count == 0)
        {
            Debug.LogWarning("No cards added to the database!");
        }

        if (OnDatabaseReady != null)
        {
            OnDatabaseReady();
        }
    }


    public void AddCard(Card newCard)
    {
        cards.Add(newCard);
    }

    public Card GetCardById(int id)
    {
        return cards.Find(card => card.id == id);
    }

    public void RemoveCard(int id)
    {
        Card cardToRemove = GetCardById(id);
        if (cardToRemove != null)
        {
            cards.Remove(cardToRemove);
        }
    }

    public void UpdateCard(Card updatedCard)
    {
        Card cardToUpdate = GetCardById(updatedCard.id);
        if (cardToUpdate != null)
        {
            cardToUpdate.cardName = updatedCard.cardName;
            cardToUpdate.cost = updatedCard.cost;
            cardToUpdate.damage = updatedCard.damage;
            cardToUpdate.health = updatedCard.health;
            cardToUpdate.dinoImage = updatedCard.dinoImage;
            cardToUpdate.cardClass = updatedCard.cardClass;
        }
    }
}
       
