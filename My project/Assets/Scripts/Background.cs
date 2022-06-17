using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;

    float viewWidth;

    private void Awake()
    {
        float viewHeight = 2 * Camera.main.orthographicSize;
        viewWidth = viewHeight * Camera.main.aspect;
    }

    void Update()
    {
        Vector3 curPos = transform.position;
        Vector3 nexPos = Vector3.left * speed * Time.deltaTime;
        transform.position = curPos + nexPos;

        if (sprites[endIndex].position.x < viewWidth * (-1.1f))
        {
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            //Vector3 frontSpritePos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.right * viewWidth;

            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
        }
    }
}