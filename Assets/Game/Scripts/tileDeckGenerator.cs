using System.Collections;
using System.Collections.Generic;
using System;

public class tileDeckGenerator{
    static double doorP = 0.6;
    static double lengthTwoP = 0.2;
    static double lengthThreeP = 0.02;
    static double squareIsEmptyP = 0.1;

    public static List<boardTile> generateDeck(int cardCount, int seed = 0) {
        List<boardTile> newDeck = new List<boardTile>();
        Random randomNum = new Random();
        for (int i = 0; i < cardCount; i++) {
            newDeck.Add(generateTile(randomNum));
        }
        return newDeck;
    }

    private static boardTile generateTile(Random randomNum) {
        boardTile tile;
        double xLenChance = randomNum.NextDouble();
        double yLenChance = randomNum.NextDouble();
        int xLen, yLen = 0;

        if(xLenChance < lengthThreeP) {
            xLen = 3;
        }else if(xLenChance < lengthTwoP) {
            xLen = 2;
        } else {
            xLen = 1;
        }

        if (yLenChance < lengthThreeP) {
            yLen = 3;
        } else if (yLenChance < lengthTwoP) {
            yLen = 2;
        } else {
            yLen = 1;
        }
        boardTile.Wall top = boardTile.Wall.fullWall;
        boardTile.Wall right = boardTile.Wall.fullWall;
        boardTile.Wall bottom = boardTile.Wall.fullWall;
        boardTile.Wall left = boardTile.Wall.fullWall;
        boardTile.Wall door = boardTile.Wall.door;
        tile = new boardTile(xLen, yLen);
        bool hasDoor = false;
        for(int y = 0; y<yLen; y++) {
            for(int x = 0;x<xLen; x++) {
                if (randomNum.NextDouble() < squareIsEmptyP) {
                    tile.setNewSquare(x, y, true);
                } else {
                    tile.setNewSquare(x, y, false);
                }
            }
        }

        bool setTop;
        bool setRight;
        bool setBottom;
        bool setLeft;


        //Set each square in the new tile
        for (int y = 0; y < yLen; y++) {
            for (int x = 0; x < xLen; x++) {
                setTop = true;
                setRight = true;
                setBottom = true;
                setLeft = true;
                if (y+1 < yLen) {
                    setBottom = !(tile.squares[y + 1, x].isEmpty());
                }
                if(y-1 > 0) {
                    setTop = !(tile.squares[y - 1, x].isEmpty());
                }
                if(x+1 < xLen) {
                    setRight = !(tile.squares[y, x + 1].isEmpty());
                }
                if(x-1 > 0){
                    setLeft = !(tile.squares[y, x - 1].isEmpty());
                }
                top = (setTop) ? ((randomNum.NextDouble() < doorP) ? boardTile.Wall.door : boardTile.Wall.fullWall) : boardTile.Wall.noWall;
                right = (setRight) ? ((randomNum.NextDouble() < doorP) ? boardTile.Wall.door : boardTile.Wall.fullWall) : boardTile.Wall.noWall;
                bottom = (setBottom) ? ((randomNum.NextDouble() < doorP) ? boardTile.Wall.door : boardTile.Wall.fullWall) : boardTile.Wall.noWall;
                left = (setTop) ? ((randomNum.NextDouble() < doorP) ? boardTile.Wall.door : boardTile.Wall.fullWall) : boardTile.Wall.noWall;
                if(top == door || right == door || bottom == door || left == door) {
                    hasDoor = true;
                }
                tile.setSquare(x, y, new boardTile.Wall[]{top, right, bottom, left});
            }
        }


        //Make sure each new tile has at least one outward facing door (does not work atm, infinite loop)
        //while (!hasDoor) {
        //    int x = randomNum.Next(xLen);
        //    int y = randomNum.Next(yLen);
        //    if (!tile.squares[y, x].isEmpty()) {
        //        while (!hasDoor) {
        //            int dx = randomNum.Next(2);
        //            int dy = randomNum.Next(2);
        //            if(tile.squares[y,x].sqWalls[dy,dx] != boardTile.Wall.noWall) {
        //                tile.squares[y, x].sqWalls[dy, dx] = boardTile.Wall.door;
        //            }
        //        }
        //    }
        //}
        return tile;
    }
}
