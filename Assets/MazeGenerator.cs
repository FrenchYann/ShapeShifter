using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class MazeGenerator {



    private static int holePhase = 0;
    private static int colorPhase = 0;
    private static int maxDepth = -1;

    public static Maze GenerateMaze(long seed,  LevelDesign.Data data)
    {
        maxDepth = -1;
        UnityEngine.Random.seed = (int)(seed % int.MaxValue);

        int width = RoundMazeSize(data.size.Width);
        int height = RoundMazeSize(data.size.Height);

        Maze maze = new Maze(width, height, Maze.DefaultStart);

        Iterate(maze, maze.Start);
        
        SetHolesAndColors(maze, data);
        BreakWalls(maze, data);
        return maze;
    }

    private static void PrepareEnd(Maze maze, Coord end)
    {
        maze[end] = new Maze.Free(end);
        List<Coord> links = maze.GetNeighbors(end);
        for (int i = links.Count -1; i >= 0; i--)
        {

            if (links[i].X == maze.Width - 1 || links[i].X == 0 ||
                links[i].Y == maze.Height - 1 || links[i].Y == 0)
            {
                links.RemoveAt(i);
            }
        }
        Coord link = links.Random();
        maze[link] = new Maze.Free(link);
    }

    private static void SetHolesAndColors(Maze maze, LevelDesign.Data data)
    {
        List<Maze.Free> colorable = new List<Maze.Free>();
        List<Maze.Free> holable = new List<Maze.Free>();
        for (int x = 0; x < maze.Width; x++)
        {
            for (int y = 0; y < maze.Height; y++)
            {
                Coord coord = new Coord(x, y);
                Maze.Cell cell = maze[coord];
                if (coord == maze.Start || coord == maze.End) continue;

                if (cell is Maze.Free)
                {
                    Maze.Free free = (Maze.Free)cell;
                    if((x + y) % 2 == 0)
                    {
                        colorable.Add(free);
                    }
                    else
                    {
                        holable.Add(free);
                    }

                }
            }
        }
        colorable.Shuffle();
        holable.Shuffle();
        int greenCount = (int)(data.greenPercent * colorable.Count);
        int redCount = (int)(data.redPercent * colorable.Count);
        int circleCount = (int)(data.circlePercent * holable.Count);
        int squareCount = (int)(data.squarePercent * holable.Count);

        for (int i = 0; i < greenCount; i++)
        {
            colorable.Last().Color = Maze.Free.MatColor.Green;
            colorable.RemoveAt(colorable.Count - 1);
        }
        for (int i = 0; i < redCount; i++)
        {
            colorable.Last().Color = Maze.Free.MatColor.Red;
            colorable.RemoveAt(colorable.Count - 1);
        }
        for (int i = 0; i < circleCount; i++)
        {
            holable.Last().Type = Maze.Free.Trap.Circle;
            holable.RemoveAt(holable.Count - 1);
        }
        for (int i = 0; i < squareCount; i++)
        {
            holable.Last().Type = Maze.Free.Trap.Square;
            holable.RemoveAt(holable.Count - 1);
        }


    }
    
    public static int FreeCellCountFromSize(Maze.Size size)
    {
        int freeColumns = (size.Width - 2) / 2 + 1;
        int pathWaysBetweenColumns = (size.Width - 2) - freeColumns;
        int columnsLength = size.Height - 2;
        return freeColumns * columnsLength + pathWaysBetweenColumns;
    }
    public static int WallCellCountFromSize(Maze.Size size)
    {
        return (size.Width - 2) * (size.Height - 2) - FreeCellCountFromSize(size);
    }

    private static void BreakWalls(Maze maze, LevelDesign.Data data)
    {
        List<Coord> wallCoords = new List<Coord>();
        foreach(Maze.Cell cell in maze)
        {
            // we don't break outer walls for now
            if (cell.Coord.X > 0 && cell.Coord.Y > 0 && 
                cell.Coord.X < maze.Width - 1 && cell.Coord.Y < maze.Height - 1 && 
                cell is Maze.Wall)
            {
                wallCoords.Add(cell.Coord);
            }
        }
        wallCoords.Shuffle();
        int count = (int)(data.brokenWallPercent * wallCoords.Count); 
        for(int i = 0; i < count; i++)
        {
            maze[wallCoords[i]] = null;
        }

    }

    private static int RoundMazeSize(int number)
    {
        return number / 2 * 2 + 1;
    }



    private static void Iterate(Maze maze, Coord position, int distance = 0)
    {
        if (distance > maxDepth)
        {
            maxDepth = distance;
            maze.End = position;
        }
        maze[position] = new Maze.Free(position);
        List<Coord> cardinals = new List<Coord>(Coord.ForeachCardinalDirection());
        cardinals.Shuffle();
        foreach (Coord dir in cardinals)
        {
            Coord leap = position + dir * 2;
            if (maze.IsValid(leap) && maze[leap] is Maze.Wall)
            {
                Coord interval = position + dir;
                maze[interval] = new Maze.Free(interval);
                Iterate(maze, leap, (distance+1));
            }
        }
    }

    private static Maze.Free CreateNextFreeColoredCell(Maze maze, Coord position)
    {

        Maze.Free cell = new Maze.Free(position);

        if (position != maze.Start && position != maze.End && UnityEngine.Random.value < 0.1f)
        {
            cell.Color = (colorPhase % 2 == 0) ? Maze.Free.MatColor.Red: Maze.Free.MatColor.Green;
            //cell.Type = UnityEngine.Random.value < 0.5f ? Maze.Free.Trap.Circle : Maze.Free.Trap.Square;
            colorPhase += UnityEngine.Random.value < 0.7f ? 1 : 2;

        }
        return cell;
    }
    private static Maze.Free CreateNextFreeHoledCell(Maze maze, Coord position)
    {

        Maze.Free cell = new Maze.Free(position);

        if (position != maze.Start && position != maze.End && UnityEngine.Random.value < 0.1f)
        {
            cell.Type = (holePhase % 2 == 0) ? Maze.Free.Trap.Circle : Maze.Free.Trap.Square;
            //cell.Type = UnityEngine.Random.value < 0.5f ? Maze.Free.Trap.Circle : Maze.Free.Trap.Square;
            holePhase += UnityEngine.Random.value < 0.7f ? 1 : 2;

        }
        return cell;
    }
}
