using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private string horizontal = "Horizontal";    // キー入力用の文字列指定
    private string jump = "Jump";

    private Rigidbody2D rb;                      // コンポーネントの取得用

    private float scale;                         // 向きの設定に利用する


    private Animator anim;

    private float limitPosX = 8.2f;           // 横方向の制限値
    private float limitPosY = 4.45f;          // 縦方向の制限値

    ////* ここまで *////


    public float moveSpeed;                      // 移動速度
    public float jumpPower;                      // ジャンプ・浮遊力

    public bool isGrounded;

    [SerializeField, Header("Linecast用 地面判定レイヤー")]
    private LayerMask groundLayer;


    void Start()
    {
        // 必要なコンポーネントを取得して用意した変数に代入
        rb = GetComponent<Rigidbody2D>();

        scale = transform.localScale.x;
        anim = GetComponent<Animator>();
    }



    void Update()
    {
        // 地面接地  Physics2D.Linecastメソッドを実行して、Ground Layerとキャラのコライダーとが接地している距離かどうかを確認し、接地しているなら true、接地していないなら false を戻す
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, groundLayer);

        // Sceneビューに Physics2D.LinecastメソッドのLineを表示する
        Debug.DrawLine(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, Color.red, 1.0f);

        // ジャンプ
        if (Input.GetButtonDown(jump))
        {
            Jump();
        }

        // 接地していない(空中にいる)間で、落下中の場合
        if (isGrounded == false && rb.velocity.y < 0.15f)
        {
            // 落下アニメを繰り返す
            anim.SetTrigger("Fall");
        }
    }

    /// <summary>
    /// ジャンプと空中浮遊
    /// </summary>
    private void Jump()
    {

        // キャラの位置を上方向へ移動させる(ジャンプ・浮遊)
        rb.AddForce(transform.up * jumpPower);

        // Jump(Up + Mid) アニメーションを再生する
        anim.SetTrigger("Jump");
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

            // 待機状態のアニメの再生を止めて、走るアニメの再生への遷移を行う
            anim.SetBool("Idle", false);
            anim.SetFloat("Run", 0.5f);

        }
        else
        {
            //  左右の入力がなかったら横移動の速度を0にしてピタッと止まるようにする
            rb.velocity = new Vector2(0, rb.velocity.y);

            //  走るアニメの再生を止めて、待機状態のアニメの再生への遷移を行う
            anim.SetFloat("Run", 0.0f);
            anim.SetBool("Idle", true);
        }


        ////* ここから追加 *////

        // 現在の位置情報が移動範囲の制限範囲を超えていないか確認する。超えていたら、制限範囲内に収める
        float posX = Mathf.Clamp(transform.position.x, -limitPosX, limitPosX);
        float posY = Mathf.Clamp(transform.position.y, -limitPosY, limitPosY);

        // 現在の位置を更新(制限範囲を超えた場合、ここで移動の範囲を制限する)
        transform.position = new Vector2(posX, posY);


        ////* ここまで *////

    }




}
