using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   
    private string horizontal = "Horizontal";    // キー入力用の文字列指定

    private Rigidbody2D rb;                      // コンポーネントの取得用

    private float scale;                         // 向きの設定に利用する


    ////* ここから追加 *////

    private Animator anim;

    ////* ここまで *////

    public float moveSpeed;                      // 移動速度


    void Start()
    {
        // 必要なコンポーネントを取得して用意した変数に代入
        rb = GetComponent<Rigidbody2D>();

        scale = transform.localScale.x;


        ////* ここから追加 *////

        anim = GetComponent<Animator>();

        ////* ここまで *////

    }

    void FixedUpdate()
    {
        // 移動
        Move();
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {

        // 水平(横)方向への入力受付
        float x = Input.GetAxis(horizontal);

        // x の値が 0 ではない場合 = キー入力がある場合
        if (x != 0)
        {

            // velocity(速度)に新しい値を代入して移動
            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

            // temp 変数に現在の localScale 値を代入
            Vector3 temp = transform.localScale;

            // 現在のキー入力値 x を temp.x に代入
            temp.x = x;

            // 向きが変わるときに小数になるとキャラが縮んで見えてしまうので整数値にする            
            if (temp.x > 0)
            {

                //  数字が0よりも大きければすべて1にする
                temp.x = scale;

            }
            else
            {
                //  数字が0よりも小さければすべて-1にする
                temp.x = -scale;
            }

            // キャラの向きを移動方向に合わせる
            transform.localScale = temp;


            ////* ここから追加 *////

            // 待機状態のアニメの再生を止めて、走るアニメの再生への遷移を行う
            anim.SetBool("Idle", false);   // ☆　追加　Idle アニメーションを false にして、待機アニメーションを停止する
            anim.SetFloat("Run", 0.5f);    // ☆　追加  Run アニメーションに対して、0.5f の値を情報として渡す。遷移条件が greater 0.1 なので、0.1 以上の値を渡すと条件が成立してRun アニメーションが再生される

        }
        else
        {
            //  左右の入力がなかったら横移動の速度を0にしてピタッと止まるようにする
            rb.velocity = new Vector2(0, rb.velocity.y);

            //  走るアニメの再生を止めて、待機状態のアニメの再生への遷移を行う
            anim.SetFloat("Run", 0.0f);     // ☆　追加  Run アニメーションに対して、0.f の値を情報として渡す。遷移条件が less 0.1 なので、0.1 以下の値を渡すと条件が成立してRun アニメーションが停止される
            anim.SetBool("Idle", true);     // ☆　追加　Idle アニメーションを true にして、待機アニメーションを再生する

            ////* ここまで追加 *////

        }
    }

}
