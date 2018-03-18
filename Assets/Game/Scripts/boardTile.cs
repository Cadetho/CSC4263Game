using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boardTile{
    public int xLen { get; private set; }
    public int yLen { get; private set; }
    public tileSquare[,] squares { get; private set; }


    public enum Wall { fullWall, door, noWall };
    public enum edgeLoc { top, bottom, left, right };
    //public List<Door> doors = new List<Door>();


    private GameObject owner;
    


    public boardTile(int _xLen, int _yLen) {
        xLen = _xLen;
        yLen = _yLen;
        squares = new tileSquare[yLen, xLen];
    }


    public void setSquare(int x, int y, Wall[] walls) {
        squares[y, x] = new tileSquare(walls[0], walls[1], walls[2], walls[3], x, y);
    }


    public void setNewSquare(int x, int y, bool empty) {
        squares[y, x] = new tileSquare(empty);
    }


    //Owner of the tile, null if nobody owns it
    public void setOwner(GameObject _owner) {
        owner = _owner;
    }

    //Called by owner object to rotate tile during placement phase for placement
    public void rotateTile(bool clockwise){
        squares = rotateMatrix(squares, clockwise);
        for(int i=0;i<yLen;i++){
            for (int j = 0; j < xLen; j++) {
                squares[i,j].rotateSquare(clockwise);
            }
        }
    }


    public List<Door> getAllDoors() {
        List<Door> tileDoors = new List<Door>();
        for(int i = 0; i < yLen; i++) {
            for(int j = 0; j < xLen; j++) {
                tileDoors.AddRange(squares[i, j].getDoors());
            }
        }
        return tileDoors;
    }

    /*Rotate any 2D matrix, used for Tile and then Tile's squares
     *clockwise = true    rotate clockwise 
     *clockwise = false   rotate counterclockwise
     *
     * ex:
     * matrixToRotate = rotateMatrix<ObjectType>(matrixToRotate, true)
     * 
     */
    public static T[,] rotateMatrix<T>(T[,] oldMatrix, bool clockwise) {
        int m = oldMatrix.GetLength(0);
        int n = oldMatrix.GetLength(1);
        T[,] newMatrix = new T[oldMatrix.GetLength(1), oldMatrix.GetLength(0)];

        if (clockwise) {
            for(int i = 0; i < n; i++) {
                for(int j = 0; j < m; j++) {
                    newMatrix[i,j] = oldMatrix[m-j-1, i];
                }
            }
        } else {
            for(int i = 0; i < n; i++) {
                for(int j=0; j < m; j++) {
                    newMatrix[i, j] = oldMatrix[j, n - i - 1];
                }
            }
        }
        return newMatrix;
    }





    public struct Door {
        public int x, y;
        public edgeLoc facing;
        public Door(int _x, int _y, edgeLoc _facing) {
            x = _x;
            y = _y;
            facing = _facing;
        }
        public void updateFace(edgeLoc newFace) {
            facing = newFace;
        }
    }

    public struct tileSquare{
        /*
         * Squares walls are defined in 2x2 matrix to make rotations easier
         * Walls will always be in clockwise order:
         * | Top   Right |
         * | Bottom Left |
         */
        public Wall[,] sqWalls;
        bool empty;
        List<Door> sqDoors;


        public tileSquare(Wall _top, Wall _right, Wall _bottom, Wall _left, int x, int y) {
            sqDoors = new List<Door>();
            sqWalls = new Wall[2,2];
            sqWalls[0,0] = _top;
            if (_top == Wall.door) { sqDoors.Add(new Door(x, y, edgeLoc.top)); }
            sqWalls[0,1] = _right;
            if(_right == Wall.door) { sqDoors.Add(new Door(x, y, edgeLoc.right)); }
            sqWalls[1,0] = _bottom;
            if(_bottom == Wall.door) { sqDoors.Add(new Door(x, y, edgeLoc.bottom)); }
            sqWalls[1,1] = _left;
            if(_left == Wall.door) { sqDoors.Add(new Door(x, y, edgeLoc.left)); }
            empty = false;
        }
        
        public bool bottomIsDoor() {
            return (sqWalls[1, 0] == Wall.door) ? true : false;
        }
        public bool topIsDoor() {
            return (sqWalls[0, 0] == Wall.door) ? true : false;
        }
        public bool rightIsDoor() {
            return (sqWalls[0, 1] == Wall.door) ? true : false;
        }
        public bool leftIsDoor() {
            return (sqWalls[1, 1] == Wall.door) ? true : false;
        }
        public bool isEmpty() {
            return empty;
        }

        public Wall getTop() {
            return sqWalls[0, 0];
        }
        public Wall getRight() {
            return sqWalls[0, 1];
        }
        public Wall getBottom() {
            return sqWalls[1, 0];
        }
        public Wall getLeft() {
            return sqWalls[1, 1];
        }
        /*empty square location
         *used when a tile is 2x2 or larger but missing a room
         */
        public tileSquare(bool _empty) {
            sqDoors = null;
            sqWalls = null;
            empty = _empty;
        }
        public List<Door> getDoors() {
            return sqDoors;
        }
        //called to rotate each square on a tile when the tile rotates
        public void rotateSquare(bool clockwise) {
            sqWalls = rotateMatrix<Wall>(sqWalls, clockwise);
            updateDoors(clockwise);
        }


        void updateDoors(bool clockwise) {
            foreach(Door d in sqDoors) {
                if (clockwise) {
                    switch (d.facing) {
                        case edgeLoc.top:
                            d.updateFace(edgeLoc.right);
                            break;
                        case edgeLoc.right:
                            d.updateFace(edgeLoc.bottom);
                            break;
                        case edgeLoc.bottom:
                            d.updateFace(edgeLoc.left);
                            break;
                        case edgeLoc.left:
                            d.updateFace(edgeLoc.top);
                            break;
                    }
                } else {
                    switch (d.facing) {
                        case edgeLoc.top:
                            d.updateFace(edgeLoc.left);
                            break;
                        case edgeLoc.right:
                            d.updateFace(edgeLoc.top);
                            break;
                        case edgeLoc.bottom:
                            d.updateFace(edgeLoc.right);
                            break;
                        case edgeLoc.left:
                            d.updateFace(edgeLoc.bottom);
                            break;
                    }
                }
            }
        }
    }
}
