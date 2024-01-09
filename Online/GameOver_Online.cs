using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;



public class GameOver_Online : MonoBehaviourPunCallbacks
{
    
    [SerializeField] GameManager_Online gameManager;
    [SerializeField] GameObject winResult, loseResult;
    [SerializeField] Text winText, loseText;
    [SerializeField] Button[] buttons;
    public static int adsCount = 0;
    private bool first, error, isWin;
    private int rate1, rate2, rensyou;
    // Start is called before the first frame update


    void Start()
    {
        error = true;
        first = true;
        rate1 = PlayerPrefs.GetInt("rate");
        rate2 = OnlineMenuManager.opponentRate;
        rensyou = PlayerPrefs.GetInt("rensyou");
        PlayerPrefs.SetInt("rate", rate1 - 17);
        PlayerPrefs.SetInt("rensyou", 0);
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (first && collision.transform.tag == "Active")
        {
            adsCount++;
            error = false;

            if ((!gameManager.isMyTurn && !gameManager.acceptPlayerInput) ||
(gameManager.isMyTurn && gameManager.acceptPlayerInput))
            {
                isWin = true;
            }
            else
            {
                isWin = false;
            }


            Invoke("GameOver", 0.3f);

            first = false;
            //gameManager.RequestBanner();
        }
    }

    private void GameOver()
    {
        disconnect();



#if UNITY_IOS
        int randomNumber = Random.Range(0, 9);
        if (randomNumber == 1) UnityEngine.iOS.Device.RequestStoreReview();
#endif

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
        }

        if (isWin)
        {
            PlayerWin();
        }
        else
        {
            PlayerLose();
        }

        AudioManager.instance.PlaySound();
        PlayerPrefs.SetInt("rate", rate1);
        PlayerPrefs.SetInt("rensyou", rensyou);
        PlayerPrefs.Save();
    }



    private void disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    void PlayerWin()
    {
        int expected = (int)(1 / (1 + Mathf.Pow(10, (rate2 - rate1) / 400f)) * 100);
        int kFactor = 32;

        int ratingDelta = Mathf.RoundToInt(kFactor * (1 - expected / 100f));

        rate1 += ratingDelta;
        rensyou++;

        if (PlayerPrefs.GetInt("maxRate") < rate1)
        {
            PlayerPrefs.SetInt("maxRate", rate1);
        }

        winText.text = "今のレート：" + rate1 + "\n最高レート：" + PlayerPrefs.GetInt("maxRate") + "\n連勝数：" + rensyou;
        winResult.SetActive(true);

    }

    void PlayerLose() 
    {
        int expected = (int)(1 / (1 + Mathf.Pow(10, (rate2 - rate1) / 400f)) * 100);
        int kFactor = 32;

        int ratingDelta = Mathf.RoundToInt(kFactor * (0 - expected / 100f));

        rate1 += ratingDelta;
        rensyou = 0;
        loseText.text = "今のレート：" + rate1 + "\n最高レート：" + PlayerPrefs.GetInt("maxRate") + "\n連勝数：" + rensyou;
        loseResult.SetActive(true);
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!error) return;
        if (PhotonNetwork.IsMasterClient) isWin = true;
        else isWin = false;
        first = false;
        GameOver();
    }
}
