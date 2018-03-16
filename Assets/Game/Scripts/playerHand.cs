using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerHand : MonoBehaviour{
    private readonly int handSize = 7;
    List<boardTile> tiles = new List<boardTile>();
    private int handCount = 0;
    public GameObject handPanel;
    public GameObject cardPrefab;

    private int cardNextOffset = 1;
    private int cardOffsetAmount = 3;
    public bool addCard(boardTile newtile) {
        bool added = false;
        if(tiles.Count < handSize + 1) {
            tiles.Add(newtile);
            added = true;
            showNewCard(newtile);
        }

        return added;
    }
   private void showNewCard(boardTile newTile) {
        GameObject newTileButton = Instantiate(cardPrefab);
    }
}
