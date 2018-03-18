using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // A set of Quaternions used for prefab rotations
    private Quaternion qforward = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    private Quaternion qright = Quaternion.LookRotation(Vector3.right, Vector3.up);
    private Quaternion qback = Quaternion.LookRotation(Vector3.back, Vector3.up);
    private Quaternion qleft = Quaternion.LookRotation(Vector3.left, Vector3.up);

    //private List<Tile> tilemap = new List<Tile>();
    public boardMap masterBoard = new boardMap();
    private List<boardTile> tileDeck = new List<boardTile>();
    private List<playerController> players = new List<playerController>();
    public GameObject prefabwall;
    public GameObject prefabdoor;
    public GameObject prefabopen;
    public GameObject prefabroom;
    public GameObject prefabDoorRightOpen;
    public GameObject prefabDoorLeftOpen;
    public GameObject prefabDoorBothOpen;
    public GameObject prefabWallRightOpen;
    public GameObject prefabWallLeftOpen;
    public GameObject prefabWallBothOpen;
    public ObjectPlacementController boardController;
    public const int tilesize = 1;
    public GameObject selectedCard;

    public const int cardCount = 6;
    private const int handSize = 7;
    public playerController mainPlayer;

	void Start () {
        players.Add(mainPlayer);
        tileDeck = tileDeckGenerator.generateDeck(cardCount);
        foreach(playerController player in players) {
            player.drawCards(handSize);
        }
        placeDefaultTile();
        //TestMap();

        //GenerateLevel();
    }

    public boardTile drawCard() {
        boardTile newCard;
        newCard = tileDeck[0];
        tileDeck.RemoveAt(0);
        return newCard;
    }

    public bool deckNotEmpty() {
        return (tileDeck.Count > 0);
    }
    public void placeDefaultTile() {
        boardTile defaultTile = new boardTile(1, 1);
        defaultTile.setSquare(0, 0, new boardTile.Wall[] { boardTile.Wall.door, boardTile.Wall.door, boardTile.Wall.door, boardTile.Wall.door });
        masterBoard.placeTile(0 , 0, defaultTile, true);
        GameObject gridCard = boardController.createGridCard(defaultTile);
        boardController.setGridCardSpot(0, 0, defaultTile, gridCard);
        boardController.setGridCardSpot(0, 0, defaultTile, gridCard);
    }

    public void selectCard(cardController card){
        selectedCard = boardController.createGridCard(card.tile);
        //boardController.selectedCard(selectedCard);
    }
    public void placeCard(int x, int y, boardTile tile) {
        masterBoard.placeTile(x, y, tile);
    }
    public bool checkBoardSpot(int x, int y, boardTile tile) {
        return masterBoard.checkTileSpot(x, y, tile);
    }
    public void GenerateLevel() {
        // TODO: match doors & remove inside corner square in 2x2 tile
        List<GameObject> prefabs = new List<GameObject>();
        Debug.Log("creating tile at: ");
        foreach (placedTile t in masterBoard.board) {

            boardTile.Wall top = t.square.getTop();
            boardTile.Wall right = t.square.getRight();
            boardTile.Wall bottom = t.square.getBottom();
            boardTile.Wall left = t.square.getLeft();
            Vector3 boardPos = GridToPosition(t.boardX, t.boardY);
            prefabs.Add(Instantiate(prefabroom, boardPos, qforward));
            if (top == boardTile.Wall.fullWall || top == boardTile.Wall.door) {
                if (top == boardTile.Wall.fullWall) {

                    if (right == boardTile.Wall.noWall && left == boardTile.Wall.noWall) {
                       prefabs.Add(Instantiate(prefabWallBothOpen, gridToTop(t.boardX, t.boardY), qforward));
                    } else if(right == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallRightOpen, gridToTop(t.boardX, t.boardY), qforward));
                    } else if(left == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallLeftOpen, gridToTop(t.boardX, t.boardY), qforward));
                    } else {
                        prefabs.Add(Instantiate(prefabwall, gridToTop(t.boardX, t.boardY), qforward));
                    }
                } else {
                    if (right == boardTile.Wall.noWall && left == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorBothOpen, gridToTop(t.boardX, t.boardY), qforward));
                    } else if (right == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorRightOpen, gridToTop(t.boardX, t.boardY), qforward));
                    } else if (left == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorLeftOpen, gridToTop(t.boardX, t.boardY), qforward));
                    } else {
                        prefabs.Add(Instantiate(prefabdoor, gridToTop(t.boardX, t.boardY), qforward));
                    }
                }
            }

            if (right == boardTile.Wall.fullWall || right == boardTile.Wall.door) {
                if(right == boardTile.Wall.fullWall) {
                    if (bottom == boardTile.Wall.noWall && top == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallBothOpen, gridToRight(t.boardX, t.boardY), qright));
                    } else if (bottom == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallRightOpen, gridToRight(t.boardX, t.boardY), qright));
                    } else if (top == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallLeftOpen, gridToRight(t.boardX, t.boardY), qright));
                    } else {
                        prefabs.Add(Instantiate(prefabwall, gridToRight(t.boardX, t.boardY), qright));
                    }
                } else {
                    if (bottom == boardTile.Wall.noWall && top == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorBothOpen, gridToRight(t.boardX, t.boardY), qright));
                    } else if (bottom == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorRightOpen, gridToRight(t.boardX, t.boardY), qright));
                    } else if (top == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorLeftOpen, gridToRight(t.boardX, t.boardY), qright));
                    } else {
                        prefabs.Add(Instantiate(prefabdoor, gridToRight(t.boardX, t.boardY), qright));
                    }
                }
            }

            if (bottom == boardTile.Wall.fullWall || bottom == boardTile.Wall.door) {
                if (bottom == boardTile.Wall.fullWall) {
                    if (left == boardTile.Wall.noWall && right == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallBothOpen, gridToBottom(t.boardX, t.boardY), qback));
                    } else if (left == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallRightOpen, gridToBottom(t.boardX, t.boardY), qback));
                    } else if (right == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallLeftOpen, gridToBottom(t.boardX, t.boardY), qback));
                    } else {
                        prefabs.Add(Instantiate(prefabwall, gridToBottom(t.boardX, t.boardY), qback));
                    }
                } else {
                    if (left == boardTile.Wall.noWall && right == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorBothOpen, gridToBottom(t.boardX, t.boardY), qback));
                    } else if (left == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorRightOpen, gridToBottom(t.boardX, t.boardY), qback));
                    } else if (right == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorLeftOpen, gridToBottom(t.boardX, t.boardY), qback));
                    } else {
                        prefabs.Add(Instantiate(prefabdoor, gridToBottom(t.boardX, t.boardY), qback));
                    }
                }
            }

            if (left == boardTile.Wall.fullWall || left == boardTile.Wall.door) {
                if(left == boardTile.Wall.fullWall) {
                    if (top == boardTile.Wall.noWall && bottom == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallBothOpen, gridToLeft(t.boardX, t.boardY), qleft));
                    } else if (top == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallRightOpen, gridToLeft(t.boardX, t.boardY), qleft));
                    } else if (bottom == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabWallLeftOpen, gridToLeft(t.boardX, t.boardY), qleft));
                    } else {
                        prefabs.Add(Instantiate(prefabwall, gridToLeft(t.boardX, t.boardY), qleft));
                    }
                } else {
                    if (top == boardTile.Wall.noWall && bottom == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorBothOpen, gridToLeft(t.boardX, t.boardY), qleft));
                    } else if (top == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorRightOpen, gridToLeft(t.boardX, t.boardY), qleft));
                    } else if (bottom == boardTile.Wall.noWall) {
                        prefabs.Add(Instantiate(prefabDoorLeftOpen, gridToLeft(t.boardX, t.boardY), qleft));
                    } else {
                        prefabs.Add(Instantiate(prefabdoor, gridToLeft(t.boardX, t.boardY), qleft));
                    }
                }
            }

            //int tw = t.Width;
            //int th = t.Height;

            //int[,] squares = t.GetRelativeSpacesOccupied();

            //for (int i = 0; i < squares.Length / 2; i++)
            //{
            //int sqx = squares[i, 0];
            //int sqy = squares[i, 1];
            //int x = t.XPos + sqx;
            //int y = t.YPos + sqy;

            // Place the room center/floor prefab
            //prefabs.Add(Instantiate(prefabroom, GridToPosition(x, y), qforward));
            // Place the wall/door/open prefabs
            //bool topsquare = (sqy == 0);
            //    if (!topsquare) // check if there's a square above
            //    {
            //        topsquare = true;
            //        for (int j = 0; j < squares.Length / 2; j++)
            //        {
            //            if (squares[j, 0] == sqx && squares[j, 1] == 0)
            //            {
            //                topsquare = false;
            //                break;
            //            }
            //        }
            //    }
            //    if (topsquare)
            //    {
            //        prefabs.Add(Instantiate((t.TopWall[sqx] == Wall.Door ? prefabdoor : prefabwall), GridToPosition(x, y), qforward));
            //    }
            //    else
            //    {
            //        prefabs.Add(Instantiate(prefabopen, GridToPosition(x, y), qforward));
            //    }

            //    bool rightsquare = (sqx == tw - 1);
            //    if (!rightsquare) // check if there's a square to the right
            //    {
            //        rightsquare = true;
            //        for (int j = 0; j < squares.Length / 2; j++)
            //        {
            //            if (squares[j, 1] == sqy && squares[j, 0] == 1)
            //            {
            //                rightsquare = false;
            //                break;
            //            }
            //        }
            //    }
            //    if (rightsquare)
            //    {
            //        prefabs.Add(Instantiate((t.RightWall[sqy] == Wall.Door ? prefabdoor : prefabwall), GridToPosition(x, y), qright));
            //    }
            //    else
            //    {
            //        prefabs.Add(Instantiate(prefabopen, GridToPosition(x, y), qright));
            //    }

            //    bool bottomsquare = (sqy == th - 1);
            //    if (!bottomsquare) // check if there's a square below
            //    {
            //        bottomsquare = true;
            //        for (int j = 0; j < squares.Length / 2; j++)
            //        {
            //            if (squares[j, 0] == sqx && squares[j, 1] == 1)
            //            {
            //                bottomsquare = false;
            //                break;
            //            }
            //        }
            //    }
            //    if (bottomsquare)
            //    {
            //        int wi = tw - sqx - 1;
            //        prefabs.Add(Instantiate((t.BottomWall[wi] == Wall.Door ? prefabdoor : prefabwall), GridToPosition(x, y), qback));
            //    }
            //    else
            //    {
            //        prefabs.Add(Instantiate(prefabopen, GridToPosition(x, y), qback));
            //    }

            //    bool leftsquare = (sqx == 0);
            //    if (!leftsquare) // check if there's a square to the left
            //    {
            //        leftsquare = true;
            //        for (int j = 0; j < squares.Length / 2; j++)
            //        {
            //            if (squares[j, 1] == sqy && squares[j, 0] == 0)
            //            {
            //                leftsquare = false;
            //                break;
            //            }
            //        }
            //    }
            //    if (leftsquare)
            //    {
            //        int wi = th - sqy - 1;
            //        prefabs.Add(Instantiate((t.LeftWall[wi] == Wall.Door ? prefabdoor : prefabwall), GridToPosition(x, y), qleft));
            //    }
            //    else
            //    {
            //        prefabs.Add(Instantiate(prefabopen, GridToPosition(x, y), qleft));
            //    }
            //}
            //}
            //foreach (GameObject pf in prefabs)
            //{
            //    if (pf.GetType() == prefabdoor.GetType())
            //    {
            //        if (pf.name == string.Format("{0}(Clone)", prefabdoor.name))
            //        {

            //        }
            //    }
            //}
        }
    }


    // Initialized Vector3 used for prefab positions. Changes each time TilePos is called.
    // This is used so we don't have to allocate memory for a new Vector3 each time this method is called.
    private Vector3 pos = new Vector3();
    /// <summary>
    /// Converts tilemap grid coordinates to a Vector3 to be used for transform positions, based on the tilesize field.
    /// </summary>
    /// <param name="x">Tilemap x coordinate</param>
    /// <param name="y">Tilemap y coordinate</param>
    /// <returns>A Vector3 of the position in 3D space.</returns>
    /// 
    public Vector3 GridToPosition(int x, int y)
    {
        pos.Set(x, 0, -y);
        return pos;
    }
    public Vector3 gridToTop(int x, int y) {
        pos.Set(x,0, -y);
        return pos;
    }
    public Vector3 gridToRight(int x, int y) {
        pos.Set(x , 0, -y-1);
        return pos;
    }
    public Vector3 gridToBottom(int x, int y) {
        pos.Set(x - 1, 0, -y - 1);
        return pos;
    }
    public Vector3 gridToLeft(int x, int y) {
        pos.Set(x -1, 0, -y);
        return pos;
    }
    // GENERATE A TEST TILEMAP
    void TestMap()
    {
        //Tile.DoorProbability = 0.3f;
        //Wall wc = Wall.Closed;
        //Wall wd = Wall.Door;

        boardTile.Wall door = boardTile.Wall.door;
        boardTile.Wall wall = boardTile.Wall.fullWall;
        boardTile.Wall open = boardTile.Wall.noWall;
        //Tile t = null;
        boardTile t1 = new boardTile(1, 1);
        boardTile.Wall[] wall1 = new boardTile.Wall[4];
        wall1[0] = door;
        wall1[1] = door;
        wall1[2] = door;
        wall1[3] = door;
        t1.setSquare(0, 0, wall1);
        boardTile t2 = new boardTile(1, 1);
        t2.setSquare(0, 0, new boardTile.Wall[] { door, wall, door, wall });
        boardTile t3 = new boardTile(2, 2);
        t3.setSquare(0, 0, new boardTile.Wall[] { door, open, open, door});
        t3.setSquare(1, 0, new boardTile.Wall[] { wall, door, open, open });
        t3.setSquare(0, 1, new boardTile.Wall[] { open, open, wall, door});
        t3.setSquare(1, 1, new boardTile.Wall[] { open, door, wall, open});
        boardTile t4 = new boardTile(1, 1);
        t4.setSquare(0, 0, new boardTile.Wall[] { wall, door, wall, wall });


        masterBoard.placeTile(50, 50, t1, true);
        masterBoard.placeTile(50, 49, t2);
        masterBoard.placeTile(51, 50, t3);
        //masterBoard.placeTile(49, 50, t4);

        //tilemap.Add(new TileCorner(0, 0, 0)
        //{
        //    TopWall = new Wall[] { wc, wc },
        //    RightWall = new Wall[] { wc, wd },
        //    BottomWall = new Wall[] { wd, wc },
        //    LeftWall = new Wall[] { wc, wc }
        //});
        //tilemap.Add(new Tile1x1(1, 1)
        //{
        //    TopWall = new Wall[] { wd },
        //    RightWall = new Wall[] { wd },
        //    BottomWall = new Wall[] { wc },
        //    LeftWall = new Wall[] { wd }
        //});
        //t = tilemap[tilemap.Count - 1];
        //tilemap.Add(new Tile2x1(2, 1, 0)
        //{
        //    TopWall = new Wall[] { wc, wc },
        //    RightWall = new Wall[] { wd },
        //    BottomWall = new Wall[] { wc, wd },
        //    LeftWall = new Wall[] { wd }
        //});
        //t = tilemap[tilemap.Count - 1];
        //tilemap.Add(new Tile1x1(4, 1)
        //{
        //    TopWall = new Wall[] { wc },
        //    RightWall = new Wall[] { wc },
        //    BottomWall = new Wall[] { wd },
        //    LeftWall = new Wall[] { wd }
        //});
        //t = tilemap[tilemap.Count - 1];
        //tilemap.Add(new Tile1x1(1, 2)
        //{
        //    TopWall = new Wall[] { wc },
        //    RightWall = new Wall[] { wd },
        //    BottomWall = new Wall[] { wd },
        //    LeftWall = new Wall[] { wc }
        //});
        //t = tilemap[tilemap.Count - 1];
        //tilemap.Add(new Tile1x1(2, 2)
        //{
        //    TopWall = new Wall[] { wd },
        //    RightWall = new Wall[] { wd },
        //    BottomWall = new Wall[] { wd },
        //    LeftWall = new Wall[] { wd }
        //});
        //t = tilemap[tilemap.Count - 1];
        //tilemap.Add(new Tile2x2(3, 2)
        //{
        //    TopWall = new Wall[] { wc, wd },
        //    RightWall = new Wall[] { wc, wc },
        //    BottomWall = new Wall[] { wd, wd },
        //    LeftWall = new Wall[] { wd, wd }
        //});
        //t = tilemap[tilemap.Count - 1];
        //tilemap.Add(new Tile1x1(1, 3)
        //{
        //    TopWall = new Wall[] { wd },
        //    RightWall = new Wall[] { wd },
        //    BottomWall = new Wall[] { wc },
        //    LeftWall = new Wall[] { wc }
        //});
        //t = tilemap[tilemap.Count - 1];
        //tilemap.Add(new Tile2x1(2, 3, 90)
        //{
        //    TopWall = new Wall[] { wd },
        //    RightWall = new Wall[] { wd, wc },
        //    BottomWall = new Wall[] { wc },
        //    LeftWall = new Wall[] { wd, wd }
        //});
        //t = tilemap[tilemap.Count - 1];
        //tilemap.Add(new Tile1x1(1, 4)
        //{
        //    TopWall = new Wall[] { wc },
        //    RightWall = new Wall[] { wd },
        //    BottomWall = new Wall[] { wc },
        //    LeftWall = new Wall[] { wc }
        //});
        //t = tilemap[tilemap.Count - 1];
        //tilemap.Add(new Tile2x1(3, 4)
        //{
        //    TopWall = new Wall[] { wd, wd },
        //    RightWall = new Wall[] { wc },
        //    BottomWall = new Wall[] { wc, wc },
        //    LeftWall = new Wall[] { wc }
        //});
        //t = tilemap[tilemap.Count - 1];
        //Debug.LogFormat( "Tile Created \n" + 
        //    			 "Tile GridPos: {0},{1} \n" +
        //                 "Tile Rotation: {2} \n" +
        //                 "Tile Size: {3},{4} \n", t.XPos, t.YPos, t.Rotation, t.Width, t.Height);


        //tilemap.Add(new Tile1x1(0, 1));
        //tilemap.Add(new Tile2x2(0, 0, 90));
        //tilemap.Add(new Tile1x1(0, 0)
        //{
        //    TopWall = new Wall[] { Wall.Closed },
        //    RightWall = new Wall[] { Wall.Closed },
        //    BottomWall = new Wall[] { Wall.Closed },
        //    LeftWall = new Wall[] { Wall.Closed }
        //});

    }
}
