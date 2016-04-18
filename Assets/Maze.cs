using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Maze {


    public static readonly Coord DefaultStart = new Coord(1, 1);
    public readonly Coord Start;
    public Coord End { get; set; }

    public struct Size
    {
        public readonly int Width;
        public readonly int Height;
        public Size(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
        public override bool Equals(object obj)
        {
            if (obj is Size)
            {
                Size other = (Size)obj;
                return other.Width == this.Width && other.Height == this.Height;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Width + this.Height * 997;
        }
        public static bool operator ==(Size s1, Size s2)
        {
            return s1.Equals(s2);
        }
        public static bool operator !=(Size s1, Size s2)
        {
            return !s1.Equals(s2);
        }
    }

    internal List<Coord> GetNeighbors(Coord end)
    {
        List<Coord> res = new List<Coord>();
        foreach(Coord dir in Coord.ForeachCardinalDirection())
        {
            Coord coord = end + dir;
            if (this.IsValid(coord))
            {
                res.Add(coord);
            }
        }
        return res;
    }

    public class Wall : Cell
    {
        public Wall(Coord coord) : base(coord) { }
        public override string ToString()
        {
            return "#";
        }
    }
    public class Free : Cell
    {
        public enum Trap
        {
            None,
            Circle,
            Square
        }
        public enum MatColor
        {
            White,
            Red,
            Green
        }


        public Trap Type { get; set; }
        public MatColor Color { get; set; }
        public Free(Coord coord) : base(coord) { }

        public override string ToString()
        {
            switch (this.Type)
            {
                case Trap.Circle:
                    return "o";
                case Trap.Square:
                    return "¤";
                default:
                    return " ";
            }
        }
    }

    public abstract class Cell
    {
        public Coord Coord { private set; get; }

        public Cell(Coord coord)
        {
            this.Coord = coord;
        }
    }


    public readonly int Width;
    public readonly int Height;
    private Cell[,] cells;

    public Cell this[Coord coord]
    {
        get
        {
            return cells[coord.X, coord.Y];
        }
        set
        {
            this.cells[coord.X, coord.Y] = value;
        }
    }


    public Maze(int width, int height, Coord start)
    {
        this.Width = width;
        this.Height = height;
        this.cells = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Coord coord = new Coord(x, y);
                this[coord] = new Wall(coord);
            }
        }
        this.Start = start;
    }
    
    public bool IsValid(Coord coord)
    {
        return coord.X >= 0 && coord.X < this.Width && coord.Y >= 0 && coord.Y < this.Height;
    }

    public override string ToString()
    {
        string[] lines = new string[this.Height];
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                lines[y] = lines[y] ?? "";
                lines[y] += this.cells[x, y];
            }
        }
        return string.Join("\n", lines);
    }

    public IEnumerator<Cell> GetEnumerator()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                yield return this.cells[x, y];
            }
        }
    }

}
