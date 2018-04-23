using System;
using System.Collections.Generic;
using UnityEngine;
using Coords = UnityEngine.Vector2Int;
using Wall = boardTile.Wall;

namespace Game.PlayPhase
{
    public partial class PlayEnvironment
    {
        #region Fields

        private PlayPhaseManager pm;
        private GameManager gm;
        private EnvironmentPrefabs envPrefabs;
        private Transform envContainer;
        private Transform tileContainer;
        private Transform buildingContainer;
        private Transform borderContainer;
        private Dictionary<Coords, Building> buildings = new Dictionary<Coords, Building>();
        private Dictionary<Coords, Tile> tiles = new Dictionary<Coords, Tile>();
        private Dictionary<Coords, Empty> emptySpaces = new Dictionary<Coords, Empty>();

        private class Empty
        {
            public GameObject go;

            public Empty(GameObject go)
            {
                this.go = go;
            }

            public bool Destroy()
            {
                if (go == null) return false;
                GameObject.Destroy(go);
                go = null;
                return true;
            }
        }

        #endregion

        public PlayEnvironment(PlayPhaseManager pm, Transform parent)
        {
            this.pm = pm;
            gm = pm.gameManager;
            envPrefabs = pm.envPrefabs;

            envContainer = new GameObject("Environment").transform;
            tileContainer = new GameObject("Tiles").transform;
            buildingContainer = new GameObject("Buildings").transform;
            borderContainer = new GameObject("Borders").transform;
            envContainer.parent = parent;
            tileContainer.parent = envContainer;
            buildingContainer.parent = envContainer;
            borderContainer.parent = envContainer;
        }

        /// <summary>
        /// Places the environment GameObjects in the level.
        /// </summary>
        public void Generate()
        {
            // Replace size with number of tiles placed each round (account for large tiles).
            List<Tile> newTiles = new List<Tile>();

            // Update tile data
            String errmsg = "Generating Play Environment\nTilemap:\n";
            foreach (var pt in gm.masterBoard.board)
            {
                Coords tileCoords = new Coords(pt.boardX, -pt.boardY);
                Tile tile = GetTile(tileCoords);
                Debug.Log("EnvGen - Checked for tile @ " + tileCoords);
                if (tile == null)
                {
                    errmsg += "New ";
                    Debug.Log("EnvGen - No tile found @ " + tileCoords);
                    // placedTile is new.
                    tile = AddTile(pt);
                    newTiles.Add(tile);

                    SetAdjacentTiles(tile);
                }
                errmsg += "Placed Tile @ " + tileCoords + " -> " + tile.Coords +"\n";
            }

            UpdateBuildings(newTiles);
            Logger.Debug(errmsg);
            Debug.Log(errmsg);
            //Place GameObjects for empty spaces bordering placed tiles
            //foreach (var emptySpace in emptySpaces)
            //{
            //    Empty empty = emptySpace.Value;
            //    if (empty.go == null)
            //    {
            //        GameObject go = new GameObject("Empty (" + emptySpace.Key + ")");
            //        go.transform.parent = tileContainer;
            //        go.transform.position = Utility.TranslateGridPosition(emptySpace.Key);
            //        GameObject.Instantiate(envPrefabs.NullTilePrefab, go.transform);
            //        //emptySpaces[emptySpace.Key] = new Empty(go);
            //        empty.go = go;
            //        if (empty.go == null)
            //            Debug.Log("ERROR: Empty is null at " + emptySpace.Key);
            //    }
            //}
        }

        /// <summary>
        /// Returns the PlayTile at the given coordinates, or null if none exists.
        /// </summary>
        private Tile GetTile(Coords coords)
        {
            //return (tiles.ContainsKey(coords) ? tiles[coords] : null);
            Tile tile;
            tiles.TryGetValue(coords, out tile);
            return tile;
        }

        /// <summary>
        /// Creates a new PlayTile with the given PlacedTile.
        /// Do not use if a PlayTile already exists at the same location.
        /// </summary>
        private Tile AddTile(placedTile pt)
        {
            Tile tile = new Tile(pt, envPrefabs, tileContainer);
            Debug.Log("EnvGen - Adding new tile @ " + tile.Coords);
            tiles.Add(tile.Coords, tile);
            RemoveEmpty(tile.Coords);
            return tile;
        }

