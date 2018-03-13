using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // A set of Quaternions used for prefab rotations
    private Quaternion qforward = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    private Quaternion qright = Quaternion.LookRotation(Vector3.right, Vector3.up);
    private Quaternion qback = Quaternion.LookRotation(Vector3.back, Vector3.up);
    private Quaternion qleft = Quaternion.LookRotation(Vector3.left, Vector3.up);

    private List<Tile> tilemap = new List<Tile>();

    public GameObject prefabwall;
    public GameObject prefabdoor;
    public GameObject prefabopen;
    public GameObject prefabroom;
    public const int tilesize = 16;

	void Start () {
        // Create a test tilemap.
        TestMap();

        GenerateLevel();
    }
    
    public void GenerateLevel()
    {
        // TODO: match doors & remove inside corner square in 2x2 tile
        List<GameObject> prefabs = new List<GameObject>();
        foreach (Tile t in tilemap)
        {
            int tw = t.Width;
            int th = t.Height;

            int[,] squares = t.GetRelativeSpacesOccupied();

            for (int i = 0; i < squares.Length / 2; i++)
            {
                int sqx = squares[i, 0];
                int sqy = squares[i, 1];
                int x = t.XPos + sqx;
                int y = t.YPos + sqy;

                // Place the room center/floor prefab
                prefabs.Add(Instantiate(prefabroom, GridToPosition(x, y), qforward));
                // Place the wall/door/open prefabs
                bool topsquare = (sqy == 0);
                if (!topsquare) // check if there's a square above
                {
                    topsquare = true;
                    for (int j = 0; j < squares.Length / 2; j++)
                    {
                        if (squares[j, 0] == sqx && squares[j, 1] == 0)
                        {
                            topsquare = false;
                            break;
                        }
                    }
                }
                if (topsquare)
                {
                    prefabs.Add(Instantiate((t.TopWall[sqx] == Wall.Door ? prefabdoor : prefabwall), GridToPosition(x, y), qforward));
                }
                else
                {
                    prefabs.Add(Instantiate(prefabopen, GridToPosition(x, y), qforward));
                }

                bool rightsquare = (sqx == tw - 1);
                if (!rightsquare) // check if there's a square to the right
                {
                    rightsquare = true;
                    for (int j = 0; j < squares.Length / 2; j++)
                    {
                        if (squares[j, 1] == sqy && squares[j, 0] == 1)
                        {
                            rightsquare = false;
                            break;
                        }
                    }
                }
                if (rightsquare)
                {
                    prefabs.Add(Instantiate((t.RightWall[sqy] == Wall.Door ? prefabdoor : prefabwall), GridToPosition(x, y), qright));
                }
                else
                {
                    prefabs.Add(Instantiate(prefabopen, GridToPosition(x, y), qright));
                }

                bool bottomsquare = (sqy == th - 1);
                if (!bottomsquare) // check if there's a square below
                {
                    bottomsquare = true;
                    for (int j = 0; j < squares.Length / 2; j++)
                    {
                        if (squares[j, 0] == sqx && squares[j, 1] == 1)
                        {
                            bottomsquare = false;
                            break;
                        }
                    }
                }
                if (bottomsquare)
                {
                    int wi = tw - sqx - 1;
                    prefabs.Add(Instantiate((t.BottomWall[wi] == Wall.Door ? prefabdoor : prefabwall), GridToPosition(x, y), qback));
                }
                else
                {
                    prefabs.Add(Instantiate(prefabopen, GridToPosition(x, y), qback));
                }

                bool leftsquare = (sqx == 0);
                if (!leftsquare) // check if there's a square to the left
                {
                    leftsquare = true;
                    for (int j = 0; j < squares.Length / 2; j++)
                    {
                        if (squares[j, 1] == sqy && squares[j, 0] == 0)
                        {
                            leftsquare = false;
                            break;
                        }
                    }
                }
                if (leftsquare)
                {
                    int wi = th - sqy - 1;
                    prefabs.Add(Instantiate((t.LeftWall[wi] == Wall.Door ? prefabdoor : prefabwall), GridToPosition(x, y), qleft));
                }
                else
                {
                    prefabs.Add(Instantiate(prefabopen, GridToPosition(x, y), qleft));
                }
            }
        }
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


    // Initialized Vector3 used for prefab positions. Changes each time TilePos is called.
    // This is used so we don't have to allocate memory for a new Vector3 each time this method is called.
    private Vector3 pos = new Vector3();
    /// <summary>
    /// Converts tilemap grid coordinates to a Vector3 to be used for transform positions, based on the tilesize field.
    /// </summary>
    /// <param name="x">Tilemap x coordinate</param>
    /// <param name="y">Tilemap y coordinate</param>
    /// <returns>A Vector3 of the position in 3D space.</returns>
    public Vector3 GridToPosition(int x, int y)
    {
        pos.Set(x*tilesize, 0, -y*tilesize);
        return pos;
    }
    
    // GENERATE A TEST TILEMAP
    void TestMap()
    {
        Tile.DoorProbability = 0.3f;
        Wall wc = Wall.Closed;
        Wall wd = Wall.Door;
        Tile t = null;
        tilemap.Add(new TileCorner(0, 0, 0)
        {
            TopWall = new Wall[] { wc, wc },
            RightWall = new Wall[] { wc, wd },
            BottomWall = new Wall[] { wd, wc },
            LeftWall = new Wall[] { wc, wc }
        });
        tilemap.Add(new Tile1x1(1, 1)
        {
            TopWall = new Wall[] { wd },
            RightWall = new Wall[] { wd },
            BottomWall = new Wall[] { wc },
            LeftWall = new Wall[] { wd }
        });
        t = tilemap[tilemap.Count - 1];
        tilemap.Add(new Tile2x1(2, 1, 0)
        {
            TopWall = new Wall[] { wc, wc },
            RightWall = new Wall[] { wd },
            BottomWall = new Wall[] { wc, wd },
            LeftWall = new Wall[] { wd }
        });
        t = tilemap[tilemap.Count - 1];
        tilemap.Add(new Tile1x1(4, 1)
        {
            TopWall = new Wall[] { wc },
            RightWall = new Wall[] { wc },
            BottomWall = new Wall[] { wd },
            LeftWall = new Wall[] { wd }
        });
        t = tilemap[tilemap.Count - 1];
        tilemap.Add(new Tile1x1(1, 2)
        {
            TopWall = new Wall[] { wc },
            RightWall = new Wall[] { wd },
            BottomWall = new Wall[] { wd },
            LeftWall = new Wall[] { wc }
        });
        t = tilemap[tilemap.Count - 1];
        tilemap.Add(new Tile1x1(2, 2)
        {
            TopWall = new Wall[] { wd },
            RightWall = new Wall[] { wd },
            BottomWall = new Wall[] { wd },
            LeftWall = new Wall[] { wd }
        });
        t = tilemap[tilemap.Count - 1];
        tilemap.Add(new Tile2x2(3, 2)
        {
            TopWall = new Wall[] { wc, wd },
            RightWall = new Wall[] { wc, wc },
            BottomWall = new Wall[] { wd, wd },
            LeftWall = new Wall[] { wd, wd }
        });
        t = tilemap[tilemap.Count - 1];
        tilemap.Add(new Tile1x1(1, 3)
        {
            TopWall = new Wall[] { wd },
            RightWall = new Wall[] { wd },
            BottomWall = new Wall[] { wc },
            LeftWall = new Wall[] { wc }
        });
        t = tilemap[tilemap.Count - 1];
        tilemap.Add(new Tile2x1(2, 3, 90)
        {
            TopWall = new Wall[] { wd },
            RightWall = new Wall[] { wd, wc },
            BottomWall = new Wall[] { wc },
            LeftWall = new Wall[] { wd, wd }
        });
        t = tilemap[tilemap.Count - 1];
        tilemap.Add(new Tile1x1(1, 4)
        {
            TopWall = new Wall[] { wc },
            RightWall = new Wall[] { wd },
            BottomWall = new Wall[] { wc },
            LeftWall = new Wall[] { wc }
        });
        t = tilemap[tilemap.Count - 1];
        tilemap.Add(new Tile2x1(3, 4)
        {
            TopWall = new Wall[] { wd, wd },
            RightWall = new Wall[] { wc },
            BottomWall = new Wall[] { wc, wc },
            LeftWall = new Wall[] { wc }
        });
        t = tilemap[tilemap.Count - 1];
        Debug.LogFormat( "Tile Created \n" + 
            			 "Tile GridPos: {0},{1} \n" +
                         "Tile Rotation: {2} \n" +
                         "Tile Size: {3},{4} \n", t.XPos, t.YPos, t.Rotation, t.Width, t.Height);


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
