using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeController : MonoBehaviour
{
    public int time;
    [SceneName]
    public string nextLevel;
    public Text timer;

    void Update()
    {
        int remainingTime = time - Mathf.FloorToInt(Time.timeSinceLevelLoad * 2.5f);

        if (0 <= remainingTime)
        {
            timer.text = remainingTime.ToString("000");
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                StartCoroutine(LoadNextLevel());
                enabled = false;
            }
        }
    }

    private IEnumerator LoadNextLevel()
    {
        var player = GameObject.FindGameObjectWithTag("Player");

        if (player)
        {
            player.SendMessage("TimeOver", SendMessageOptions.DontRequireReceiver);
        }

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(nextLevel);
    }
}
