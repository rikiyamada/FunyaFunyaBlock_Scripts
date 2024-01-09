using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject RewardPanel;

    public bool first = false, watchedReward = false;
    // Start is called before the first frame update


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        first = true;
    }

    // Update is called once per frame

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (first)
        {
            first = false;
            GameFinish();
        }

        Destroy(collision.gameObject);
    }

    private void GameFinish()
    {
        gameManager.acceptPlayerInput = false;

        if(gameManager.score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", gameManager.score);
        }

        if (!watchedReward && gameManager.score > PlayerPrefs.GetInt("HighScore") - 5)
        {
            RewardPanel.SetActive(true);
        }
        else
        {
            gameManager.showResult();
#if UNITY_IOS
        int randomNumber = Random.Range(0, 9);
        if (randomNumber == 1) UnityEngine.iOS.Device.RequestStoreReview();
#endif
        }
    }
}
