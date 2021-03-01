using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text txtScore;   // txtScore ゲームオブジェクトの持つ Text コンポーネントをインスペクターからアサインする


    /// <summary>
    /// スコア表示を更新
    /// </summary>
    /// <param name="score"></param>

    public void UpdateDisplayScore(int score)
    {
        txtScore.text = score.ToString();
    }
    
        
    
}
