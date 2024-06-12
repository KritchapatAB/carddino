using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystem : MonoBehaviour {
    public bool isYourTurn;
    public int yourTurn;
    public int yourOpponentTurn;
    public TextMeshProUGUI turnText;
    
    public int maxMana;
    public int currentMana;
    public TextMeshProUGUI ManaText;

    void Start()
    {
       isYourTurn = true;
       yourTurn = 1;
       yourOpponentTurn = 0;

       maxMana = 1;
       currentMana =1; 
    }

    void Update()
    {
        if(isYourTurn == true){
            turnText.text = "YourTurn";
        }else turnText.text = "Opponent Turn";
        
        ManaText.text= currentMana+"/"+maxMana;        
    }

    public void EndYourTurn(){
        isYourTurn = false;
        yourOpponentTurn += 1;
    }
    public void EndYourOpponentTurn(){
        isYourTurn = true;
        yourTurn += 1;

        maxMana +=1;
        currentMana = maxMana;
    }
}
