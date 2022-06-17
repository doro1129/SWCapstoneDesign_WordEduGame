using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    public AudioSource ButtonSound;

    // Start is called before the first frame update
    public void Click()
    {
        ButtonSound.Play();
    }
}
