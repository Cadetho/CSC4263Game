using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile
{
    private int[] pos = new int[2];
    /// <summary>
    /// The tilemap coordinates of this tile.
    /// </summary>
    public int[] Pos
    {
        get
        {
            return new int[] { XPos, YPos };
        }
        set
        {
            if (value.Length == 2)
            {
                XPos = value[0];
                YPos = value[1];
            }
            else
                throw new ArgumentException("Position must be an array of length 2", "Pos");
        }
    }
    /// <summary>
    /// The tilemap x coordinate of this tile.
    /// </summary>
    public int XPos
    {
        get
        {
            return pos[0];
        }
        set
        {
            if (value >= 0)
                pos[0] = value;
            else
                throw new ArgumentOutOfRangeException("XPos");
        }
    }
    /// <summary>
    /// The tilemap y coordinate of this tile.
    /// </summary>
    public int YPos
    {
        get
        {
            return pos[1];
        }
        set
        {
            if (value >= 0)
                pos[1] = value;
            else
                throw new ArgumentOutOfRangeException("YPos");
        }
    }
    private int rotation;
    /// <summary>
    /// Returns the width of this tile (Read Only).
    /// </summary>
    public int Width
    {
        get
        {
            return TopWall.Length;
        }
    }
    /// <summary>
    /// Returns the height of this tile (Read Only).
    /// </summary>
    public int Height
    {
        get
        {
            return RightWall.Length;
        }
    }
    /// <summary>
    /// The rotation of this tile in degrees. Values outside the range [0,360) will be converted to fit in that range.
    /// </summary>
    public int Rotation
    {
        get
        {
            return rotation;
        }

        set
        {
            rotation = ((value % 360) + 360) % 360;
        }
    }

    protected Wall[][] walls = new Wall[4][] { new Wall[1], new Wall[1], new Wall[1], new Wall[1] }; // When rotation is 0: { top, right, bottom, left }
    /// <summary>
    /// A <see cref="Wall"/>[] whose length is that of the top side of the tile.
    /// </summary>
    public Wall[] TopWall
    {
        get
        {
            return walls[(4 - Rotation / 90) % 4];
        }
        set
        {
            int i = (4 - Rotation / 90) % 4;
            if (value.Length == walls[i].Length)
                walls[i] = value;
            else
                throw new System.ArgumentException("Wall array length isn't correct.", "TopWall");
        }
    }
    /// <summary>
    /// A <see cref="Wall"/>[] whose length is that of the right side of the tile.
    /// </summary>
    public Wall[] RightWall
    {
        get
        {
            return walls[(5 - Rotation / 90) % 4];
        }
        set
        {
            int i = (5 - Rotation / 90) % 4;
            if (value.Length == walls[i].Length)
                walls[i] = value;
            else
                throw new System.ArgumentException("Wall array length isn't correct.", "TopWall");
        }
    }
    /// <summary>
    /// A <see cref="Wall"/>[] whose length is that of the bottom side of the tile.
    /// </summary>
    public Wall[] BottomWall
    {
        get
        {
            return walls[(6 - Rotation / 90) % 4];
        }
        set
        {
            int i = (6 - Rotation / 90) % 4;
            if (value.Length == walls[i].Length)
                walls[i] = value;
            else
                throw new System.ArgumentException("Wall array length isn't correct.", "TopWall");
        }
    }
    /// <summary>
    /// A <see cref="Wall"/>[] whose length is that of the left side of the tile.
    /// </summary>
    public Wall[] LeftWall
    {
        get
        {
            return walls[(7 - Rotation / 90) % 4];
        }
        set
        {
            int i = (7 - Rotation / 90) % 4;
            if (value.Length == walls[i].Length)
                walls[i] = value;
            else
                throw new System.ArgumentException("Wall array length isn't correct.", "TopWall");
        }
    }


    private int level;
    /// <summary>
    /// Enemy difficulty level. Must be > 0.
    /// </summary>
    public int Level {
        get
        {
            return level;
        }
        set
        {
            if (value < 0)
                throw new System.ArgumentOutOfRangeException("Level");
        }
    }
    /// <summary>
    /// A List of TrapModifiers applied to this tile.
    /// </summary>
    public List<TrapModifier> traps;
    /// <summary>
    /// A List of EnemyModifiers applied to the enemies spawned in this tile.
    /// </summary>
    public List<EnemyModifier> enemymodifiers;


    /// <summary>
    /// Sets this Tiles's coordinates and rotation.
    /// </summary>
    /// <param name="x">Tilemap x coordinate</param>
    /// <param name="y">Tilemap y coordinate</param>
    /// <param name="rotation">Rotation in degrees</param>
    public void Place(int x, int y, int rotation)
    {
        XPos = x;
        YPos = y;
        Rotation = rotation;
    }

    private static float doorprobability;
    /// <summary>
    /// Fractional probability of each wall having a door (e.g., 0.5 = 50%).
    /// </summary>
    public static float DoorProbability { get; set; }
    /// <summary>
    /// For each wall of this tile, randomly determine if a door is to be placed there based on the static Tile.DoorProbability property.
    /// </summary>
    public void RandomizeWalls()
    {
        RandomizeWalls(DoorProbability);
    }
    /// <summary>
    /// For each wall of this tile, randomly determine if a door is to be placed there based on the given probability.
    /// </summary>
    /// <param name="doorprob">Fractional probability of each wall having a door (e.g., 0.5 = 50%)</param>
    public void RandomizeWalls(float doorprob)
    {
        bool hasdoor = false;
        foreach (Wall[] side in walls)
        {
            for (int i = 0; i < side.Length; i++)
            {
                bool isdoor = UnityEngine.Random.value <= doorprob;
                side[i] = isdoor ? Wall.Door : Wall.Closed;
                hasdoor = hasdoor || isdoor;
            }
        }
        if (!hasdoor)
        {
            Wall[] side = walls[UnityEngine.Random.Range(0,4)];
            side[UnityEngine.Random.Range(0,side.Length)] = Wall.Door;
        }
    }

    public void AddTrap(TrapModifier trap)
    {
        traps.Add(trap);
    }
    public void AddEnemyModifier(EnemyModifier em)
    {
        enemymodifiers.Add(em);
    }
    
    /// <summary>
    /// Returns an array of tilemap coordinates {x,y} occupied by this Tile. 
    /// </summary>
    /// <returns>Array of Int[] containing {x, y}</returns>
    public abstract int[,] GetRelativeSpacesOccupied();

    /// <summary>
    /// Returns a random tile.
    /// </summary>
    /// <param name="prob1x1">Probability of returning a 1x1 Tile</param>
    /// <param name="prob2x1">Probability of returning a 2x1 Tile</param>
    /// <param name="probCorner">Probability of returning a Corner Tile</param>
    /// <param name="prob2x2">Probability of returning a 2x2 Tile</param>
    /// <returns>A Tile object</returns>
    public static Tile RandomTile(int prob1x1, int prob2x1, int probCorner, int prob2x2)
    {
        float rand = UnityEngine.Random.Range(0, prob1x1 + prob2x1 + probCorner + prob2x2);
        if (rand < prob1x1)
        {
            return new Tile1x1();
        }
        if (rand < prob1x1+prob2x1)
        {
            return new Tile2x1();
        }
        if (rand < prob1x1+prob2x1+probCorner)
        {
            return new TileCorner();
        }
        else
        {
            return new Tile2x2();
        }
    }
}

/// <summary>
/// Wall types: Closed (solid wall), Door, and Open (no wall).
/// </summary>
public enum Wall
{
    Closed,
    Door,
    Open
};
