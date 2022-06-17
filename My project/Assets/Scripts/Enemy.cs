using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyScore;
    public int tempHealth;
    int health;
    public int dmg;

    public float speed;

    Rigidbody2D rigid;
    Animator anim;
    public GameObject player;

    public string enemyWord;
    public GameObject enemyText;
    public TextMesh textMesh;
    public GameManager gameManager;

    public float maxAttackDelay;
    public float curAttackDelay;

    public float curDisappearDelay;
    public float maxDisappearDelay;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        //rigid.velocity = Vector2.left * speed;
        anim = GetComponent<Animator>();
        textMesh = enemyText.GetComponent<TextMesh>();
    }

    void Update()
    {
        Move();

        curAttackDelay += Time.deltaTime;
        curDisappearDelay += Time.deltaTime;
    }


    void OnHit(int dmg)
    {
        if (curDisappearDelay < maxDisappearDelay)
            return;

        health -= dmg;
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        if (health > 0)
        {
            gameManager.SfxPlay(GameManager.Sfx.MonsterHit);
        }
        Invoke("ReturnWalk", 0.1f);
        if (health <= 0)
        {
            enemyWord = textMesh.text;

            if (gameObject.tag == "Enemy_Answer")
            {
                Player playerLogic = player.GetComponent<Player>();
                playerLogic.score += enemyScore;
                playerLogic.tempScore += enemyScore;
                playerLogic.numKilledMonster += 1;
                gameManager.IsCorrectWord();

                anim.SetBool("IsDead", true);
                gameManager.SfxPlay(GameManager.Sfx.MonsterDeath);
                Invoke("Disappear", 1.0f);
            }
            else
            {
                if (curAttackDelay < maxAttackDelay)
                    return;

                Attack();
                Debug.Log("답이 틀림");

                curAttackDelay = 0;
            }
        }

        curDisappearDelay = 0;
    }

    void ReturnWalk()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Block")
        {
            if (curAttackDelay < maxAttackDelay)
                return;

            if (gameObject.tag != "Enemy_Answer")
                return;

            Attack();
            curAttackDelay = 0;
        }
    }

    void Attack()
    {
        Debug.Log("Attack");
        anim.SetBool("IsAttacking", true);
        Player playerLogic = player.GetComponent<Player>();
        playerLogic.EnemyAttack();
        transform.position = playerLogic.GunPosition.transform.position + Vector3.up * 0.3f;

        transform.localScale += new Vector3(2, 2, 0);
        speed *= 2;
        Invoke("FinishAttack", 0.35f);
    }

    void FinishAttack()
    {
        Debug.Log("FinishAttack");
        anim.SetBool("IsAttacking", false);
        Time.timeScale = 0;
        speed /= 2;
        transform.localScale -= new Vector3(2, 2, 0);
        gameManager.ShowAnswer();
        Disappear();
    }

    void Disappear()
    {
        GameObject[] enemyAnswer = GameObject.FindGameObjectsWithTag("Enemy_Answer");
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemyAnswer)
        {
            health = tempHealth;
            e.SetActive(false);
        }
        foreach (GameObject e in enemy)
        {
            health = tempHealth;
            e.SetActive(false);
        }

        for (int i = 0; i < 4; i++)
        {
            gameManager.SpawnEnemy();
        }
        anim.SetBool("IsDead", false);
    }

    void Move()
    {
        rigid.MovePosition(rigid.position + Vector2.left * speed * Time.deltaTime);
    }
}