        private void SetAdjacentTiles(Tile tile)
        {
            Coords upOne = tile.Coords + Coords.up;
            Coords rightOne = tile.Coords + Coords.right;
            Coords downOne = tile.Coords + Coords.down;
            Coords leftOne = tile.Coords + Coords.left;
            Tile northAdjTile = GetTile(upOne);
            Tile eastAdjTile = GetTile(rightOne);
            Tile southAdjTile = GetTile(downOne);
            Tile westAdjTile = GetTile(leftOne);
            Debug.Log("Checking adjacent tiles around " + tile.Coords);
            if (northAdjTile != null)
            {
                //tile.NorthAdjTile = northAdjTile;
                //northAdjTile.SouthAdjTile = tile;

                // Set north border to the existing south border of northAdjTile.
                tile.NorthBorder = northAdjTile.SouthBorder;
                tile.NorthBorder.edge = false;
                tile.NorthBorder.Wall = tile.NorthWall;
            }
            else
            {
                AddEmpty(upOne);

                // Create a new border.
                Transform borderParent = new GameObject("Border North of Tile " + tile.Coords).transform;
                borderParent.parent = borderContainer;
                borderParent.localPosition = Utility.TranslateGridPosition(tile.Coords + (Vector2)Coords.up * 0.5f);
                tile.NorthBorder = new Border(tile.NorthWall, borderParent);
            }

            if (eastAdjTile != null)
            {
                //tile.EastAdjTile = eastAdjTile;
                //eastAdjTile.WestAdjTile = tile;

                // Set east border to the existing west border of eastAdjTile.
                tile.EastBorder = eastAdjTile.WestBorder;
                tile.EastBorder.edge = false;
                tile.EastBorder.Wall = tile.EastWall;
            }
            else
            {
                AddEmpty(rightOne);

                // Create a new border.
                Transform borderParent = new GameObject("Border East of Tile " + tile.Coords).transform;
                borderParent.parent = borderContainer;
                borderParent.localPosition = Utility.TranslateGridPosition(tile.Coords + (Vector2)Coords.right * 0.5f);
                tile.EastBorder = new Border(tile.EastWall, borderParent);
            }

            if (southAdjTile != null)
            {
                //tile.SouthAdjTile = southAdjTile;
                //southAdjTile.NorthAdjTile = tile;

                // Set south border to the existing north border of southAdjTile.
                tile.SouthBorder = southAdjTile.NorthBorder;
                tile.SouthBorder.edge = false;
                tile.SouthBorder.Wall = tile.SouthWall;
            }
            else
            {
                AddEmpty(downOne);

                // Create a new border.
                Transform borderParent = new GameObject("Border South of Tile " + tile.Coords).transform;
                borderParent.parent = borderContainer;
                borderParent.localPosition = Utility.TranslateGridPosition(tile.Coords + (Vector2)Coords.down * 0.5f);
                tile.SouthBorder = new Border(tile.SouthWall, borderParent);
            }

            if (westAdjTile != null)
            {
                //tile.WestAdjTile = westAdjTile;
                //westAdjTile.EastAdjTile = tile;

                // Set west border to the existing east border of westAdjTile.
                tile.WestBorder = westAdjTile.EastBorder;
                tile.WestBorder.edge = false;
                tile.WestBorder.Wall = tile.WestWall;
            }
            else
            {
                AddEmpty(leftOne);

                // Create a new border.
                Transform borderParent = new GameObject("Border West of Tile " + tile.Coords).transform;
                borderParent.parent = borderContainer;
                borderParent.localPosition = Utility.TranslateGridPosition(tile.Coords + (Vector2)Coords.left * 0.5f);
                tile.WestBorder = new Border(tile.NorthWall, borderParent);
            }

            BorderGenerate(tile.NorthBorder, envPrefabs.NSAlleyFloorPrefab, envPrefabs.NSOpenFloorPrefab);
            BorderGenerate(tile.EastBorder, envPrefabs.EWAlleyFloorPrefab, envPrefabs.EWOpenFloorPrefab);
            BorderGenerate(tile.SouthBorder, envPrefabs.NSAlleyFloorPrefab, envPrefabs.NSOpenFloorPrefab);
            BorderGenerate(tile.WestBorder, envPrefabs.EWAlleyFloorPrefab, envPrefabs.EWOpenFloorPrefab);
        }

        private void BorderGenerate(Border border, GameObject alleyFloorPrefab, GameObject openFloorPrefab)
        {
            if (border.Floor == null && border.Wall != Wall.fullWall)
            {
                GameObject prefab = null;
                switch (border.Wall)
                {
                    case Wall.door:
                        prefab = alleyFloorPrefab;
                        break;
                    case Wall.noWall:
                        prefab = openFloorPrefab;
                        break;
                }
                //Transform bt = GameObject.Instantiate(prefab, border.Parent).transform;
                //bt.localPosition = Vector3.zero;
                border.Floor = GameObject.Instantiate(prefab, border.Parent);
                // create a barrier
                if (border.edge == true && border.Barrier == null)
                {
                    border.Barrier = GameObject.Instantiate(envPrefabs.NullTilePrefab, border.Parent);
                }
            }
        }

