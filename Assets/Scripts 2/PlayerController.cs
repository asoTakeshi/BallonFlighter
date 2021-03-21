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
    private bool isGameOver;                 // GameOver状態の判定用。true ならゲームオーバー。

    public bool isFirstGenerateBallon;       // 初めてバルーンを生成したかを判定するための変数(後程外部スクリプトでも利用するためpublicで宣言する)



    public float moveSpeed;                      // 移動速度
    public float jumpPower;                      // ジャンプ・浮遊力

    public bool isGrounded;

    public GameObject[] ballons;                // GameObject型の配列。インスペクターからヒエラルキーにある Ballon ゲームオブジェクトを２つアサインする

    public int maxBallonCount;                 // バルーンを生成する最大数
    public Transform[] ballonTrans;             // バルーンの生成位置の配列
    public GameObject ballonPrefab;            // バルーンのプレファブ
    public float generateTime;                 // バルーンを生成する時間
    public bool isGenerating;                   // バルーンを生成中かどうかを判定する。false なら生成していない状態。true は生成中の状態
    public float knockbackPower;              // 敵と接触した際に吹き飛ばされる力
    public int coinPoint;                       // コインを獲得すると増えるポイントの総数
    public UIManager uiManager;
    public GameObject bulletPrefab;
    public Transform shotPoint;
    float coolTime = 0.3f;                       //待機時間
    float leftCoolTime;　　　　　　　　　　 // 待機している時間
    bool isRight;


    [SerializeField, Header("Linecast用 地面判定レイヤー")]
    private LayerMask groundLayer;

    [SerializeField]
    private StartChecker StartChecker;

    [SerializeField]
    private AudioClip knockbackSE;                    // 敵と接触した際に鳴らすSE用のオーディオファイルをアサインする

    [SerializeField]
    private GameObject knockbackEffectPrefab;         // 敵と接触した際に生成するエフェクト用のプレファブのゲームオブジェクトをアサインする

    [SerializeField]
    private AudioClip coinSE;                    // コインに接触した際に鳴らすSE用のオーディオファイルをアサインする

    [SerializeField]
    private GameObject coinEffectPrefab; 　　　　　　//コインと接触した際に生成するエフェクト用のプレファブのゲームオブジェクトをアサインする

    
  

    void Start()
    {
        // 必要なコンポーネントを取得して用意した変数に代入
        rb = GetComponent<Rigidbody2D>();

        scale = transform.localScale.x;
        anim = GetComponent<Animator>();

        // 配列の初期化(バルーンの最大生成数だけ配列の要素数を用意する)
        ballons = new GameObject[maxBallonCount];

        leftCoolTime = 0;

        //isRight = true;
    }



    void Update()
    {
        Shot();
        // 地面接地  Physics2D.Linecastメソッドを実行して、Ground Layerとキャラのコライダーとが接地している距離かどうかを確認し、接地しているなら true、接地していないなら false を戻す
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, groundLayer);

        // Sceneビューに Physics2D.LinecastメソッドのLineを表示する
        Debug.DrawLine(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, Color.red, 1.0f);


        ////* ここから修正 *////

        // ballons変数の最初の要素の値が空ではないなら = バルーンが１つ生成されるとこの要素に値が代入される = バルーンが１つあるなら
        if (ballons[0] != null)
        {         // <=　☆　条件を変更する　☆

            ////* ここまで *////

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
        else
        {
           
        }



        // Velocity.y の値が 5.0f を超える場合(ジャンプを連続で押した場合)
        if (rb.velocity.y > 5.0f)
        {

            // Velocity.y の値に制限をかける(落下せずに上空で待機できてしまう現象を防ぐため)
            rb.velocity = new Vector2(rb.velocity.x, 5.0f);
        }


        ////* ここから追加 *////

        // 地面に接地していて、バルーンが生成中ではない場合
        if (isGrounded == true && isGenerating == false)
        {

            // Qボタンを押したら
            if (Input.GetKeyDown(KeyCode.Q))
            {

                // バルーンを１つ作成する
                StartCoroutine(GenerateBallon());
            }
        }
        
        
        


    }
    void Shot()
    {
        leftCoolTime -= Time.deltaTime;
        if (leftCoolTime <= 0)
        {
           
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Instantiate(bulletPrefab, shotPoint.position, transform.rotation);

                leftCoolTime = coolTime;
            }
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
        if (isGameOver == true)
        {
            return;
        }
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
        //Direction(x);

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

        // 現在の位置情報が移動範囲の制限範囲を超えていないか確認する。超えていたら、制限範囲内に収める
        float posX = Mathf.Clamp(transform.position.x, -limitPosX, limitPosX);
        float posY = Mathf.Clamp(transform.position.y, -limitPosY, limitPosY);

        // 現在の位置を更新(制限範囲を超えた場合、ここで移動の範囲を制限する)
        transform.position = new Vector2(posX, posY);
    }


    ////* ここからメソッドを１つ追加 *////

    /// <summary>
    /// バルーン生成
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateBallon()
    {

        // すべての配列の要素にバルーンが存在している場合には、バルーンを生成しない
        if (ballons[1] != null)
        {
            yield break;
        }

        // 生成中状態にする
        isGenerating = true;

        // isFirstGenerateBallon 変数の値が false、つまり、ゲームを開始してから、まだバルーンを１回も生成していないなら
        if (isFirstGenerateBallon == false)
        {
            // 初回バルーン生成を行ったと判断し、true に変更する = 次回以降はバルーンを生成しても、if 文の条件を満たさなくなり、この処理には入らない
            isFirstGenerateBallon = true;

            //Debug.Log("初回のバルーン生成");

            // startChecker 変数に代入されている StartChecker スクリプトにアクセスして、SetInitialSpeed メソッドを実行する
            StartChecker.SetInitialSpeed();
        }

        // １つめの配列の要素が空なら
        if (ballons[0] == null)
        {
            // 1つ目のバルーン生成を生成して、1番目の配列へ代入
            ballons[0] = Instantiate(ballonPrefab, ballonTrans[0]);

            ballons[0].GetComponent<Ballon>().SetUpBallon(this);
        }

        else
        {
            // 2つ目のバルーン生成を生成して、2番目の配列へ代入
            ballons[1] = Instantiate(ballonPrefab, ballonTrans[1]);

            ballons[1].GetComponent<Ballon>().SetUpBallon(this);
        }


        // 生成時間分待機
        yield return new WaitForSeconds(generateTime);

        // 生成中状態終了。再度生成できるようにする
        isGenerating = false;
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        // 接触したコライダーを持つゲームオブジェクトのTagがEnemyなら 
        if (col.gameObject.tag == "Enemy")
        {
            // キャラと敵の位置から距離と方向を計算
            Vector3 direction = (transform.position - col.transform.position).normalized;

            // 敵の反対側にキャラを吹き飛ばす
            transform.position += direction * knockbackPower;

            // 敵との接触用のSE(AudioClip)を再生する
            AudioSource.PlayClipAtPoint(knockbackSE, transform.position);

            // 接触した際のエフェクトを、敵の位置に、クローンとして生成する。生成されたゲームオブジェクトを変数へ代入
            GameObject knockbackEffect = Instantiate(knockbackEffectPrefab, col.transform.position, Quaternion.identity);

            // エフェクトを 0.5 秒後に破棄
            Destroy(knockbackEffect, 0.5f);
        }
    }
    
             
        
    
    /// <summary>
    /// バルーン破壊
    /// </summary>
    
    public void DestroyBallon()
    {
        // TODO 後程、バルーンが破壊される際に「割れた」ように見えるアニメ演出を追加する
        if (ballons[1] != null)
        {
            Destroy(ballons[1]);
        }
        else if(ballons[0] != null)
        {
            Destroy(ballons[0]);
        }
    }
    ////* 新しいメソッドを１つ追加　ここから *////

    // IsTriggerがオンのコライダーを持つゲームオブジェクトを通過した場合に呼び出されるメソッド
    private void OnTriggerEnter2D(Collider2D col)
    {
        // 通過したコライダーを持つゲームオブジェクトの Tag が Coin の場合
        if (col.gameObject.tag == "Coin")
        {
            // 通過したコインのゲームオブジェクトの持つ Coin スクリプトを取得し、point 変数の値をキャラの持つ coinPoint 変数に加算
            coinPoint += col.gameObject.GetComponent<Coin>().point;

            uiManager.UpdateDisplayScore(coinPoint);
            // 通過したコインのゲームオブジェクトを破壊する
            Destroy(col.gameObject);

            //コインとの接触用のSE(AudioClip)を再生する
            AudioSource.PlayClipAtPoint(coinSE, transform.position);

            // 接触した際のエフェクトを、コインの位置に、クローンとして生成する。生成されたゲームオブジェクトを変数へ代入
            GameObject coinEffect = Instantiate(coinEffectPrefab, transform.position, Quaternion.identity);

            // エフェクトを 0.3 秒後に破棄
            Destroy(coinEffect, 0.3f);
            
        }
    }
    /// <summary>
    /// ゲームオーバー
    /// </summary>
    
    public void GameOver()
    {
        isGameOver = true;

        // Console ビューに isGameOver 変数の値を表示する。ここが実行されると true と表示される
        Debug.Log(isGameOver);

        // 画面にゲームオーバー表示を行う
        uiManager.DisplayGameOverInfo();
    }
    //void Direction(float inputX)

    //{
    //    if (isRight && inputX < 0)
    //    {
    //        transform.Rotate(0f, 180f, 0f);
    //        isRight = false;
    //    }

    //    if (!isRight && inputX > 0)
    //    {
    //        transform.Rotate(0f, 180f, 0f);
    //        isRight = true;
    //    }


    //}


}
