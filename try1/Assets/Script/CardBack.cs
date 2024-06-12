using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBack : MonoBehaviour
{
    public GameObject cardBack;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (ThisCard.staticCardBack == true)
        {
            cardBack.SetActive(true); // Set active
        }
        else
        {
            cardBack.SetActive(false); // Set inactive
        }
    }
}

