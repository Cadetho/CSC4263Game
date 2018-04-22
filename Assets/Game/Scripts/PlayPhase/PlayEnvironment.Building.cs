using UnityEngine;
using Coords = UnityEngine.Vector2Int;
using Wall = boardTile.Wall;

namespace Game.PlayPhase
{
    public partial class PlayEnvironment
    {
        private class Building
        {
            /// <summary>
            /// The board grid position of this Building.
            /// (This Building is on the NW corner of the tile at the same coordinates)
            /// </summary>
            public Coords coords;

            private GameObject parent;
            private BuildingPrefabs prefabs;
            private Wall northWall = Wall.noWall;
            private Wall southWall = Wall.noWall;
            private Wall eastWall = Wall.noWall;
            private Wall westWall = Wall.noWall;
            public GameObject buildingCenter;
            public GameObject northWing;
            public GameObject southWing;
            public GameObject westWing;
            public GameObject eastWing;

            /// <summary>
            /// Creates a new Building at the given coords.
            /// </summary>
            public Building(Coords coords, BuildingPrefabs prefabs, Transform container = null)
            {
                this.coords = coords;
                this.prefabs = prefabs;

                parent = new GameObject("Building " + coords);
                Parent.parent = container;
                Parent.localPosition = Utility.TranslateGridPosition(coords + new Vector2(-0.5f, 0.5f));
            }

            public Wall NorthWall
            {
                get
                {
                    return northWall;
                }

                set
                {
                    UpdateWing(value, ref northWall, ref northWing);
                }
            }

            public Wall SouthWall
            {
                get
                {
                    return southWall;
                }

                set
                {
                    UpdateWing(value, ref southWall, ref southWing);
                }
            }

            public Wall EastWall
            {
                get
                {
                    return eastWall;
                }

                set
                {
                    UpdateWing(value, ref eastWall, ref eastWing);
                }
            }

            public Wall WestWall
            {
                get
                {
                    return westWall;
                }

                set
                {
                    UpdateWing(value, ref westWall, ref westWing);
                }
            }

            public BuildingPrefabs Prefabs
            {
                get
                {
                    return prefabs;
                }
            }

            public Transform Parent
            {
                get
                {
                    return parent.transform;
                }
            }

            // Old Code: Adjacent Tiles

            //private Tile tileNW;
            //private Tile tileNE;
            //private Tile tileSE;
            //private Tile tileSW;

            ///// <summary>
            ///// The Tile just northwest of this Building.
            ///// </summary>
            //public Tile TileNW { get; set; }

            ///// <summary>
            ///// The Tile just northeast of this Building.
            ///// </summary>
            //public Tile TileNE { get; set; }

            ///// <summary>
            ///// The Tile just southeast of this Building.
            ///// </summary>
            //public Tile TileSE { get; set; }

            ///// <summary>
            ///// The Tile just southwest of this Building.
            ///// </summary>
            //public Tile TileSW { get; set; }

            public void Flatten(GameObject prefab)
            {
                if (buildingCenter != null)
                {
                    GameObject.Destroy(buildingCenter);
                }
                if (northWing != null)
                {
                    GameObject.Destroy(northWing);
                }
                if (eastWing != null)
                {
                    GameObject.Destroy(eastWing);
                }
                if (southWing != null)
                {
                    GameObject.Destroy(southWing);
                }
                if (westWing != null)
                {
                    GameObject.Destroy(westWing);
                }
                northWing = null;
                eastWing = null;
                southWing = null;
                westWing = null;

                buildingCenter = GameObject.Instantiate(prefab, Parent.transform);
            }

            private void UpdateWing(Wall newValue, ref Wall currValue, ref GameObject wing)
            {
                Wall updated = Max(newValue, currValue);
                if (updated != currValue && wing != null)
                {
                    GameObject.Destroy(wing);
                    wing = null;
                }
                currValue = updated;
            }
        }
    }
}