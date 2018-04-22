using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placedTile{
    public boardTile.tileSquare square { get; private set; }
    public int boardX { get; private set; }
    public int boardY { get; private set; }
    public bool hasEnemies;

    public placedTile(int x, int y, boardTile.tileSquare tSquare) {
        boardX = x;
        boardY = y;
        square = tSquare;
    }
    
}
