using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour{

    playerHand hand = new playerHand();
    GameManager gm;
    bool showHand;
    
    public playerController(GameManager _gm) {
        gm = _gm;
    }

    public void drawCards(int num) {
        for (int i = 0; i < num; i++) {
            if (gm.deckNotEmpty()) {
                hand.addCard(gm.drawCard());
            }
        }
    }


    private void Update() {
        
    }
}
