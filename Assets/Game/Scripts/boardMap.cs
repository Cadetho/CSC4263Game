using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardMap{
    private int boardSizeX, boardSizeY;
    private boardTile[,] boardTiles;
    public List<placedTile> board{ get; private set; }


    public boardMap() {
        boardSizeX = boardSizeY = 101;
        boardTiles = new boardTile[boardSizeY, boardSizeX];
        board = new List<placedTile>();
    }


    public bool placeTile(int x, int y, boardTile newTile, bool firstTile=false) {
        if (checkTileSpot(x, y, newTile) || firstTile) {
            Debug.Log(newTile);
            for (int i = 0; i < newTile.yLen; i++) {
                for (int j = 0; j < newTile.xLen; j++) {
                    board.Add(new placedTile(x+j, y+i, newTile.squares[i, j]));
                }
            }
            return true;
        }
        return false;
    }

    /*
     * Check a tile to the board location
     * x,y location of top-left corner of tile
     * check the location to ensure there are not rooms where tile will place rooms
     */
    public bool checkTileSpot(int x, int y, boardTile newTile) {
        if (checkLocation(x, y, newTile) && checkForOneDoor(x,y,newTile)){
            return true;
        } else {
            return false;
        }
    }

    private bool checkForOneDoor(int x, int y, boardTile newTile) {
        List<boardTile.Door> doors = newTile.getAllDoors();
        bool doorCheck = false;
        foreach(boardTile.Door door in doors) {
            switch (door.facing) {
                case boardTile.edgeLoc.top:
                    if (boardTiles[y - 1, x] != null) {
                        if (boardTiles[y - 1, x].squares[0, 0].bottomIsDoor()) {
                            doorCheck = true;
                        }
                    }
                    break;
                case boardTile.edgeLoc.right:
                    if (boardTiles[y, x + 1] != null) {
                        if (boardTiles[y, x + 1].squares[0, 0].leftIsDoor()) {
                            doorCheck = true;
                        }
                    }
                    break;
                case boardTile.edgeLoc.bottom:
                    if (boardTiles[y + 1, x] != null) {
                        if (boardTiles[y + 1, x].squares[0, 0].topIsDoor()) {
                            doorCheck = true;
                        }
                    }
                    break;
                case boardTile.edgeLoc.left:
                    if (boardTiles[y, x - 1] != null) {
                        if (boardTiles[y, x - 1].squares[0, 0].rightIsDoor()) {
                            doorCheck = true;
                        }
                    }
                    break;
            }
        }
        return doorCheck;
    }

    private bool checkLocation(int x, int y, boardTile newTile) {
        int xLen = newTile.xLen;
        int yLen = newTile.yLen;
        bool clear = true;
        for (int i = 0; i < yLen; i++) {
            for (int j = 0; j < xLen; j++) {
                if (boardTiles[y + i, x + j] != null && !newTile.squares[i, j].isEmpty()) {
                    clear = false;
                }
            }
        }
        return clear;
    }
}
