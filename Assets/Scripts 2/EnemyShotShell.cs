using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShotShell : MonoBehaviour
{
    public GameObject shellPrefab;
    public float shotSpeed;
    public AudioClip shotSound;
    Rigidbody2D rb;

    // ＜追加＞
    private int count = 0;

    // ＜追加＞

   
    void Update()
    {

        count += 1;

        // １００フレームごとにEnemyShot()メソッドを実行する。
        if (count % 100 == 0)
        {
            EnemyShot();
        }
    }


    public void EnemyShot()
    {

        GameObject shell = Instantiate(shellPrefab, transform.position, Quaternion.identity) as GameObject;

        rb = GetComponent<Rigidbody2D>();　　　  //Rigidbody2Dのコンポーネントを変数に代入
        rb.velocity = transform.right * shotSpeed;   //球を赤軸方向に打つ



        AudioSource.PlayClipAtPoint(shotSound, transform.position);

        Destroy(shell, 1.5f);
    }

}
