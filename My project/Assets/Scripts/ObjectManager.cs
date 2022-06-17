using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject bulletSniperPrefab;
    public GameObject bulletDemomanPrefab;

    GameObject[] enemy;

    GameObject[] bulletSniper;
    GameObject[] bulletDemoman;

    GameObject[] targetPool;

    void Awake()
    {
        enemy = new GameObject[15];

        //item = new GameObject[3];

        bulletSniper = new GameObject[10];
        bulletDemoman = new GameObject[5];

        Generate();
    }

    void Generate()
    {
        //#1.Enemy
        for(int index = 0; index < enemy.Length; index++)
        {
            enemy[index] = Instantiate(enemyPrefab);
            enemy[index].SetActive(false);
        }
        //#2.Bullet
        for (int index = 0; index < bulletSniper.Length; index++)
        {
            bulletSniper[index] = Instantiate(bulletSniperPrefab);
            bulletSniper[index].SetActive(false);
        }
        for (int index = 0; index < bulletDemoman.Length; index++)
        {
            bulletDemoman[index] = Instantiate(bulletDemomanPrefab);
            bulletDemoman[index].SetActive(false);
        }
    }

    public GameObject MakeObj(string type)
    {
        switch (type)
        {
            case "Enemy":
                targetPool = enemy;
                break;
            case "Sniper_Bullet":
                targetPool = bulletSniper;
                break;
            case "Demoman_Bullet":
                targetPool = bulletDemoman;
                break;
        }

        for (int index = 0; index < targetPool.Length; index++)
        {
            if (!targetPool[index].activeSelf)
            {
                targetPool[index].SetActive(true);
                return targetPool[index];
            }
        }

        return null;
    }
}
