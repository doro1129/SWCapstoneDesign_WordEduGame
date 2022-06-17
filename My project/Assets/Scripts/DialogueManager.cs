using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText;
    public List<Dialogue> dialogueList;
    public int dialogueIndex;
    public bool dialogueEnd;

    public Sprite[] sprites;
    public Image dialogueWindow;
    public GameObject PortraitPlayer;
    public Image PortraitNPC;
    public int speaker;

    //public Sprite[] placeSprites;
    //public int characterPlace;
    public int textIndex;
    public bool textDone;

    public string dialogueFile;
    public string stageFile;

    private void Awake()
    {
        dialogueList = new List<Dialogue>();
        ReadWordFile();
        textDone = false;
        textIndex = 0;

        dialogueWindow.gameObject.SetActive(false);
        PortraitPlayer.SetActive(false);
        PortraitNPC.gameObject.SetActive(false);
    }

    void ReadWordFile()
    {
        //#1.변수 초기화
        dialogueList.Clear();
        dialogueIndex = 0;
        dialogueEnd = false;

        //#2.단어 파일 읽기
        TextAsset textFile = Resources.Load(dialogueFile) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        //#3.한 줄씩 데이터 저
        while (stringReader != null)
        {
            string line = stringReader.ReadLine();

            if (line == null)
                break;

            //#.단어 데이터 생성
            Dialogue dialogueData = new Dialogue();
            dialogueData.speaker = line.Split(",")[0];
            dialogueData.talk = line.Split(",")[1];
            dialogueList.Add(dialogueData);
        }

        //#.텍스트 파일 닫기
        stringReader.Close();
    }

    public void NextTalk()
    {
        dialogueWindow.gameObject.SetActive(true);
        switch (dialogueList[textIndex].speaker)
        {
            case "Player":
                speaker = 0;
                PortraitPlayer.gameObject.SetActive(true);
                PortraitNPC.gameObject.SetActive(false);
                break;
            case "NPC1":
                speaker = 1;
                PortraitPlayer.gameObject.SetActive(false);
                PortraitNPC.gameObject.SetActive(true);
                break;
        }
        PortraitNPC.sprite = sprites[speaker];

        /*
        switch (dialogueList[textIndex].place)
        {
            case "Home":
                characterPlace = 0;
                PortraitPlayer.gameObject.SetActive(true);
                PortraitNPC.gameObject.SetActive(false);
                break;
            case "Forest":
                characterPlace = 1;
                PortraitPlayer.gameObject.SetActive(false);
                PortraitNPC.gameObject.SetActive(true);
                break;
            case "Village":
                characterPlace = 2;
                PortraitPlayer.gameObject.SetActive(false);
                PortraitNPC.gameObject.SetActive(true);
                break;
        } 
         */


        textIndex++;
        
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (textIndex < dialogueList.Count)
            {
                dialogueText.text = string.Format("{0}", dialogueList[textIndex].talk);
                NextTalk();
            }
            else
            {
                PlayerPrefs.SetInt("PreviousScene", SceneManager.GetActiveScene().buildIndex);
                SceneManager.LoadScene("Stage1");
            }
        }

    }
}
