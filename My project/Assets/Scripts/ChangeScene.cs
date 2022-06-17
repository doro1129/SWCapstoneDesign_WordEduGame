using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void ChangeSceneBtn()
    {
        switch(this.gameObject.name)
        {
            case "StartBtn":
                SceneManager.LoadScene("SelectStage");
                break;
            case "StageBtn":
                SceneManager.LoadScene("SelectStage");
                break;
            case "ReviewBtn":
                SceneManager.LoadScene("Review");
                break;
            case "GraphBtn":
                SceneManager.LoadScene("ProgressGraph");
                break;
            case "QuitBtn":
                {
                    Application.CancelQuit();
#if !UNITY_EDITOR
System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
                    break;
                }            
        }
    }
}