        private void AddEmpty(Coords coords)
        {
            //GameObject empty = new GameObject("Empty (" + coords + ")");
            //empty.transform.parent = tileContainer;
            //empty.transform.position = Utility.TranslateGridPosition(coords);
            ////GameObject.Instantiate(envPrefabs.NullTilePrefab, empty.transform);
            //emptySpaces[coords] = new Empty(GameObject.Instantiate(envPrefabs.NullTilePrefab, empty.transform));
            //Debug.Log("Empty set at " + coords);
        }

        private void RemoveEmpty(Coords coords)
        {
            Debug.Log("Remove empty @ " + coords);
            if (! emptySpaces.ContainsKey(coords) )
            {
                Debug.Log("No empty to remove @ " + coords);
                return;
            }
            bool destroyed = emptySpaces[coords].Destroy();
            if (destroyed == false) Debug.Log("ERROR: Empty was null @ " + coords);
            //Debug.Log("Removing empty @ " + coords);
            //GameObject empty;
            //if (emptySpaces.TryGetValue(coords, out empty))
            //{
            //    if (empty != null)
            //    {
            //        empty.SetActive(false);
            //        GameObject.Destroy(empty);
            //        //emptySpaces.Remove(coords);
            //        Debug.Log("Removed empty @ " + coords);
            //    }
            //    else
            //        Debug.Log("ERROR: Empty at " + coords + " was null.");
            //}
            //else
            //    Debug.Log("No empty found @ " + coords);
        }

