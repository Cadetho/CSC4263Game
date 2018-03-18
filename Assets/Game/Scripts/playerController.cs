using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour{

    private playerHand hand;
    public GameManager gm;
    bool showHand;



    public void drawCards(int num) {
        for (int i = 0; i < num; i++) {
            if (gm.deckNotEmpty()) {
                hand.addCard(gm.drawCard());
            }
        }
    }


    private void Start() {
        hand = GetComponent<playerHand>();
    }

    private void Update() {
        
    }
}
