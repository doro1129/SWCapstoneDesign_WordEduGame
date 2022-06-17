using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageLock : MonoBehaviour
{
    int levelat; //현재 스테이지 번호, 오픈한 스테이지 번호
    public GameObject stageNumObject;
    
    void Start()
    {
        Button[] stages = stageNumObject.GetComponentsInChildren<Button>();
        Debug.Log("Start" + levelat);

        levelat = PlayerPrefs.GetInt("levelAt");

        for (int i = levelat + 1; i < stages.Length; i++)
        {
            stages[i].interactable = false;
            stages[i].GetComponent<Image>().color = Color.gray;
        }
    }
}
