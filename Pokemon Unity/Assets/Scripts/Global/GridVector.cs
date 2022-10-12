using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridVector
{
    public int x;
    public int y;

    public static GridVector Zero => new GridVector();
    public static GridVector Down => new GridVector(0, -1);

    static Dictionary<GridVector, Direction> gridVectorToDirectionMap = new Dictionary<GridVector, Direction>()
    {
        { new GridVector(0, 0), Direction.None },
        { new GridVector(1, 0), Direction.Right },
        { new GridVector(-1, 0), Direction.Left },
        { new GridVector(0, 1), Direction.Up },
        { new GridVector(0, -1), Direction.Down },
    };

    static Dictionary<Direction, GridVector> directionToGridVectorMap = new Dictionary<Direction, GridVector>()
    {
        { Direction.None, new GridVector(0, 0) },
        { Direction.Right, new GridVector(1, 0) },
        { Direction.Left, new GridVector(-1, 0) },
        { Direction.Up, new GridVector(0, 1) },
        { Direction.Down, new GridVector(0, -1) },
    };

    public GridVector(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }

    public GridVector(Vector3 vector)
    {
        this.x = Mathf.RoundToInt(vector.x);
        this.y = Mathf.RoundToInt(vector.z);
    }

    public GridVector(Vector3 vector, bool floorX, bool floorY)
    {
        if (floorX)
            this.x = Mathf.FloorToInt(vector.x);
        else
            this.x = Mathf.CeilToInt(vector.x);
        if (floorY)
            this.y = Mathf.FloorToInt(vector.z);
        else
            this.y = Mathf.CeilToInt(vector.z);
    }

    /**
     * Creates a GridVector from a Vector3. Any dimension (x or/and y) will be set to 
    public GridVector(Vector3 currentPosition, GridVector beforePosition)
        : this(currentPosition,
               beforePosition.x < currentPosition.x,
               beforePosition.y < currentPosition.z) { }

    public GridVector(Direction direction) : this(directionToGridVectorMap[direction]) { }

    public Direction ToDirection() => gridVectorToDirectionMap[this];

    public override bool Equals(object obj)
    {
        GridVector other = (GridVector)obj;
        return x == other.x && y == other.y;
    }

    // This is very lazy but whatevrrrr
    public override int GetHashCode()
    {
        return x + y * 167;
    }

    public override string ToString() => $"GridVector({x}, {y})";

    public static GridVector operator +(GridVector a, GridVector b)
        => new GridVector(a.x + b.x, a.y + b.y);

    public static GridVector operator -(GridVector a, GridVector b)
        => new GridVector(a.x - b.x, a.y - b.y);

    public static implicit operator Vector3(GridVector v)
        => new Vector3(v.x, 0, v.y);
}
