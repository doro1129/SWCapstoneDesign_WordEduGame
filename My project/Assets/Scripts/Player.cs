using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isHit;
    bool isRealBullet;

    public int health;
    public int life;
    public int score;
    public int level;
    public int tempScore;

    public int numKilledMonster;
    public int numMonsters;

    public float speed;
    public float maxShotDelay;
    public float curShotDelay;

    public VariableJoystick joy;
    Rigidbody2D rigid;
    Animator anim;
    Vector2 moveVec;

    public GameManager gameManager;
    public ObjectManager objectManager;
    public GameObject Sniper_Bullet;
    public GameObject Demoman_Bullet;
    public GameObject GunPosition;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isRealBullet = false;
    }

    void Update()
    {
        Move();
        Fire();
        Reload();

        IsLevelUp();
    }

    void IsLevelUp()
    {
        if (level == 1 && tempScore >= 960)
        {
            level = 2;
        }
        if (level == 2 && tempScore >= 2880)
        {
            level = 3;
        }
    }

    void Move()
    {
        // 1. Input Value
        //float x = joy.Horizontal;
        float y = joy.Vertical;
        if (isTouchTop && y > 0)
        {
            return;
        }
        if (isTouchBottom && y < 0)
        {
            return;
        }

        // 2. Move Position
        moveVec = new Vector2(0, y) * speed * Time.deltaTime;
        rigid.MovePosition(rigid.position + moveVec);

        if (moveVec.sqrMagnitude == 0)
            return; // #. No input = No Rotation
    }

    public void Fire()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (curShotDelay < maxShotDelay)
            return;

        if (isRealBullet)
            return;

        anim.SetBool("IsFire", true);

        Invoke("RealFire", 0.2f);
    }

    void RealFire()
    {
        gameManager.SfxPlay(GameManager.Sfx.PlayerAttack, 0.05f);
        isRealBullet = true;

        if (level == 0)
        {
            level = 1;
        }

        switch (level)
        {
            case 1:
                GameObject bullet = objectManager.MakeObj("Sniper_Bullet");
                bullet.transform.position = GunPosition.transform.position + Vector3.up * 0.15f;
                Rigidbody2D bulletRigid = bullet.GetComponent<Rigidbody2D>();
                bulletRigid.AddForce(Vector2.right * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletU = objectManager.MakeObj("Sniper_Bullet");
                bulletU.transform.position = GunPosition.transform.position + Vector3.up * 0.2f;
                GameObject bulletD = objectManager.MakeObj("Sniper_Bullet");
                bulletD.transform.position = GunPosition.transform.position + Vector3.up * 0.1f;
                Rigidbody2D bulletRigidU = bulletU.GetComponent<Rigidbody2D>();
                Rigidbody2D bulletRigidD = bulletD.GetComponent<Rigidbody2D>();
                bulletRigidU.AddForce(Vector2.right * 10.00001f, ForceMode2D.Impulse);
                bulletRigidD.AddForce(Vector2.right * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletUU = objectManager.MakeObj("Sniper_Bullet");
                bulletUU.transform.position = GunPosition.transform.position + Vector3.up * 0.3f;
                GameObject bulletCC = objectManager.MakeObj("Demoman_Bullet");
                bulletCC.transform.position = GunPosition.transform.position + Vector3.up * 0.15f;
                GameObject bulletDD = objectManager.MakeObj("Sniper_Bullet");
                bulletDD.transform.position = GunPosition.transform.position;
                Rigidbody2D bulletRigidUU = bulletUU.GetComponent<Rigidbody2D>();
                Rigidbody2D bulletRigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D bulletRigidDD = bulletDD.GetComponent<Rigidbody2D>();
                bulletRigidUU.AddForce(Vector2.right * 10.00002f, ForceMode2D.Impulse);
                bulletRigidCC.AddForce(Vector2.right * 10.00001f, ForceMode2D.Impulse);
                bulletRigidDD.AddForce(Vector2.right * 10, ForceMode2D.Impulse);
                break;
        }

        curShotDelay = 0f;
        anim.SetBool("IsFire", false);
        isRealBullet = false;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
            }
        }
    }

    public void EnemyAttack()
    {
        life = life - 1;
        gameManager.UpdateLifeIcon(life);
        anim.SetBool("IsHurt", true);
        gameManager.SfxPlay(GameManager.Sfx.PlayerHit);

        if (life == 0)
        {
            anim.SetInteger("IsDead", life);
            gameManager.GameOver();
        }
        else
        {
            Invoke("ReturnWalk", 0.35f);
        }
    }

    private void ReturnWalk()
    {
        anim.SetBool("IsHurt", false);
    }
}
