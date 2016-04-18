using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;

        }
    }


    [SerializeField]
    private int maxLevel = 50;
    private static int level = 1;
    public static int Level { set { level = value; } get { return level; } }
    public static LevelDesign.Difficulty Difficulty { get; set; }
    private static Maze.Size lastsize;  

    private static long sessionSeed;
    private static float lastCameraHeight;

    private bool @lock = false;

    private PlayerController player;
    private Maze maze;
    // Use this for initialization


    private MazeSpawner spawner;

    static GameManager()
    {
        sessionSeed = (long)Math.Floor(
            (DateTime.Now.ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ).TotalMilliseconds));
    }

    void Awake ()
    {
        spawner = this.GetComponent<MazeSpawner>();

        LevelDesign.Data data = LevelDesign.GetData(level, this.maxLevel, Difficulty);

        //Maze.Size size = this.LevelToSize(level);
        this.maze = MazeGenerator.GenerateMaze(sessionSeed + level, data);
        this.player = spawner.SetupLevel(maze, data);

        if(level == 1 || lastsize != data.size )
        {
            lastCameraHeight = this.CameraHeight(level);
            lastsize = data.size;
        }
        Vector3 position = Camera.main.transform.position;
        position.y = lastCameraHeight;
        Camera.main.transform.position = position;

    }


    public void Win()
    {
        if(!@lock)
        {
            this.@lock = true;
            this.player.DisableControls();
            this.player.PlayWin();
            this.player.SnapTo(this.spawner.FromCoordToPosition(this.maze.End));
            level++;
            SaveLevel(Difficulty, level);
            this.StartCoroutine(this.WinCoroutine());
        }
    }

    private void SaveLevel(LevelDesign.Difficulty difficulty, int level)
    {
        PlayerPrefs.SetInt(difficulty.ToString(), level);
    }

    private IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Lose()
    {
        if (!@lock)
        {
            this.@lock = true;
            this.player.DisableControls();
            this.StartCoroutine(LoseCoroutine());
        }
    }
    
    private IEnumerator LoseCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    private float CameraHeight(int level)
    {
        float current = Mathf.Clamp01(level / (float)maxLevel);
        return Mathf.Lerp(5, 10, current);
    }

}
