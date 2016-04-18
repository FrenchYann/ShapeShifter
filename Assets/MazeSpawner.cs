using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Serialization;

public class MazeSpawner : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("floor")]
    private Floor floorPrefab = null;
    [SerializeField, FormerlySerializedAs("wall")]
    private GameObject wallPrefab = null;

    [SerializeField, FormerlySerializedAs("ground")]
    private GameObject groundPrefab = null;

    [SerializeField, FormerlySerializedAs("player")]
    private PlayerController playerPrefab = null;

    private float x0;
    private float y0;

    public PlayerController SetupLevel(Maze maze, LevelDesign.Data data)
    {
        this.x0 = -maze.Width / 2f + 0.5f;
        this.y0 = -maze.Height / 2f + 0.5f;
        this.SpawnMaze(maze, data);
        return this.SpawnPlayer(maze);
    }

    public Vector3 FromCoordToPosition(Coord coord)
    {
        return new Vector3(x0 + coord.X, 0, y0 + coord.Y);
    }

    private void SpawnMaze(Maze maze, LevelDesign.Data data)
    {
        Transform mazeContainer = new GameObject("maze").transform;

        GameObject ground = (GameObject)UnityEngine.Object.Instantiate(
            this.groundPrefab,
            Vector3.zero,
            Quaternion.identity
        );
        ground.transform.localScale = new Vector3(maze.Width-2, 1, maze.Height-2);
        ground.transform.SetParent(mazeContainer);
        foreach (Maze.Cell cell in maze)
        {
            if (cell == null) continue;
            if (cell is Maze.Wall)
            {
                GameObject wall = (GameObject)UnityEngine.Object.Instantiate(
                    this.wallPrefab,
                    this.FromCoordToPosition(cell.Coord),
                    Quaternion.identity
                );
                wall.transform.SetParent(mazeContainer);
            }
            else
            {
                Floor floor = (Floor)UnityEngine.Object.Instantiate(
                    this.floorPrefab,
                    this.FromCoordToPosition(cell.Coord),
                    Quaternion.identity
                );
                floor.SetFallTime(data.fallTime);
                floor.SetMeshAndColor(cell);
                if (cell.Coord == maze.Start)
                {
                    floor.SetStart();
                }
                else  if (cell.Coord == maze.End)
                {
                    floor.SetEnd();
                }

                floor.transform.SetParent(mazeContainer);
            }
        }


    }

    private PlayerController SpawnPlayer(Maze maze)
    {
        PlayerController player = (PlayerController)UnityEngine.Object.Instantiate(
            this.playerPrefab,
            this.FromCoordToPosition(maze.Start),
            Quaternion.identity
        );
        return player;
    }

}
