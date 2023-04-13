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

    private GridVector _normalized = null;
    public GridVector normalized
    {
        get
        {
            if (_normalized is null)
            {
                int xNorm = x == 0 ? 0 : Math.Sign(x);
                int yNorm = y == 0 ? 0 : Math.Sign(y);
                _normalized = new GridVector(xNorm, yNorm);
            }
            return _normalized;
        }
    }

    public float magnitude => new Vector2(x, y).magnitude;

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

    static public GridVector GetLookAt(Vector3 position, Vector3 target) => GetLookAt(new GridVector(position), new GridVector(target));
    static public GridVector GetLookAt(GridVector position, GridVector target) => (target - position).normalized;

    public GridVector(int x = 0, int y = 0)
    {
        this.x = x;
        this.y = y;
    }

    private int RoundToInt(float f)
    {
        int result = Mathf.RoundToInt(f);
        if (Mathf.Abs(result - f) - .5f < .01f)
            return Mathf.CeilToInt(f);
        return result;
    }

    public GridVector(Vector3 vector)
    {
        this.x = RoundToInt(vector.x);
        this.y = RoundToInt(vector.z);
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
     * Creates a GridVector from a Vector3. Any dimension (x or/and y) of the resulting Vector will thus only be different
     * than the respective dimension of currentPosition, if it is at least different by 1.0 to that dimension.
     * This constructor is used to check whether a position has changed so that we can safely assume the actor
     * has moved a full 1.0 units away from beforePosition on at least one of the axis.
     * **/
    public GridVector(Vector3 currentPosition, Vector3 beforePosition) : this(currentPosition, new GridVector(beforePosition)) { }
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

    public static GridVector operator *(int d, GridVector v)
        => new GridVector(d * v.x, d * v.y);

    public static GridVector operator *(float d, GridVector v)
        => new GridVector((int)d * v.x, (int)d * v.y);

    public static GridVector operator -(GridVector v)
        => new GridVector(-v.x, -v.y);

    public static implicit operator Vector3(GridVector v)
        => new Vector3(v.x, 0, v.y);
}
