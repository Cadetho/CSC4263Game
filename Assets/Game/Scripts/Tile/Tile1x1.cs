using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile1x1 : Tile
{
    private readonly int[,] occupied = new int[,] { { 0, 0 } };

    public Tile1x1(int x = 0, int y = 0, int rot = 0)
    {
        Place(x, y, rot);
        RandomizeWalls();
    }

    /// <summary>
    /// Returns an array of tilemap coordinates {x,y} occupied by this Tile. 
    /// </summary>
    /// <returns>Array of Int[] containing {x, y}</returns>
    public override int[,] GetRelativeSpacesOccupied()
    {
        return occupied;
    }
}