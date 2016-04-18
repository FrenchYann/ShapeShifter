using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField]
    Text easyLabel = null;
    [SerializeField]
    Text mediumLabel = null;
    [SerializeField]
    Text hardLabel = null;

    void Awake()
    {
        DisplayCurrentLevels();
    }

    private void DisplayCurrentLevels()
    {
        DisplayCurrentLevel(LevelDesign.Difficulty.Easy, this.easyLabel);
        DisplayCurrentLevel(LevelDesign.Difficulty.Medium, this.mediumLabel);
        DisplayCurrentLevel(LevelDesign.Difficulty.Hard, this.hardLabel);
    }

    private void DisplayCurrentLevel(LevelDesign.Difficulty difficulty, Text label)
    {
        label.text = string.Format("lvl: {0}", GetCurrentLevel(difficulty));
    }

    private int GetCurrentLevel(LevelDesign.Difficulty difficulty)
    {
        if (PlayerPrefs.HasKey(difficulty.ToString()))
        {
            return PlayerPrefs.GetInt(difficulty.ToString());
        }
        else
        {
            return 1;
        }
    }


    public void Easy()
    {
        this.Play(LevelDesign.Difficulty.Easy);
    }
    public void Medium()
    {
        this.Play(LevelDesign.Difficulty.Medium);
    }
    public void Hard()
    {
        this.Play(LevelDesign.Difficulty.Hard);
    }
    public void EasyReset()
    {
        this.Reset(LevelDesign.Difficulty.Easy);
    }
    public void MediumReset()
    {
        this.Reset(LevelDesign.Difficulty.Medium);
    }
    public void HardReset()
    {
        this.Reset(LevelDesign.Difficulty.Hard);
    }

    private void Play(LevelDesign.Difficulty difficulty)
    {
        GameManager.Difficulty = difficulty;
        GameManager.Level = this.GetCurrentLevel(difficulty);
        SceneManager.LoadScene("main");
    }

    private void Reset(LevelDesign.Difficulty difficulty)
    {
        PlayerPrefs.DeleteKey(difficulty.ToString());
        DisplayCurrentLevels();
    }


}
