using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum Sfx { MonsterAttack, MonsterHit, MonsterDeath, PlayerAttack, PlayerHit, GameClear, GameOver, Button };
    int sfxCursor;

    public Transform[] spawnPoints;
    public int[] answerIndexs;

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Text numDeadMonsterText;
    public Image[] lifeImage;
    public GameObject gameOverSet;
    public GameObject gameClearSet;
    public GameObject gamePauseSet;
    public GameObject meaningWindow;
    public ObjectManager objectManager;

    public List<Words> wordsList;
    public int wordsIndex;
    public bool wordsEnd;
    public GameObject wordText;

    public Words answerWord;
    public int answerIndex;
    List<int> randomIndex;
    List<Words> answers;
    List<Words> notanswers;
    List<Words> correctWords;
    List<Words> incorrectWords;
    Words notAnswerWord;
    bool isAnswer;
    public Text meaningText;
    public bool hasAnswer;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    public GameObject correctAnswerSet;
    public Text correctWordText;
    public Text correctMeaningText;

    List<AllGameData> data;
    bool isClear = false;

    public string WordsFile; 

    private void Awake()
    {
        wordsList = new List<Words>();
        notanswers = new List<Words>();
        ReadWordFile();

        randomIndex = new List<int> { 1, 2, 3, 4 };
        answers = new List<Words> { };

        spawnList = new List<Spawn>();
        ReadSpawnFile();
        hasAnswer = false;

        data = new List<AllGameData>();
        correctWords = new List<Words>();
        incorrectWords = new List<Words>();
    }

    private void Start()
    {
        GameLoad();

        bgmPlayer.Play();
        for (int i = 0; i < 4; i++)
        {
            SpawnEnemy();
        }
    }

    void ReadWordFile()
    {
        //#1.변수 초기화
        wordsList.Clear();
        wordsIndex = 0;
        wordsEnd = false;
        int previousIndex = PlayerPrefs.GetInt("PreviousScene");
        switch (previousIndex)
        {
            case 2:
                WordsFile = "Stage 1_words";
                break;
            case 3:
                WordsFile = "Stage 2_words";
                break;
            case 4:
                WordsFile = "Stage 3_words";
                break;
            case 5:
                WordsFile = "Stage 4_words";
                break;
            case 6:
                WordsFile = "Stage 5_words";
                break;
        }

        //#2.단어 파일 읽기
        TextAsset textFile = Resources.Load(WordsFile) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        //#3.한 줄씩 데이터 저장
        while (stringReader != null)
        {
            string line = stringReader.ReadLine();

            if (line == null)
                break;

            //#.단어 데이터 생성
            Words wordsData = new Words();
            wordsData.word = line.Split(":")[0];
            wordsData.meaning = line.Split(":")[1];
            wordsList.Add(wordsData);
        }

        //#.텍스트 파일 닫기
        stringReader.Close();
    }

    void ReadSpawnFile()
    {
        //#1.변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2.리스폰 파일 읽기
        TextAsset textFile = Resources.Load("Stage 0") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        //#3.한 줄씩 데이터 저
        while (stringReader != null)
        {
            string line = stringReader.ReadLine();

            if (line == null)
                break;

            //#.리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        //#.텍스트 파일 닫기
        stringReader.Close();

        //#.첫번째 스폰 딜레이 적
        nextSpawnDelay = spawnList[0].delay;
    }

    void Update()
    {
        //#.UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
        numDeadMonsterText.text = string.Format("{0} / {1}", playerLogic.numKilledMonster, playerLogic.numMonsters);
    }

    public void UpdateLifeIcon(int life)
    {
        for (int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);
        }
        for (int index = life - 1; index >= 0; index--)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void SpawnEnemy()
    {
        //#.리스폰 인덱스 증가
        spawnIndex++;
        int count = spawnList.Count * 4 + 1;
        if (spawnIndex == count)
        {
            spawnEnd = true;
            Time.timeScale = 0;
            gameClearSet.SetActive(true);
            SfxPlay(Sfx.GameClear);
            isClear = true;
        }

        if (randomIndex.Count == 0)
        {
            randomIndex.AddRange(new int[] { 1, 2, 3, 4 });
            notanswers.Clear();
            //hasAnswer = false;
            //isAnswer = false;
        }

        if (spawnIndex%4 == 1)
        {
            hasAnswer = false;
            isAnswer = true;
        }
        else
        {
            isAnswer = false;
        }

        int ranPoint = randomIndex[Random.Range(0, randomIndex.Count)];
        GameObject enemy = objectManager.MakeObj("Enemy");
        enemy.transform.position = spawnPoints[ranPoint].position;

        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        Player playerLogic = player.GetComponent<Player>();
        enemyLogic.gameManager = playerLogic.gameManager;

        int range;
        if (isAnswer && !hasAnswer)
        {
            do
            {
                range = Random.Range(0, wordsList.Count);
                answerIndex = range;
                answerWord = wordsList[answerIndex];
                enemyLogic.textMesh.text = string.Format("{0}", answerWord.word);
                meaningText.text = string.Format("{0}", answerWord.meaning);
                hasAnswer = true;
            } while (answers.Contains(answerWord));
        }
        else
        {
            do
            {
                range = Random.Range(0, wordsList.Count);
                notAnswerWord = wordsList[range];
                enemyLogic.textMesh.text = string.Format("{0}", notAnswerWord.word);
            } while (notanswers.Contains(notAnswerWord) || notAnswerWord.Equals(answerWord));

            notanswers.Add(notAnswerWord);
        }
        /*
        int retryIndex = notanswers.FindIndex(x => x == answerWord);
        if (retryIndex != -1)
        {
            do
            {
                range = Random.Range(0, wordsList.Count);
                notAnswerWord = wordsList[range];
                enemyLogic.textMesh.text = string.Format("{0}", notAnswerWord.word);
            } while (notanswers.Contains(notAnswerWord) || notAnswerWord.Equals(answerWord));

            notanswers[retryIndex] = notAnswerWord;
        } 
         */

        if (isAnswer)
        {
            enemy.tag = "Enemy_Answer";
            Debug.Log("답 " + answerWord.word);
            answers.Add(answerWord);
            wordsList.Remove(answerWord);
        }
        
        randomIndex.Remove(ranPoint);
        
        //#.다음 리스폰 딜레이 갱신
        //nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    public void IsCorrectWord()
    {
        correctWords.Add(answerWord);
    }

    public void ShowAnswer()
    {
        meaningWindow.SetActive(false);
        correctAnswerSet.SetActive(true);
        
        correctWordText.text = string.Format("{0}", answerWord.word);
        correctMeaningText.text = string.Format("{0}", answerWord.meaning);
        incorrectWords.Add(answerWord);
    }

    public void StopShowAnswer()
    {
        correctAnswerSet.SetActive(false);
        meaningWindow.SetActive(true);
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        SfxPlay(Sfx.GameOver);
        Invoke("GameOverStop", 1.4f);
    }

    private void GameOverStop()
    {
        Time.timeScale = 0;
        gameOverSet.SetActive(true);
    }

    public void GamePause()
    {
        SfxPlay(Sfx.Button);
        gamePauseSet.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameContinue()
    {
        Time.timeScale = 1;
        SfxPlay(Sfx.Button);
        gamePauseSet.SetActive(false);
    }

    public void GoBackStage()
    {
        SceneManager.LoadScene("SelectStage");
        GameSave();
        _save();
    }

    public void GameRetry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameSave();
        _save();
    }

    public void GameSave()
    {
        Player playerLogic = player.GetComponent<Player>();

        PlayerPrefs.SetInt("Level", playerLogic.level);

        PlayerPrefs.SetInt("Score", playerLogic.tempScore);

        if (isClear)
        {
            if (PlayerPrefs.GetInt("PreviousScene") - 2 <= 5)
                PlayerPrefs.SetInt("levelAt", PlayerPrefs.GetInt("PreviousScene") - 1);
        }

        //저장
        PlayerPrefs.Save();
    }

    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey("Level"))
            return;

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.level = PlayerPrefs.GetInt("Level");
        playerLogic.tempScore = PlayerPrefs.GetInt("Score");
        _load();
    }

    public void GameQuit()
    {
        GameSave();
        _save();
        Application.CancelQuit();
#if !UNITY_EDITOR
System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }

    public void SfxPlay(Sfx type, float volume = 0.3f)
    {
        switch (type)
        {
            case Sfx.MonsterAttack:
                sfxPlayer[sfxCursor].clip = sfxClip[0];
                break;
            case Sfx.MonsterHit:
                sfxPlayer[sfxCursor].clip = sfxClip[1];
                break;
            case Sfx.MonsterDeath:
                sfxPlayer[sfxCursor].clip = sfxClip[2];
                break;
            case Sfx.PlayerAttack:
                sfxPlayer[sfxCursor].clip = sfxClip[3];
                break;
            case Sfx.PlayerHit:
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
            case Sfx.GameClear:
                sfxPlayer[sfxCursor].clip = sfxClip[5];
                break;
            case Sfx.GameOver:
                sfxPlayer[sfxCursor].clip = sfxClip[6];
                break;
            case Sfx.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[7];
                break;
        }

        sfxPlayer[sfxCursor].volume = volume;
        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }

    public void _save()
    {
        string path = Path.Combine(Application.dataPath, "AllGameData.json");
        
        bool isAdd = false;
        for (int i = 0; i < data.Count; ++i)
        {
            if (data[i].stage == PlayerPrefs.GetInt("PreviousScene") - 1)
            {
                data[i] = new AllGameData(System.DateTime.Now.ToString("yyyy/MM/dd"),
                    PlayerPrefs.GetInt("PreviousScene") - 1,
                    correctWords,
                    incorrectWords);
                isAdd = true;
            }

        }
        if (!isAdd)
        {
            data.Add(new AllGameData(System.DateTime.Now.ToString("yyyy/MM/dd"),
                PlayerPrefs.GetInt("PreviousScene") - 1,
                correctWords,
                incorrectWords));
        }
        
        string jdata = JsonConvert.SerializeObject(data);
        File.WriteAllText(path, jdata);
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

public class AllGameData
{
    public string date;
    public int stage;
    public List<Words> corretWordsData;
    public List<Words> incorretWordsData;

    public AllGameData(string date, int stage, List<Words> corretWordsData, List<Words> incorretWordsData)
    {
        this.date = date;
        this.stage = stage;
        this.corretWordsData = corretWordsData;
        this.incorretWordsData = incorretWordsData;
    }
}