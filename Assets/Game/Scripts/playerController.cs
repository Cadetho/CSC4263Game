using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour{

    [SerializeField]
    private playerHand hand;
    public GameManager gm;



    public void drawCards(int num) {
        for (int i = 0; i < num; i++) {
            if (gm.deckNotEmpty()) {
                hand.addCard(gm.drawCard());
            }
        }
    }
    public void hideHand() {
        hand.hide();
    }
    public void showHand() {
        hand.show();
    }
    public void replaceCard(GameObject handCard) {
        boardTile newTile = gm.drawCard();
        hand.replaceCard(newTile, handCard);
    }

    private void Start() {
        hand = GetComponent<playerHand>();
    }

    private void Update() {
        
    }
}
