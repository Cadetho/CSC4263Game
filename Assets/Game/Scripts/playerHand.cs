using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHand {
    private readonly int handSize = 7;
    List<boardTile> tiles = new List<boardTile>();

    public bool addCard(boardTile newtile) {
        bool added = true;
        if(tiles.Count < handSize + 1) {
            tiles.Add(newtile);
        }
        return added;
    }
   
}
