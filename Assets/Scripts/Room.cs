using System;

public class Room
{
    public int X { get; }
    public int Y { get; }

    public Room(int n)
    {
        X = UnityEngine.Random.Range(0, n);
        Y = UnityEngine.Random.Range(0, n);
    }

    public Room(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public bool IsNeighboring(Room other)
    {
        var dx = Math.Abs(X - other.X);
        var dy = Math.Abs(Y - other.Y);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    public bool IsNear(Room other)
    {
        var dx = Math.Abs(X - other.X);
        var dy = Math.Abs(Y - other.Y);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1) || (dx == 1 && dy == 1);
    }

    public (int, int) GetPosition()
    {
        return (X, Y);
    }

    public override bool Equals(object obj)
    {
        if (obj is not Room room) return false;
        return X == room.X && Y == room.Y;
    }

    protected bool Equals(Room other)
    {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}