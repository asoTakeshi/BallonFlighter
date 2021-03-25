using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    public GameObject impactPrefab;
    float span = 1.5f;
    float delta = 0;
    public float speed = 10f;  //スピードの代入
    Rigidbody2D rb;
    [Header("攻撃力")]
    public int at;
    public GameObject bulletPrefab;
    public Transform shotPoints;
    public void Shot(float direction)
    {
        //Debug.Log(direction);
        rb = GetComponent<Rigidbody2D>();     //Rigidbody2Dのコンポーネントを変数に代入
        rb.velocity = transform.right * speed * direction;

    }
    void Update()
    {
        this.delta += Time.deltaTime;
        if (this.delta > this.span)
        {
            this.delta = 0;
            GameObject bullet = Instantiate(bulletPrefab, shotPoints.position, transform.rotation);
            bullet.GetComponent<BulletManeger>().Shot(transform.localScale.x -4);

        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        //バルーンに当たったら
        if (col.gameObject.tag == "Ballon")
        {

            //ダメージを与える
            //EnemyManager enemy = col.gameObject.GetComponent<EnemyManager>();
            //enemy.OnDamage(at);

            GameObject effect = Instantiate(impactPrefab, transform.position, transform.rotation);

            Destroy(effect, 1.0f);
            //破裂
            //Destroy(col.gameObject);

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }
}

