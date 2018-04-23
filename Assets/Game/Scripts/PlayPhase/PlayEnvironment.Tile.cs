using System;
using UnityEngine;
using Coords = UnityEngine.Vector2Int;
using Wall = boardTile.Wall;

namespace Game.PlayPhase
{
    public partial class PlayEnvironment
    {
        private class Tile
        {
            private readonly placedTile tile;
            private EnvironmentPrefabs prefabs;
            private Border northBorder;
            private Border eastBorder;
            private Border southBorder;
            private Border westBorder;

            /// <summary>
            /// Creates a new Tile based on the given placedTile.
            /// </summary>
            public Tile(placedTile pt, EnvironmentPrefabs prefabs, Transform container = null)
            {
                this.tile = pt;
                this.prefabs = prefabs;
                Transform tileParent = new GameObject("Tile " + Coords).transform;
                tileParent.parent = container;
                tileParent.localPosition = Utility.TranslateGridPosition(Coords);
                GameObject go = GameObject.Instantiate(prefabs.CenterFloorPrefab, tileParent);
                go.name = "Tile Floor";
                go.transform.localPosition = Vector3.zero;
            }

            /// <summary>
            /// The board grid position of this Tile.
            /// </summary>
            public Coords Coords
            {
                get
                {
                    return new Coords(tile.boardX, -tile.boardY);
                }
            }

            /// <summary>
            /// Returns the northern Wall of this tile.
            /// </summary>
            public Wall NorthWall
            {
                get
                {
                    return tile.square.getTop();
                }
            }

            /// <summary>
            /// Returns the eastern Wall of this tile.
            /// </summary>
            public Wall EastWall
            {
                get
                {
                    return tile.square.getRight();
                }
            }

            /// <summary>
            /// Returns the southern Wall of this tile.
            /// </summary>
            public Wall SouthWall
            {
                get
                {
                    return tile.square.getBottom();
                }
            }

            /// <summary>
            /// Returns the western Wall of this tile.
            /// </summary>
            public Wall WestWall
            {
                get
                {
                    return tile.square.getLeft();
                }
            }

            // Old Code: Adjacent Tiles

            //private Tile northAdjTile;
            //private Tile eastAdjTile;
            //private Tile southAdjTile;
            //private Tile westAdjTile;

            ///// <summary>
            ///// The Tile just north of this Tile on the board grid.
            ///// </summary>
            //public Tile NorthAdjTile { get; set; }

            ///// <summary>
            ///// The Tile just east of this Tile on the board grid.
            ///// </summary>
            //public Tile EastAdjTile { get; set; }

            ///// <summary>
            ///// The Tile just south of this Tile on the board grid.
            ///// </summary>
            //public Tile SouthAdjTile { get; set; }

            ///// <summary>
            ///// The Tile just west of this Tile on the board grid.
            ///// </summary>
            //public Tile WestAdjTile { get; set; }

            public Border NorthBorder
            {
                get
                {
                    return northBorder;
                }

                set
                {
                    northBorder = value;
                }
            }

            public Border EastBorder
            {
                get
                {
                    return eastBorder;
                }

                set
                {
                    eastBorder = value;
                }
            }

            public Border SouthBorder
            {
                get
                {
                    return southBorder;
                }

                set
                {
                    southBorder = value;
                }
            }

            public Border WestBorder
            {
                get
                {
                    return westBorder;
                }

                set
                {
                    westBorder = value;
                }
            }
        }

        private class Border
        {
            private Wall wall;
            private Transform parent;
            private GameObject floor;

            public Border(Wall wall, Transform parent)
            {
                this.wall = wall;
                this.Parent = parent;
            }

            public Wall Wall
            {
                get
                {
                    return wall;
                }

                set
                {
                    Wall updated = Max(value, wall);
                    if (updated != wall && floor != null)
                    {
                        GameObject.Destroy(floor);
                        floor = null;
                    }
                    wall = updated;
                }
            }

            public GameObject Floor
            {
                get
                {
                    return floor;
                }

                set
                {
                    floor = value;
                    floor.transform.parent = parent;
                }
            }

            public Transform Parent
            {
                get
                {
                    return parent;
                }

                set
                {
                    parent = value;
                }
            }
        }
    }
}