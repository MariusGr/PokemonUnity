using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVector
{
    int x;
    int y;

    public GridVector(int x, int y)
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

    public GridVector(Vector3 currentPosition, GridVector beforePosition)
        : this(currentPosition,
               beforePosition.x < currentPosition.x,
               beforePosition.y < currentPosition.z) { }

    public override bool Equals(object obj)
    {
        GridVector other = (GridVector)obj;
        return x == other.x && y == other.y;
    }

    public override string ToString() => $"GridVector({x}, {y})";

    public static GridVector operator +(GridVector a, GridVector b)
        => new GridVector(a.x + b.x, a.y + b.y);

    public static GridVector operator -(GridVector a, GridVector b)
        => new GridVector(a.x - b.x, a.y - b.y);

    public static implicit operator Vector3(GridVector v)
        => new Vector3(v.x, 0, v.y);
}
