using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManeger : MonoBehaviour
{
    public GameObject impactPrefab;
    float speed = 10f;  //スピードの代入
    Rigidbody2D rb;
    [Header("攻撃力")]
    public int at;

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();　　　  //Rigidbody2Dのコンポーネントを変数に代入
        rb.velocity = transform.right * speed;   //球を赤軸方向に打つ
        
    }   
   

    private void OnCollisionEnter2D(Collision2D col)
    {
        //敵に当たったら
        if (col.gameObject.tag == "Enemy")
        {
            
            //ダメージを与える
            EnemyManager enemy = col.gameObject.GetComponent<EnemyManager>();
            enemy.OnDamage(at);
            
            Instantiate(impactPrefab, transform.position, transform.rotation);
            

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
