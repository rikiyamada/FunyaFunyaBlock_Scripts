using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] GameObject holdBox, RewardPanel, ResultPanel, RankingPanel;
    [SerializeField] private Text scoreText;
    private Block activeBlock, nextBlock, holdBlock;
    private Rigidbody2D rbActive;
    public bool acceptPlayerInput = true;
    private bool isWaiting;
    private Vector2 tapPos1, tapPos2, beforePos, spawnPos;
    public int score;
    private static int adsCount = 0;
    private float maxWaitTime = 10, waitingTime = 0;


    // Start is called before the first frame update
    private void Start()
    {
        nextBlock = spawner.spawnBlock();
        NextToActive();
        nextBlock = spawner.spawnBlock();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!activeBlock) return;
        moveBlock();
        CheckActiveBlock(); //activeBlockが落下して停止したか

        if (isWaiting)
        {
            waitingTime += Time.deltaTime;
        }
    }


    private void spawnBlock()
    {
        spawnerMove();
        AddScore();
        NextToActive();
        nextBlock = spawner.spawnBlock();
        acceptPlayerInput = true;
        isWaiting = false;
    }

    public void dropBlock()
    {
        if (acceptPlayerInput)
        {
            acceptPlayerInput = false;
            isWaiting = true;
            waitingTime = 0;
            PhysicsOn(activeBlock);
        }

        AudioManager.instance.PlaySound();
    }

    private void AddScore()
    {
        score++;
        scoreText.text = score + "";
    }

    private void CheckActiveBlock()
    {
        if (waitingTime > maxWaitTime) //待ち時間が超過したかの判定
        {
            waitingTime = -100000;
            spawnBlock();
            return;
        }

        if (activeBlock.transform.position.y > spawner.transform.position.y - 0.1f) return; //落とされているかの判定
        if (rbActive.velocity.magnitude > 0.01f) return; //停止しているかの判定

        spawnBlock();
    }

    private void moveBlock()
    {
        if (acceptPlayerInput)
        {
            if (Input.GetMouseButtonDown(0))
            {
                tapPos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                beforePos = activeBlock.transform.position;
            }


            if (Input.GetMouseButton(0))
            {
                tapPos2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (activeBlock.transform.position.x > 2.1 || activeBlock.transform.position.x < -2.1)
                {
                    if (activeBlock.transform.position.x * (tapPos2 - tapPos1).x > 0)
                    {
                        return;
                    }
                }
                activeBlock.transform.position = new Vector2((beforePos + (tapPos2 - tapPos1)).x, activeBlock.transform.position.y);

                if (tapPos2.y - tapPos1.y > 1)
                {
                    hold();
                    tapPos1 = new Vector2(tapPos1.x, tapPos2.y + 100);
                }
            }
        }
    }

    public void RotateR()
    {
        if (acceptPlayerInput)
        {
            activeBlock.transform.Rotate(0, 0, -30);
        }

        AudioManager.instance.PlaySound();
    }

    public void RotateL()
    {
        if (acceptPlayerInput)
        {
            activeBlock.transform.Rotate(0, 0, 30);
        }

        AudioManager.instance.PlaySound();
    }

    private void spawnerMove()
    {
        if (!activeBlock) return;
        if (spawnPos.y < activeBlock.transform.position.y + 1.5f)
        {
            spawnPos = new Vector2(spawnPos.x, spawnPos.y + 1.0f);
        }
    }

    private void hold()
    {
        AudioManager.instance.PlaySound();
        PhysicsOff(activeBlock);
        if(holdBlock == null)
        {
            ActiveToHold();
            spawnBlock();
            score--;
            scoreText.text = score + "";
        }
        else
        {
            Block TempHold = holdBlock;
            ActiveToHold();
            HoldToActive(TempHold);
        }
    }

    private void NextToActive()
    {
        activeBlock = nextBlock;
        rbActive = activeBlock.GetComponent<Rigidbody2D>();
        nextBlock = null;
        activeBlock.transform.position = spawnPos;
        activeBlock.transform.localScale = new Vector2(0.8f, 0.8f);
    }

    private void HoldToActive(Block Hold)
    {
        activeBlock = Hold;
        rbActive = activeBlock.GetComponent<Rigidbody2D>();
        activeBlock.transform.position = spawnPos;
        activeBlock.transform.localScale = new Vector2(0.8f, 0.8f);
    }

    private void ActiveToHold()
    {
        Vector2 holdPos = holdBox.GetComponent<RectTransform>().position;
        holdPos += new Vector2(0, -0.07f);

        holdBlock = activeBlock;
        holdBlock.transform.position = holdPos;
        holdBlock.transform.localScale = new Vector2(0.5f, 0.5f);
        activeBlock = null;
    }


    private void PhysicsOn(Block block)
    {
        block.transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        foreach (Collider2D col in block.transform.GetComponents<Collider2D>())
        {
            col.enabled = true;
        }

        foreach(Transform child in block.transform)
        {
            child.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            child.GetComponent<Collider2D>().enabled = true;
        }
    }

    private void PhysicsOff(Block block)
    {
        block.transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        foreach (Collider2D col in block.transform.GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        foreach (Transform child in block.transform)
        {
            child.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            child.GetComponent<Collider2D>().enabled = false;
        }
    }


    public void RewardFinish()
    {
        RewardPanel.SetActive(false);
        acceptPlayerInput = true;
        FindObjectOfType<GameOver>().first = true;
        FindObjectOfType<GameOver>().watchedReward = true;

        score = GameObject.FindGameObjectsWithTag("Player").Length - 2; //ネクスト、アクティブの分を引く
        if (holdBlock) score--;　//ホールドの分を引く
        if (!activeBlock || activeBlock.transform.position.y != spawnPos.y)　//すでに落としてある場合
        {
            spawnBlock();
        }
        scoreText.text = score + "";
    }

    public void showResult()
    {
        acceptPlayerInput = false;
        adsCount++;
        Debug.Log(adsCount);
        if (adsCount == 3)
        {
            adsCount = 0;
            AdManager.instance.showInterstitialAd();
        }

        ResultPanel.SetActive(true);
        GameObject.Find("HighScoreText").GetComponent<Text>().text = $"今までのハイスコア\n{PlayerPrefs.GetInt("HighScore")}個";
        GameObject.Find("ScoreText").GetComponent<Text>().text = $"今回のスコア\n{score}個";

        PlayFabController.Instance.SendPlayScore(score);

    }

    public void RevivalButton()
    {
        AdManager.instance.showRewardAd();
        AudioManager.instance.PlaySound();

    }

    public void GiveUpButton()
    {
        RewardPanel.SetActive(false);
        showResult();
        AudioManager.instance.PlaySound();
    }

    public void HomeButton()
    {
        SceneManager.LoadScene("TitleScene");
        AudioManager.instance.PlaySound();
    }

    public void RetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        AudioManager.instance.PlaySound();
    }

    public void RankingButton()
    {
        RankingPanel.SetActive(true);
        AudioManager.instance.PlaySound();
    }
}
