using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class LevelDesign 
{
    public struct Data
    {
        public float circlePercent;
        public float squarePercent;
        public float brokenWallPercent;
        public float greenPercent;
        public float redPercent;
        public Maze.Size size;
        public float fallTime;

        public static readonly Data Default =
            new Data()
            {
                circlePercent = 0.2f,
                squarePercent = 0.2f,
                greenPercent = 0,
                redPercent = 0,
                brokenWallPercent = 0,
                size = new Maze.Size(7, 4),
                fallTime = -1
            };

        internal void Normalize()
        {
            float shapeTotal = circlePercent + squarePercent;
            float colorTotal = redPercent + greenPercent;
            if (shapeTotal > 1)
            {
                this.circlePercent /= shapeTotal;
                this.squarePercent /= shapeTotal;
            }
            if (colorTotal > 1)
            {
                this.redPercent /= colorTotal;
                this.greenPercent /= colorTotal;
            }
            brokenWallPercent = Mathf.Clamp01(brokenWallPercent);
        }
    }

    private class Bounds
    {
        public Data Lower;
        public Data Upper;
        public Data Lerp(float percent)
        {
            Data data = new Data()
            {
                circlePercent = Mathf.Lerp(this.Lower.circlePercent, this.Upper.circlePercent, percent),
                squarePercent = Mathf.Lerp(this.Lower.squarePercent, this.Upper.squarePercent, percent),
                redPercent = Mathf.Lerp(this.Lower.redPercent, this.Upper.redPercent, percent),
                greenPercent = Mathf.Lerp(this.Lower.greenPercent, this.Upper.greenPercent, percent),
                brokenWallPercent = Mathf.Lerp(this.Lower.brokenWallPercent, this.Upper.brokenWallPercent, percent),
                fallTime = Mathf.Lerp(this.Lower.fallTime, this.Upper.fallTime, percent),
                size = new Maze.Size(
                    (int)Mathf.Round(Mathf.Lerp(this.Lower.size.Width, this.Upper.size.Width, percent)),
                    (int)Mathf.Round(Mathf.Lerp(this.Lower.size.Height, this.Upper.size.Height, percent))
                )
            };
            data.Normalize();
            return data;
        }
    } 

    public enum Difficulty
    {
        Easy,  // just holes
        Medium, // holes and sometimes color
        Hard // full holes and color 
    }

    private static Dictionary<Difficulty, Bounds> bounds = new Dictionary<Difficulty, Bounds>()
    {
        {
            Difficulty.Easy,
            new Bounds () {
                Lower = new Data() {
                        circlePercent = 0.2f,
                        squarePercent = 0.2f,
                        brokenWallPercent = 0,
                        greenPercent = 0,
                        redPercent = 0,
                        size = new Maze.Size(7,4),
                        fallTime = -1
                    },
                Upper = new Data() {
                        circlePercent = 0.5f,
                        squarePercent = 0.5f,
                        brokenWallPercent = 0,
                        greenPercent = 0f,
                        redPercent = 0f,
                        size = new Maze.Size(17,9),
                        fallTime = -1
                    }
            }
        },
        {
            Difficulty.Medium,
            new Bounds () {
                Lower = new Data() {
                        circlePercent = 0.2f,
                        squarePercent = 0.2f,
                        brokenWallPercent = 0,
                        greenPercent = 0.1f,
                        redPercent = 0.1f,
                        size = new Maze.Size(7,4),
                        fallTime = -1
                    },
                Upper = new Data() {
                        circlePercent = 0.5f,
                        squarePercent = 0.5f,
                        brokenWallPercent = 0,
                        greenPercent = 0f,
                        redPercent = 0f,
                        size = new Maze.Size(17,9),
                        fallTime = -1
                    }
            }
        },
        {
            Difficulty.Hard,
            new Bounds () {
                Lower = new Data() {
                        circlePercent = 0.2f,
                        squarePercent = 0.2f,
                        brokenWallPercent = 0,
                        greenPercent = 0,
                        redPercent = 0,
                        size = new Maze.Size(7,4),
                        fallTime = 2
                    },
                Upper = new Data() {
                        circlePercent = 0.5f,
                        squarePercent = 0.5f,
                        brokenWallPercent = 1,
                        greenPercent = 0.5f,
                        redPercent = 0.5f,
                        size = new Maze.Size(17,9),
                        fallTime = 5
                    }
            }
        }
    };


    private static Dictionary<int, Data> predefinedData = new Dictionary<int, Data>()
    {
        { 1, GetFirstLevel() },
        { 2, GetSecondLevel() }
    };

    private static Data GetFirstLevel()
    {
        Data data = Data.Default;
        data.circlePercent = 0;
        data.squarePercent = 0;
        return data;
    }
    private static Data GetSecondLevel()
    {
        Data data = Data.Default;
        int cellCount = MazeGenerator.FreeCellCountFromSize(data.size);
        int maxHoleCount = cellCount / 2;
        //int maxColorCount = cellCount - maxHoleCount;
        float percent = 1f / maxHoleCount;
        data.circlePercent = percent;
        data.squarePercent = 0;
        return data;
    }

    public static Data GetData(int level, int maxLevel, Difficulty difficulty)
    {
        Data res;
        if (predefinedData.ContainsKey(level))
        {
            res = predefinedData[level];
            res.Normalize();
        }
        else
        {
            float percent = level / (float)maxLevel;
            res = bounds[difficulty].Lerp(percent);
        }

        return res;
    }



}