        /// <summary>
        /// Links the given Tile with the buildings surrounding it.
        /// </summary>
        private void UpdateBuildings(List<Tile> newTiles)
        {
            var updatedBuildings = new List<Building>();
            foreach (var tile in newTiles)
            {
                // Get or create surrounding buildings
                Coords buildingCoords = tile.Coords;
                Building NWBuilding = GetBuilding(buildingCoords) ?? CreateBuilding(buildingCoords);
                buildingCoords = tile.Coords + new Coords(1, 0);
                Building NEBuilding = GetBuilding(buildingCoords) ?? CreateBuilding(buildingCoords);
                buildingCoords = tile.Coords + new Coords(1, -1);
                Building SEBuilding = GetBuilding(buildingCoords) ?? CreateBuilding(buildingCoords);
                buildingCoords = tile.Coords + new Coords(0, -1);
                Building SWBuilding = GetBuilding(buildingCoords) ?? CreateBuilding(buildingCoords);

                // OLD CODE: Create buildings where there are none (handled by code above) 
                //if (NWBuilding == null)
                //{
                //    // a building doesn't already exist there
                //    NWBuilding = CreateBuilding(buildingCoords);
                //}
                //if (NEBuilding == null)
                //{
                //    // a building doesn't already exist there
                //    NEBuilding = CreateBuilding(buildingCoords);
                //}
                //if (SEBuilding == null)
                //{
                //    // a building doesn't already exist there
                //    SEBuilding = CreateBuilding(buildingCoords);
                //}
                //if (SWBuilding == null)
                //{
                //    // a building doesn't already exist there
                //    SWBuilding = CreateBuilding(buildingCoords);
                //}

                // Set the walls for each building
                // Southwest corner
                SWBuilding.NorthWall = tile.WestWall;
                SWBuilding.EastWall = tile.SouthWall;
                // Southesst corner
                SEBuilding.NorthWall = tile.EastWall;
                SEBuilding.WestWall = tile.SouthWall;
                // Northeast corner
                NEBuilding.SouthWall = tile.EastWall;
                NEBuilding.WestWall = tile.NorthWall;
                // Northwest corner
                NWBuilding.SouthWall = tile.WestWall;
                NWBuilding.EastWall = tile.NorthWall;

                updatedBuildings.Add(NWBuilding);
                updatedBuildings.Add(NEBuilding);
                updatedBuildings.Add(SEBuilding);
                updatedBuildings.Add(SWBuilding);
            }

            foreach (var building in updatedBuildings)
            {
                // If building is an island (this corner is in the middle of 4 open tiles,
                // destroy the building and replace it with a flat floor prefab
                if (building.NorthWall == Wall.noWall &&
                    building.EastWall == Wall.noWall &&
                    building.SouthWall == Wall.noWall &&
                    building.WestWall == Wall.noWall)
                {
                    building.Flatten(envPrefabs.OpenCornerPrefab);
                    continue;
                }
                // Otherwise, update the parts of the building that need to be updated.
                if (building.buildingCenter == null)
                {
                    building.buildingCenter = GameObject.Instantiate(building.Prefabs.CenterRoofPrefab, building.Parent);
                    building.buildingCenter.transform.localPosition = Vector3.zero;
                }
                if (building.northWing == null)
                {
                    Quaternion rotation = Quaternion.LookRotation(Vector3.forward);
                    GameObject wing = null;
                    switch (building.NorthWall)
                    {
                        case Wall.fullWall:
                            wing = GameObject.Instantiate(building.Prefabs.ClosedWallPrefab, building.Parent);
                            break;
                        case Wall.door:
                            wing = GameObject.Instantiate(building.Prefabs.PathWallPrefab, building.Parent);
                            break;
                        case Wall.noWall:
                            wing = GameObject.Instantiate(building.Prefabs.OpenWallPrefab, building.Parent);
                            break;
                    }
                    wing.transform.localPosition = Vector3.zero;
                    wing.transform.rotation = rotation;
                    building.northWing = wing;
                }
                if (building.eastWing == null)
                {
                    Quaternion rotation = Quaternion.LookRotation(Vector3.right);
                    GameObject wing = null;
                    switch (building.EastWall)
                    {
                        case Wall.fullWall:
                            wing = GameObject.Instantiate(building.Prefabs.ClosedWallPrefab, building.Parent);
                            break;
                        case Wall.door:
                            wing = GameObject.Instantiate(building.Prefabs.PathWallPrefab, building.Parent);
                            break;
                        case Wall.noWall:
                            wing = GameObject.Instantiate(building.Prefabs.OpenWallPrefab, building.Parent);
                            break;
                    }
                    wing.transform.localPosition = Vector3.zero;
                    wing.transform.rotation = rotation;
                    building.eastWing = wing;
                }
                if (building.southWing == null)
                {
                    Quaternion rotation = Quaternion.LookRotation(Vector3.back);
                    GameObject wing = null;
                    switch (building.SouthWall)
                    {
                        case Wall.fullWall:
                            wing = GameObject.Instantiate(building.Prefabs.ClosedWallPrefab, building.Parent);
                            break;
                        case Wall.door:
                            wing = GameObject.Instantiate(building.Prefabs.PathWallPrefab, building.Parent);
                            break;
                        case Wall.noWall:
                            wing = GameObject.Instantiate(building.Prefabs.OpenWallPrefab, building.Parent);
                            break;
                    }
                    wing.transform.localPosition = Vector3.zero;
                    wing.transform.rotation = rotation;
                    building.southWing = wing;
                }
                if (building.westWing == null)
                {
                    Quaternion rotation = Quaternion.LookRotation(Vector3.left);
                    GameObject wing = null;
                    switch (building.WestWall)
                    {
                        case Wall.fullWall:
                            wing = GameObject.Instantiate(building.Prefabs.ClosedWallPrefab, building.Parent);
                            break;
                        case Wall.door:
                            wing = GameObject.Instantiate(building.Prefabs.PathWallPrefab, building.Parent);
                            break;
                        case Wall.noWall:
                            wing = GameObject.Instantiate(building.Prefabs.OpenWallPrefab, building.Parent);
                            break;
                    }
                    wing.transform.localPosition = Vector3.zero;
                    wing.transform.rotation = rotation;
                    building.westWing = wing;
                }
            }
        }

        /// <summary>
        /// Returns the Building at the given coordinates, or null if none exists.
        /// </summary>
        private Building GetBuilding(Coords coords)
        {
            //return (buildings.ContainsKey(coords) ? buildings[coords] : null);
            Building building;
            buildings.TryGetValue(coords, out building);
            return building;
        }

        /// <summary>
        /// Creates a new Building at the given coordinated.
        /// Do not use if a Building already exists at that location.
        /// </summary>
        private Building CreateBuilding(Coords coords)
        {
            Building b = new Building(coords, envPrefabs.BuildingPrefabs, buildingContainer);
            buildings.Add(coords, b);
            return b;
        }

        private static Wall Max(Wall a, Wall b)
        {
            return (a == Wall.fullWall || b == Wall.fullWall ?
                    Wall.fullWall :
                    (a == Wall.door || b == Wall.door ?
                     Wall.door :
                     Wall.noWall));
        }
    }
}