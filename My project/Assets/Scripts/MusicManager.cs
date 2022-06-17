using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager musicManagerInstance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        
    }

    private void Update()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        if (index > 1 && index <= 6)
        {
            Destroy(gameObject);
        }
    }
}
