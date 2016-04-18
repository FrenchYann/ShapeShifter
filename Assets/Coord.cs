using System;
using System.Collections.Generic;

public struct Coord
{
    public static Coord Zero  = new Coord( 0, 0);
    public static Coord Right = new Coord( 1, 0);
    public static Coord Down  = new Coord( 0, 1);
    public static Coord Left  = new Coord(-1, 0);
    public static Coord Up    = new Coord( 0,-1);
    private static readonly Coord[] CARDINAL_DIRECTIONS =   
    {
        Left,
        Down,
        Right,
        Up,
    };
    public int X;
    public int Y;
    public Coord(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public Coord Abs()
    {
        return new Coord(Math.Abs(this.X), Math.Abs(this.Y));
    }


    public Coord Normalize()
    {
        Coord norm = this.Abs();
        Coord res = Coord.Zero;
        if (norm.X <= norm.Y)
        {
            res.Y = Math.Sign(this.Y);
        }
        if (norm.Y <= norm.X)
        {
            res.X = Math.Sign(this.X);
        }
        return res;
    }

    public override bool Equals(object obj)
    {
        if (obj is Coord)
        {
            Coord other = (Coord)obj;
            return other.X == this.X && other.Y == this.Y;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return this.X + this.Y * 997;
    }

    public override string ToString()
    {
        return string.Format("({0},{1})", this.X, this.Y);
    }

    public static Coord operator +(Coord c1, Coord c2)
    {
        return new Coord(c1.X + c2.X, c1.Y + c2.Y);
    }
    public static Coord operator -(Coord c)
    {
        return new Coord(-c.X, -c.Y);
    }
    public static Coord operator -(Coord c1, Coord c2)
    {
        return new Coord(c1.X - c2.X, c1.Y - c2.Y);
    }
    public static Coord operator *(Coord c, int scalar)
    {
        return new Coord(c.X * scalar, c.Y * scalar);
    }
    public static Coord operator /(Coord c, int scalar)
    {
        return new Coord(c.X / scalar, c.Y / scalar);
    }

    public static bool operator ==(Coord c1, Coord c2)
    {
        return c1.Equals(c2);
    }
    public static bool operator !=(Coord c1, Coord c2)
    {
        return !c1.Equals(c2);
    }

    public static IEnumerable<Coord> ForeachCardinalDirection()
    {
        foreach (Coord coord in CARDINAL_DIRECTIONS)
        {
            yield return coord;
        }
    }
}
