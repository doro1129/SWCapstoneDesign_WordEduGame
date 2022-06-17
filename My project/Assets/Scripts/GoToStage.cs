using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToStage : MonoBehaviour
{
    public void GoToStageBtn()
    {
        switch (this.gameObject.name)
        {
            case "Stage1":
                SceneManager.LoadScene("Dialogue1");
                break;
            case "Stage2":
                SceneManager.LoadScene("Dialogue2");
                break;
            case "Stage3":
                SceneManager.LoadScene("Dialogue3");
                break;
            case "Stage4":
                SceneManager.LoadScene("Dialogue4");
                break;
            case "Stage5":
                SceneManager.LoadScene("Dialogue5");
                break;

        }
    }
}
