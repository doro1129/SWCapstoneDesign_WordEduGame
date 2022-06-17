using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class ScoreSlider : MonoBehaviour
{
    public int level;
    public int score;
    public int levelEndScore;
    public Slider scoreSlider;
    public Text scoreText;
    public Text levelText;

    List<AllGameData> data;

    // Start is called before the first frame update
    void Start()
    {
        GameLoad();
        if (level == 0)
        {
            level = 1;
            levelEndScore = 960;
        }
        else if (level == 1)
        {
            levelEndScore = 960;
        }
        else if (level == 2)
        {
            levelEndScore = 2880;
        }

        scoreText.text = string.Format("{0:n0} / {1:n1}", score, levelEndScore);
        scoreSlider.value = score / levelEndScore; 
        levelText.text = string.Format("Level {0}", level);
    }

    void GameLoad()
    {
        if (!PlayerPrefs.HasKey("Level"))
        {
            level = 0;
            score = 0;
        }

        level = PlayerPrefs.GetInt("Level");
        score = PlayerPrefs.GetInt("Score");
        //_load();
    }

    public void _load()
    {
        string path = Path.Combine(Application.dataPath, "AllGameData.json");
        if (!File.Exists(path))
            return;
        string jdata = File.ReadAllText(path);
        data = JsonConvert.DeserializeObject<List<AllGameData>>(jdata);
    }
}

